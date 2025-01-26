using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GachaExecutionPop : PopupBase
{
    [SerializeField] private Transform resultTran;
    
    [SerializeField] private Button btnPull;

    [SerializeField] private CanvasGroup buttonGroup;

    [SerializeField] private Text txtPull;
    [SerializeField] private Text txtPrice;

    [SerializeField] private Image gachaRarityPrefab;

    [SerializeField] private CharaButton charaButtonPrefab;

    private List<Image> generatedGachaRarities = new();

    private bool isSinglePull;
    private bool isDestinyGacha;


    public override void Setup()
    {
        base.Setup();

        btnPull.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(1f))
            .Subscribe(_ => ShowPopup(isSinglePull, isDestinyGacha))
            .AddTo(this);
    }

    /// <summary>
    /// ポップアップを表示
    /// </summary>
    /// <param name="isSinglePull"></param>
    public void ShowPopup(bool isSinglePull, bool isDestinyGacha)
    {
        Initialize();

        this.isSinglePull = isSinglePull;
        this.isDestinyGacha = isDestinyGacha;

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

        txtPull.text = isSinglePull ? "1回ガチャ" : "10回ガチャ";
        
        if (isDestinyGacha) txtPrice.text = isSinglePull ? "500" : "5000";
        else txtPrice.text = isSinglePull ? "300" : "3000";

        ExecuteGacha(isSinglePull);
    }

    /// <summary>
    /// ガチャ実行
    /// </summary>
    /// <param name="is1pull"></param>
    /// <returns></returns>
    public async void ExecuteGacha(bool isSinglePull)
    {
        var gachaCount = isSinglePull ? 1 : 10;

        List<GameData.CharaConstData> resultCharaData = new();

        // GachaRarityプレハブの生成
        for (int i = 0; i < gachaCount; i++)
        {
            // 全キャラからランダムに取得
            var charaData = DataBaseManager.instance.charaInitialDataSO.charaInitialDataList[UnityEngine.Random.Range(0, DataBaseManager.instance.charaInitialDataSO.charaInitialDataList.Count)];
            resultCharaData.Add(new (charaData.englishName, 1));

            var rarityObj = Instantiate(gachaRarityPrefab, resultTran);
            generatedGachaRarities.Add(rarityObj);
        }

        // CharaButtonプレハブの生成と、初期設定
        for (int i = 0; i < generatedGachaRarities.Count; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            // GachaRarityを透明にする
            var color = generatedGachaRarities[i].color;
            generatedGachaRarities[i].color = new Color(color.r, color.g, color.b, 0);
            
            var resultChara = Instantiate(charaButtonPrefab, generatedGachaRarities[i].transform);
            resultChara.Setup(resultCharaData[i]);
            resultChara.Button.interactable = false;
            resultChara.transform.localScale = new Vector2(1.5f, 1.5f);
        }

        buttonGroup.alpha = 1;
        buttonGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// ポップアップの初期化
    /// </summary>
    private void Initialize()
    {
        // 前回生成したオブジェクトを破棄
        for (int i = 0; i < generatedGachaRarities.Count; i++) Destroy(generatedGachaRarities[i].gameObject);
        generatedGachaRarities.Clear();

        buttonGroup.alpha = 0;
        buttonGroup.blocksRaycasts = false;
    }


    /* TODO 実装 */
    // 各キャラの排出確率(レアリティ、ピックアップ時などを考慮)
    // 各ガチャで排出されるキャラ

    // 必要な値をConstDataクラスに記述(排出確率等)
    // 上記を考慮した処理の実装

    // CharaConstDataにレアリティの情報を追加？このクラス、CharaButton等で使用したい
    // レアリティはレベルに依存しない。所持キャラのレアリティをどこで保存するか。やはり上記クラスに追加するか？

    // ジェムを必要数持っているか・回した後にジェムを減らす
    // GameDataクラス、所持キャラリストへの追加
}
