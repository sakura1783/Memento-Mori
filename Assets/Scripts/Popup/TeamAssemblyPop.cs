using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TeamAssemblyPop : PopupBase
{
    // TODO privateにしてプロパティに変更
    public List<GameData.CharaConstData> playerTeamInfo;  // プレイヤーチームに編成されたキャラの、最低限の情報群。この情報を使ってバトルの最初で各キャラのステータスを計算する
    public List<GameData.CharaConstData> opponentTeamInfo;

    [SerializeField] private Button btnFight;

    [SerializeField] private Transform charactersTran;  // 所持キャラ一覧

    [SerializeField] private Transform[] playerTeamCharaTran = new Transform[5];
    public Transform[] PlayerTeamCharaTran => playerTeamCharaTran;

    [SerializeField] private Transform[] opponentTeamCharaTran = new Transform[5];

    [SerializeField] private Text txtPlayerTeamCombat;
    [SerializeField] private Text txtOpponentTeamCombat;

    [SerializeField] private CharaButton charaButtonPrefab;  // Characters下に生成するキャラのボタン

    [SerializeField] private CopyButton copyButtonPrefab;  // 画面うえに生成するキャラのボタン


    public override void Setup()
    {
        base.Setup();

        btnFight.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2f))
            .Subscribe(_ => PopupManager.instance.Show<BattleManager>(true))
            .AddTo(this);
    }

    /// <summary>
    /// ポップアップが開かれる時毎回行う処理
    /// </summary>
    public void ShowPopup(int mapNo, int stageNo)
    {
        AssembleOpponentTeam(mapNo, stageNo);

        // TODO 保存しておいた前回のチーム編成でキャラのボタンを画面うえに生成
    
        // キャラ一覧にキャラボタンを生成
        foreach (var data in GameData.instance.ownedCharaDataList)
        {
            var charaButton = Instantiate(charaButtonPrefab, charactersTran);
            charaButton.Setup(data);
            
            charaButton.Button.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => ModifyPlayerTeam(charaButton))
                .AddTo(charaButton);
        }

        // テスト。たくさん生成
        // for (int i = 0; i < 10; i++)
        // {
        //     foreach (var data in GameData.instance.ownedCharaDataList)
        //     {
        //         var charaButton = Instantiate(charaButtonPrefab, charactersTran);
        //         charaButton.Setup(data, this);
        //     }
        // }

        PopupManager.instance.PreviousPop = this;

        base.ShowPopup();
    }

    /// <summary>
    /// 敵チームのキャラを編成
    /// </summary>
    /// <param name="selectedMapNo">プレイヤーが選択したマップ</param>
    /// <param name="selectedStageNo">プレイヤーが選択したステージ</param>
    private void AssembleOpponentTeam(int selectedMapNo, int selectedStageNo)
    {
        // 選択されたマップを取得
        //var stageData = DataBaseManager.instance.mapDataSO.mapDataList[GameData.instance.clearMapNo + 1].stageDataSO;  // <= 間違い。Listの要素番号は0から始まる
        var stageData = DataBaseManager.instance.mapDataSO.mapDataList.FirstOrDefault(data => data.mapNo == selectedMapNo).stageDataSO;

        // 選択されたステージの敵のリストを取得
        //foreach (var enemyData in stageData.stageDataList[GameData.instance.clearStageNo + 1].enemyDataList)  // <= 上記と同じ間違い
        foreach (var enemyData in stageData.stageDataList.FirstOrDefault(data => data.stageNo == selectedStageNo).enemyDataList)
        {
            // 敵チームを編成
            var enemy = new GameData.CharaConstData(enemyData.name, enemyData.level, enemyData.rarity);
            opponentTeamInfo.Add(enemy);
        }

        // 画面うえにキャラのボタン(interactableは切る)を生成
        for (int i = 0; i < opponentTeamInfo.Count; i++)
        {
            var charaButton = Instantiate(charaButtonPrefab, opponentTeamCharaTran[i], false);
            charaButton.Setup(opponentTeamInfo[i]);
            charaButton.Button.interactable = false;
        }
    }
    
    /// <summary>
    /// キャラをプレイヤーのチームに追加・削除
    /// </summary>
    /// <param name="pushedButton"></param>
    private void ModifyPlayerTeam(CharaButton pushedButton)
    {
        // すでに選択されているボタン、またはコピーのボタンを押した場合
        if (pushedButton.IsSelected.Value) //|| isCopied)
        {
            // キャラをチームから外す
            playerTeamInfo.RemoveAll(data => data.name == pushedButton.CharaData.name);  // RemoveではなくRemoveAllを使えば、ラムダ式を使ってより簡潔に記述できる

            // 画面うえからCopyButtonを破棄
            SetCopyButton(false, pushedButton);

            pushedButton.IsSelected.Value = false;

            return;
        }
        else
        {
            // チームがすでに満員の場合、処理しない
            if (IsTeamAtMaxCapacity()) return;

            // キャラをチームに追加
            var chara = new GameData.CharaConstData(pushedButton.CharaData.name, pushedButton.CharaData.level, pushedButton.CharaData.rarity);
            playerTeamInfo.Add(chara);

            // 画面うえにCopyButtonを生成
            SetCopyButton(true, pushedButton);

            pushedButton.IsSelected.Value = true;
        }
    }

    /// <summary>
    /// CopyButtonを画面うえに生成・破棄
    /// </summary>
    /// <param name="isAssembled"></param>
    /// <param name="charaButton">コピーするCharaButton</param>
    public void SetCopyButton(bool isAssembled, CharaButton charaButton)
    {
        if (isAssembled)
        {
            // CopyButtonを生成
            var generateTran = playerTeamCharaTran.FirstOrDefault(x => x.transform.childCount <= 0);
            charaButton.CopyButton = Instantiate(copyButtonPrefab, generateTran);
            charaButton.CopyButton.Setup(charaButton, this);
        }
        else
        {
            charaButton.CopyButton.RemoveCopyButton();
        }
    }

    /// <summary>
    /// 画面うえのCharaButtonを並び替え
    /// </summary>
    /// <param name="removedIndex">破棄されたCharaButtonの要素番号</param>
    public void SortCharaButton(int removedIndex)
    {
        // オブジェクトをそれぞれ左に1個ずつずらす
        for (int i = removedIndex; i < playerTeamCharaTran.Length - 1; i++)  // <= 繰り返しの条件として、6番目の要素を参照するとIndexOutOfRangeエラーになるので配列-1を指定。
        {
            if (playerTeamCharaTran[i + 1].childCount <= 0)
            {
                // CharaTranに子(CharaButton)が存在しない場合、処理しない
                return;
            }

            // 親を再設定
            playerTeamCharaTran[i + 1].GetChild(0).SetParent(playerTeamCharaTran[i]);

            // positionを再設定(新しい親の位置にずらす)
            if (i == removedIndex)
            {
                // 最初の要素の場合、CharaTran[i]には破壊する予定のCharaButtonがまだいるので、2番目の子を参照する
                playerTeamCharaTran[i].GetChild(1).localPosition = Vector2.zero;
            }
            else
            {
                playerTeamCharaTran[i].GetChild(0).localPosition = Vector2.zero;
            }
        }
    }

    /// <summary>
    /// チームが満員かどうかを調べる。trueで満員
    /// </summary>
    /// <returns></returns>
    public bool IsTeamAtMaxCapacity()
    {
        return !playerTeamCharaTran.FirstOrDefault(x => x.transform.childCount <= 0);
    }


    // TODO ポップアップを閉じるタイミングで編成したチームの情報を保存(次回開いた時に使う)
}
