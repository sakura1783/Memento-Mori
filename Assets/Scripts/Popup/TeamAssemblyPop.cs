using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.Animations;
using Unity.VisualScripting;

public class TeamAssemblyPop : MonoBehaviour
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

    [SerializeField] private CharaButton charaButtonPrefab;


    // TODO テスト。他の場所に移す
    [SerializeField] private GSSReceiver gssReceiver;

    // テスト
    private async void Start()
    {
        //Debug.Log(CalculateDamage(17000, 370, 11000));

        await gssReceiver.PrepareGSSLoadStartAsync();

        Setup();
    }

    /// <summary>
    /// ポップアップが開かれる時毎回行う処理
    /// </summary>
    public void Setup()
    {
        AssembleOpponentTeam();

        // TODO 保存しておいた前回のチーム編成でキャラのボタンを画面うえに生成
    
        // キャラ一覧にキャラボタンを生成
        // foreach (var data in GameData.instance.ownedCharaDataList)
        // {
        //     var charaButton = Instantiate(charaButtonPrefab, charactersTran);
        //     charaButton.Setup(data, this);
        // }

        // テスト。たくさん生成
        for (int i = 0; i < 10; i++)
        {
            foreach (var data in GameData.instance.ownedCharaDataList)
            {
                var charaButton = Instantiate(charaButtonPrefab, charactersTran);
                charaButton.Setup(data, this);
            }
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
        //var stageData = DataBaseManager.instance.mapDataSO.mapDataList[GameData.instance.clearMapNo + 1].stageDataSO;  // <= 間違い。Listの要素番号は0から始まる
        var stageData = DataBaseManager.instance.mapDataSO.mapDataList.FirstOrDefault(data => data.mapNo == GameData.instance.clearMapNo + 1).stageDataSO;

        // 現在のステージの敵のリストを取得
        //foreach (var enemyData in stageData.stageDataList[GameData.instance.clearStageNo + 1].enemyDataList)  // <= 上記と同じ間違い
        foreach (var enemyData in stageData.stageDataList.FirstOrDefault(data => data.stageNo == GameData.instance.clearStageNo + 1).enemyDataList)
        {
            // 敵チームを編成
            var enemy = new GameData.CharaConstData(enemyData.name, enemyData.level);
            opponentTeamInfo.Add(enemy);
        }

        // 画面うえにキャラのボタン(interactableは切る)を生成
        for (int i = 0; i < opponentTeamInfo.Count; i++)
        {
            var prefab = Instantiate(charaButtonPrefab, opponentTeamCharaTran[i], false);
            prefab.Setup(opponentTeamInfo[i], this);
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
            var generateTran = playerTeamCharaTran.FirstOrDefault(x => x.transform.childCount <= 0);
            charaButton.CopyButton = Instantiate(charaButtonPrefab, generateTran, false);
            charaButton.CopyButton.Setup(charaButton.CharaData, this);
            //charaButton.CopyButton.IsCopied = true;
            charaButton.CopyButton.IsSelected = true;
            charaButton.CopyButton.CopyButton = charaButton.CopyButton;  // TODO ややこしい。どうにかしたい
        }
        else
        {
            SortCharaButton(System.Array.FindIndex(playerTeamCharaTran, x => x == charaButton.CopyButton.transform.parent));

            Destroy(charaButton.CopyButton.gameObject);
            charaButton.CopyButton = null;
            charaButton.IsSelected = false;
        }
    }

    /// <summary>
    /// 画面うえのCharaButtonを並び替え
    /// </summary>
    /// <param name="removedIndex">破棄されたCharaButtonの要素番号</param>
    public void SortCharaButton(int removedIndex)
    {
        // オブジェクトをそれぞれ左に1個ずつずらす
        for (int i = removedIndex; i < playerTeamCharaTran.Length; i++)
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
