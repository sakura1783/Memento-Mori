using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TeamAssemblyPop : MonoBehaviour
{
    /// <summary>
    /// 編成されたキャラの名前とレベル
    /// </summary>
    [System.Serializable]
    public class TeamMemberInfo
    {
        public CharaName name;
        public int level;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        public TeamMemberInfo(CharaName name, int level)
        {
            this.name = name;
            this.level = level;
        }
    }

    public List<TeamMemberInfo> playerTeamInfo;  // プレイヤーチームに編成されたキャラの、最低限の情報群。この情報を使ってバトルの最初で各キャラのステータスを計算する
    public List<TeamMemberInfo> opponentTeamInfo;

    [SerializeField] private Button btnFight;

    [SerializeField] private Transform charactersTran;  // 所持キャラ一覧

    [SerializeField] private Transform[] playerTeamCharaTran = new Transform[5];
    public Transform[] PlayerTeamCharaTran => playerTeamCharaTran;

    [SerializeField] private Transform[] opponentTeamCharaTran = new Transform[5];

    [SerializeField] private Text txtPlayerTeamCombat;
    [SerializeField] private Text txtOpponentTeamCombat;

    [SerializeField] private CharaButton charaButtonPrefab;


    /// <summary>
    /// ポップアップが開かれる時毎回行う処理
    /// </summary>
    public void Setup()
    {
        AssembleOpponentTeam();

        // TODO 保存しておいた前回のチーム編成でキャラのボタンを画面うえに生成
    
        // キャラ一覧にキャラボタンを生成
        foreach (var data in GameData.instance.ownedCharaDataList)
        {
            var charaButton = Instantiate(charaButtonPrefab, charactersTran);
            charaButton.Setup(data, this);
        }

        btnFight.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2f))
            .Subscribe(_ => 
            {
                // TODO ポップアップを閉じて、バトルへ
            });
    }

    /// <summary>
    /// 敵チームのキャラを編成
    /// </summary>
    private void AssembleOpponentTeam()
    {
        // 現在のマップを取得
        var stageData = DataBaseManager.instance.mapDataSO.mapDataList[GameData.instance.clearMapNo + 1].stageDataSO;

        // 現在のステージの敵のリストを取得
        foreach (var enemyData in stageData.stageDataList[GameData.instance.clearStageNo + 1].enemyDataList)
        {
            // 敵チームを編成
            var enemy = new TeamMemberInfo(enemyData.name, enemyData.level);
            opponentTeamInfo.Add(enemy);
        }

        // 画面うえにキャラのボタン(interactableは切る)を生成
        for (int i = 0; i < opponentTeamInfo.Count; i++)
        {
            var prefab = Instantiate(charaButtonPrefab, opponentTeamCharaTran[i], false);
            prefab.Button.interactable = false;
        }
    }

    /// <summary>
    /// CharaButtonを画面うえに生成・破棄
    /// </summary>
    /// <param name="isAssembled"></param>
    /// <param name="charaButton">コピーするCharaButton</param>
    public void SetCharaButton(bool isAssembled, CharaButton charaButton)
    {
        if (isAssembled)
        {
            // CharaButtonを生成
            var generateTran = playerTeamCharaTran.FirstOrDefault(x => x.transform.childCount <= 0).transform;
            charaButton.CopyButton = Instantiate(charaButton.gameObject, generateTran);
            charaButton.IsCopied = true;
        }
        else
        {
            // CharaButtonを破棄
            Destroy(charaButton.CopyButton);
            charaButton.CopyButton = null;

            // CharaButtonの並び替え
            SortCharaButton(Array.FindIndex(playerTeamCharaTran, x => x == charaButton.CopyButton.transform.parent));
        }
    }

    /// <summary>
    /// 画面うえに生成されているCharaButtonの並び替え
    /// </summary>
    /// <param name="removedIndex">破棄されたCharaButtonの要素番号</param>
    public void SortCharaButton(int removedIndex)
    {
        for (int i = removedIndex; i < playerTeamCharaTran.Length; i++)
        {
            // オブジェクトをそれぞれ左に1個ずつずらす
            playerTeamCharaTran[i + 1].GetChild(0).position = playerTeamCharaTran[i].position;
            // 親を再設定
            playerTeamCharaTran[i + 1].GetChild(0).SetParent(playerTeamCharaTran[i]);
        }
    }


    // TODO ポップアップを閉じるタイミングで編成したチームの情報を保存(次回開いた時に使う)
}
