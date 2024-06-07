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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        public TeamCharaData(CharaName name, int level)
        {
            this.name = name;
            this.level = level;
        }
    }

    public List<TeamCharaData> playerTeam = new();
    public List<TeamCharaData> opponentTeam = new();


    // TODO チームの情報から、初期のデータを計算してバトルで使う。バトルが終わったらその情報は破棄する。

    /// <summary>
    /// バトルの準備
    /// </summary>
    private void PrepareBattle()
    {

    }
}
