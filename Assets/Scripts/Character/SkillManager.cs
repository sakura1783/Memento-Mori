using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// スキルのターゲットの種類
/// </summary>
public enum TargetType
{
    Me,
    Ally,  // 自分を含めた味方全員
    Opponent,
    Neighbor,
}

/// <summary>
/// スキルの基本的な処理を記述するクラス
/// 各キャラのスクリプト内で組み合わせや引数を指定し、スキルの挙動を作る
/// </summary>
public static class SkillManager
{
    private static BattleManager battleManager;


    /// <summary>
    /// コンストラクタ
    /// </summary>
    static SkillManager()  // 静的コンストラクタはクラスが参照された際に自動で呼び出されるので、アクセス修飾子を書くことはできない(書くとエラーが出る)
    {
        battleManager = GameObject.Find("Battle").GetComponent<BattleManager>();  // TODO これでもいいけど、何か他にいい方法はないか
    }

    /// <summary>
    /// 発動対象を決めるメソッド(敵or味方or自分or隣接する味方etc... ?名)
    /// </summary>
    /// <param name="user">スキルを使うキャラ</param>
    /// <param name="targetType"></param>
    /// <param name="count">取得するターゲットの数。敵?人、隣接する味方?人、など。count=-1(初期値、値を設定しない場合)で全員、それ以外は指定された数だけターゲットを取得</param>
    /// <returns></returns>
    public static List<CharaController> PickTarget(CharaController user, TargetType targetType, int count = -1)
    {
        List<CharaController> targetList = new();

        switch (targetType)
        {
            case TargetType.Me:
                targetList.Add(user);
                break;

            case TargetType.Ally:
                targetList.AddRange(battleManager.playerTeam.All(chara => chara == user) ? battleManager.playerTeam : battleManager.opponentTeam);
                break;

            case TargetType.Opponent:
                targetList.AddRange(battleManager.playerTeam.All(chara => chara != user) ? battleManager.playerTeam : battleManager.opponentTeam);
                break;

            case TargetType.Neighbor:
                targetList.AddRange(PickNeighbor(user));
                break;
        }

        // 取得するターゲットの数が指定されている場合
        if (count != -1)
        {
            // リストの要素数を超えたターゲットの取得をしないように制御(必ず、count <= targetList.Count となる)
            count = targetList.Count >= count ? count : targetList.Count;

            // countだけランダムに抽出
            targetList = targetList.OrderBy(_ => Random.value).Take(count).ToList();
        }

        // TODO 追加：攻撃力などの値が上位?人など

        return targetList;
    }

    /// <summary>
    /// 隣接するキャラを取得する
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private static List<CharaController> PickNeighbor(CharaController user)
    {
        // userが所属するチームのリストを取得
        var team = battleManager.playerTeam.All(chara => chara == user) ? battleManager.playerTeam : battleManager.opponentTeam;

        // userのインデックス番号を取得
        var userIndex = team.FindIndex(chara => chara == user);
        
        List<CharaController> neighbors = new();
        if (userIndex - 1 < 0)  // 左隣にキャラが存在したら
        {
            neighbors.Add(team[userIndex - 1]);
        }
        if (userIndex + 1 < 4)  // 右隣にキャラが存在したら
        {
            neighbors.Add(team[userIndex + 1]);
        }

        return neighbors;
    }

    /// <summary>
    /// 通常攻撃
    /// </summary>
    /// <param name="target"></param>
    /// <param name="baseValue">基準となる値</param>
    /// <param name="rate"></param>
    public static void Attack(CharaController target, int baseValue, int rate)
    {
        // baseValueのrate分の値を計算し、攻撃対象のHPを削る
        target.UpdateHp(-CalculateManager.CalculateSkillEffectValue(baseValue, rate, target.Status.defencePower));
    }

    /// <summary>
    /// 回復
    /// </summary>
    /// <param name="target"></param>
    /// <param name="baseValue"></param>
    /// <param name="rate"></param>
    public static void Heal(CharaController target, int baseValue, int rate)
    {
        target.UpdateHp(CalculateManager.CalculateSkillEffectValue(baseValue, rate));
    }

    /// <summary>
    /// 最大HPを増加
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rate"></param>
    public static void IncreaseMaxHp(CharaController target, int rate)
    {
        target.Status.MaxHp.Value += CalculateManager.CalculateSkillEffectValue(target.Status.MaxHp.Value, rate);
    }

    /// <summary>
    /// 攻撃力の増加・減少
    /// </summary>
    /// <param name="target"></param>
    /// <param name="baseValue"></param>
    /// <param name="rate"></param>
    /// <param name="isIncrease">trueで増加、falseで減少</param>
    public static void ModifyAttackPower(CharaController target, int baseValue, int rate, bool isIncrease)
    {
        target.Status.attackPower += isIncrease ? +CalculateManager.CalculateSkillEffectValue(baseValue, rate) : -CalculateManager.CalculateSkillEffectValue(baseValue, rate);
    }

    /// <summary>
    /// 防御力の増加・減少
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rate"></param>
    /// <param name="isIncrease"></param>
    public static void ModifyDefencePower(CharaController target, int rate, bool isIncrease)
    {
        target.Status.defencePower += isIncrease ? +CalculateManager.CalculateSkillEffectValue(target.Status.defencePower, rate) : -CalculateManager.CalculateSkillEffectValue(target.Status.defencePower, rate);
    }

    /// <summary>
    /// クリティカル率増加
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rate"></param>
    public static void IncreaseCriticalRate(CharaController target, int rate)
    {
        // クリティカル率のみ、CalculateManagerを利用せずに処理可能
        target.Status.criticalRate += rate;
    }


    /* */
    // 単純な攻撃
    // 回復
    // 最大HP増加
    // 攻撃力増加・減少  // baseValue
    // 防御力増加・減少
    // クリティカル率増加・減少

    /* 未実装 */
    // スピード増加・減少
    // 命中率上昇
    // 回避率上昇

    /* デバフ(状態異常) */
    // 不治(HPが回復しなくなる)
    // 気絶(行動不能になる)
    // 睡眠(行動不能になる ※ダメージを受けると解除)
    // 沈黙(スキルが使用不能になる)
    // 毒(毎ターン行動開始時に現在HP*?%のダメージを受ける)
    // 共鳴(お互いが受けたダメージ*?%のダメージを受けるようになる ※バトル開始時、防御力が最も高い敵と最も低い敵に付与)
}
