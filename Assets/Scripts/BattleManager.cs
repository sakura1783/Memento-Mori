using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    /// <summary>
    /// 編成されたキャラのデータ
    /// </summary>
    [System.Serializable]
    public class TeamCharaData
    {
        public CharaName name;
        public int level;

        ///// <summary>
        ///// 単一のキャラデータをコピーするコンストラクタ
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="level"></param>
        //public TeamCharaData(CharaName name, int level)
        //{
        //    this.name = name;
        //    this.level = level;
        //}

        ///// <summary>
        ///// 引数の情報から新しいインスタンスを生成するコンストラクタ
        ///// </summary>
        ///// <param name="teamData">編成されたキャラのデータ群</param>
        ///// <param name="targetTeam">プレイヤーと敵、どちらのチームに編成するか</param>
        //public TeamCharaData(List<TeamCharaData> teamData, List<TeamCharaData> targetTeam)
        //{
        //    foreach (var data in teamData)
        //    {
        //        targetTeam.Add(new TeamCharaData(data.name, data.level));
        //    }
        //}
    }

    private List<TeamCharaData> playerTeam = new();
    private List<TeamCharaData> opponentTeam = new();


    /// <summary>
    /// チームを編成する
    /// </summary>
    /// <param name="teamData"></param>
    /// <param name="isPlayerTeam"></param>
    //public void AssembleTeam(List<TeamCharaData> teamData, bool isPlayerTeam)
    //{
    //    var targetTeam = isPlayerTeam ? playerTeam : opponentTeam;

    //    new TeamCharaData(teamData, targetTeam);
    //}

    // TODO チームの情報から、初期のデータを計算してバトルで使う。バトルが終わったらその情報は破棄する。
}
