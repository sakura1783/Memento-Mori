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
    /// <param name="count"></param>
    /// <returns></returns>
    public static List<CharaController> PickTarget(CharaController user, TargetType targetType, int count)
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

            // TODO 追加：攻撃力などの値が上位?人など
        }

        return targetList;  // TODO 今はエラーが出るのでとりあえず。
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

    // 攻撃力、Hp、クリティカル率等を処理するメソッド
    // まとめるか、それとも各値ごとに分けた方がやりやすいか。

    // レベルアップ後の追加処理は各キャラスクリプト内に記述する
}
