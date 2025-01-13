using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// スキルのターゲットの種類
/// </summary>
public enum TargetType
{
    Me,
    Ally,  // 自分を含めた味方全員
    Opponent,
    Neighbor,
    Aggressor,  // 攻撃してきた敵
}

/// <summary>
/// この条件の優劣順にターゲットを取得
/// </summary>
public enum ValueType
{
    None,  // 指定しない場合

    ByAttackPower,
    ByDefencePower,
    ByCurrentHp,
    ByMaxHp,
    ByCriticalRate,
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
    /// <param name="valueType">この条件の優劣順に取得</param>
    /// <param name="isDescending">valueTypeが高い順(降順)に取得するかどうか</param>
    /// <returns></returns>
    public static List<CharaController> PickTarget(CharaController user, TargetType targetType, int count = -1, ValueType valueType = ValueType.None, bool isDescending = true, bool allowDuplicates = false)
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

            case TargetType.Aggressor:
                targetList.Add(battleManager.PreviousActChara);
                break;
        }

        // ValueTypeが高い順・低い順に並び替え
        // switch (valueType)
        // {
        //     case ValueType.None:
        //         break;

        //     case ValueType.ByAttackPower:
        //         if (isDescending) targetList.OrderByDescending(target => target.Status.attackPower).ToList();
        //         else targetList.OrderBy(target => target.Status.attackPower).ToList();
        //         break;

        //     case ValueType.ByDefencePower:
        //         if (isDescending) targetList.OrderByDescending(target => target.Status.defencePower).ToList();
        //         else targetList.OrderBy(target => target.Status.defencePower).ToList();
        //         break;

        //     case ValueType.ByCurrentHp:
        //         if (isDescending) targetList.OrderByDescending(target => target.Status.Hp).ToList();
        //         else targetList.OrderBy(target => target.Status.Hp).ToList();
        //         break;
            
        //     case ValueType.ByMaxHp:
        //         if (isDescending) targetList.OrderByDescending(target => target.Status.MaxHp).ToList();
        //         else targetList.OrderBy(target => target.Status.MaxHp).ToList();
        //         break;

        //     case ValueType.ByCriticalRate:
        //         if (isDescending) targetList.OrderByDescending(target => target.Status.criticalRate).ToList();
        //         else targetList.OrderBy(target => target.Status.criticalRate).ToList();
        //         break;
        // }

        // 上記をリファクタリング。ValueTypeの優劣で並び替え
        if (valueType != ValueType.None)
        {
            // ValueTypeによって、targetListの並び替えで使用する値(ステータス)を変更
            var valueDic = new Dictionary<ValueType, Func<CharaController, int>>  // Func<引数1, 戻り値>。戻り値の型が1つに決まらない場合、IComparableを利用する(ただし、比較可能な型(大小関係や順序比較が意味を持つ型)のみ)
            {
                { ValueType.ByAttackPower, target => target.Status.attackPower },  // valueDic[ValueType.ByAttackPower]した時、target.Status.attackPowerを返す
                { ValueType.ByDefencePower, target => target.Status.defencePower },
                { ValueType.ByCurrentHp, target => target.Status.Hp.Value },
                { ValueType.ByMaxHp, target => target.Status.MaxHp.Value },
                { ValueType.ByCriticalRate, target => target.Status.criticalRate },
            };

            if (valueDic.TryGetValue(valueType, out var sortValue))
            {
                targetList = isDescending
                    ? targetList.OrderByDescending(sortValue).ToList()
                    : targetList.OrderBy(sortValue).ToList();
            }
        }

        // 取得するターゲットの数が指定されている場合
        if (count != -1)
        {
            // // リストの要素数を超えたターゲットの取得をしないように制御(必ず、count <= targetList.Count となる)
            // count = targetList.Count >= count ? count : targetList.Count;

            // if (valueType == ValueType.None)
            // {
            //     // ValueTypeの指定がない場合、ランダムに抽出
            //     targetList = targetList.OrderBy(_ => UnityEngine.Random.value).Take(count).ToList();
            // }
            // else
            // {
            //     // ValueTypeの指定がある場合、並び替えずにTake()して先頭の要素から順番に取得
            //     targetList = targetList.Take(count).ToList();
            // }
            
            if (allowDuplicates)
            {   
                // List<CharaController> targets = new();

                // for (int i = 0; i < count; i++)
                // {
                //     int randomValue = UnityEngine.Random.Range(0, targetList.Count);
                //     targets.Add(targetList[randomValue]);
                // }

                // targetList = targets;
                
                // 上記をリファクタリング
                targetList = Enumerable.Range(0, count)  // for文の代わり。count-1だけループ
                    .Select(_ => targetList[UnityEngine.Random.Range(0, targetList.Count)])
                    .ToList();
            }
            else
            {
                // 上記をリファクタリング
                targetList = targetList
                .OrderBy(_ => valueType == ValueType.None ? UnityEngine.Random.value : 0)  // OrderBy(_ => 固定値)にすると、全ての要素が同じキーを持つため(OrderBy()は、キーの値を比較して要素を並び替える)、元の順序が保持される(安定ソート)
                .Take(Math.Min(count, targetList.Count))  // Math.Minで小さい方を返す(上限をリストサイズに制限)
                .ToList();
            }
        }

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
    /// 攻撃
    /// </summary>
    /// <param name="target"></param>
    /// <param name="baseValue">基準となる値</param>
    /// <param name="rate"></param>
    /// <returns>ダメージ値を返す(総与ダメージを実装する際に使う)</returns>
    public static int Attack(CharaController user, CharaController target, int baseValue, int rate)
    {
        // 「バリア」を持っている場合、一層消費してダメージを無効化
        var barrierBuff = target.Status.Buffs.FirstOrDefault(buff => buff.type == BuffType.バリア);
        if (barrierBuff != null)
        {
            barrierBuff.EffectValue.Value--;
            return 0;
        }

        // baseValueのrate分の値を計算し、攻撃対象のHPを削る
        (int damageValue, bool isCritical) = CalculateManager.CalculateAttackDamage(user, baseValue, rate, target);

        // 「シールド」を持っている場合、ダメージを軽減
        var shieldBuff = target.Status.Buffs.FirstOrDefault(buff => buff.type == BuffType.シールド);
        if (shieldBuff != null)
        {
            var shieldValue = shieldBuff.EffectValue;

            // シールド値を減少、残りのダメージを計算
            shieldValue.Value -= damageValue;
            damageValue = Math.Max(-shieldValue.Value, 0);
        }

        target.UpdateHp(-damageValue);
        target.ReceivedCriticalDamage = isCritical;

        // 「睡眠」状態を解除
        if (target.Status.Buffs.Any(debuff => debuff.type == BuffType.睡眠))
        {
            RemoveBuff(target, BuffType.睡眠);
        }

        return damageValue;
    }

    /// <summary>
    /// 回復
    /// </summary>
    /// <param name="target"></param>
    /// <param name="baseValue"></param>
    /// <param name="rate"></param>
    public static void Heal(CharaController target, int baseValue, int rate)
    {
        // 「不治」状態の場合、HPを回復できない
        if (target.Status.Buffs.Any(debuff => debuff.type == BuffType.不治))  // Any()で、List内に条件に一致する要素があるかどうか判定
        {
            return;
        }

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
    /// <returns>元の値に戻す際に、この値ぶん増加後の値から引く</returns>
    public static int ModifyAttackPower(CharaController target, int baseValue, int rate, bool isIncrease)
    {
        int value = CalculateManager.CalculateSkillEffectValue(baseValue, rate);
        target.Status.attackPower += isIncrease ? +value : -value;

        return value;
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

    /// <summary>
    /// 状態効果(バフ・デバフ)を追加
    /// </summary>
    /// <param name="target"></param>
    /// <param name="buffType"></param>
    /// <param name="isPositiveEffect"></param>
    /// <param name="isIrremovable">解除不可かどうか</param>
    /// <param name="duration">解除不可バフは、デフォルト値で大きな値を設定(値減らさないけど、一応ね)</param>
    /// <param name="effectRate">基準値の?%分の影響を与えるか。「再生」「毒」「侵食」などで使用する</param>
    /// <param name="effectValue">効果の量。「シールド」などで利用</param>
    public static void AddBuff(CharaController target, BuffType buffType, bool isPositiveEffect, bool isIrremovable, int duration = 100, int effectRate = 0, int effectValue = 0)
    {
        // 重ね掛け不可。継続時間とダメージ割合を置き換えて、処理を終了
        var duplicateBuff = target.Status.Buffs.FirstOrDefault(x => x.type == buffType);
        if (duplicateBuff != null)
        {
            duplicateBuff.Duration.Value = duration;
            duplicateBuff.effectRate = effectRate;

            return;
        }

        // デバフを生成して、追加
        var buff = new Buff(buffType, isPositiveEffect, isIrremovable, duration, effectRate, effectValue);
        target.Status.Buffs.Add(buff);

        // 監視処理。継続時間かEffectValueのいずれかが0以下になったら、バフを削除
        Observable.Merge(buff.Duration, buff.EffectValue)
            .Where(x => x <= 0)
            .Subscribe(_ => RemoveBuff(target, buff.type));  // TODO Removeされたときにインスタンスの参照が切れるのでAddTo行わなくても大丈夫？必要な場合、どのように記述すれば良いか？
    }

    /// <summary>
    /// デバフを削除
    /// </summary>
    /// <param name="target"></param>
    /// <param name="buffType"></param>
    public static void RemoveBuff(CharaController target, BuffType buffType)
    {
        target.Status.Buffs.Remove(target.Status.Buffs.FirstOrDefault(debuff => debuff.type == buffType));
    }

    /// <summary>
    /// 指定ターン待機
    /// </summary>
    /// <param name="waitTurn"></param>
    /// <returns></returns>
    public static async UniTask WaitTurnsAsync(int waitTurn)
    {
        int turnCount = battleManager.TurnCount;
        await UniTask.WaitUntil(() => battleManager.TurnCount == turnCount + waitTurn);
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
