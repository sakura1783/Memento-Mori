using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

    private List<GameObject> generatedGachaRarities = new();

    
    /// <summary>
    /// ポップアップを表示
    /// </summary>
    /// <param name="is1pull"></param>
    public async void ShowPopup(bool is1pull)
    {
        // 前回生成したオブジェクトを破棄。初期化用のInitialize()メソッド作る？

        buttonGroup.alpha = 0;
        buttonGroup.blocksRaycasts = false;

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

        await ExecuteGacha(is1pull);

        buttonGroup.alpha = 1;
        buttonGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// ガチャ実行
    /// </summary>
    /// <param name="is1pull"></param>
    /// <returns></returns>
    public async UniTask ExecuteGacha(bool is1pull)
    {
        var gachaCount = is1pull ? 1 : 10;

        List<GameData.CharaConstData> resultCharaData = new();

        // GachaRarityプレハブの生成
        for (int i = 0; i < gachaCount; i++)
        {
            // 全キャラからランダムに取得
            var charaData = DataBaseManager.instance.charaInitialDataSO.charaInitialDataList[UnityEngine.Random.Range(0, DataBaseManager.instance.charaInitialDataSO.charaInitialDataList.Count)];
            resultCharaData.Add(new (charaData.englishName, 1));

            var rarityObj = Instantiate(gachaRarityPrefab, resultTran);
            generatedGachaRarities.Add(rarityObj.gameObject);
        }

        // CharaButtonプレハブの生成と、初期設定
        for (int i = 0; i < generatedGachaRarities.Count; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            var resultChara = Instantiate(charaButtonPrefab, generatedGachaRarities[i].transform);
            resultChara.Setup(resultCharaData[i]);
            resultChara.Button.interactable = false;

            Debug.Log("待つ");
        }

        buttonGroup.alpha = 1;
        buttonGroup.blocksRaycasts = true;
    }

    /* TODO 実装 */
    // 各キャラの排出確率(レアリティ、ピックアップ時などを考慮)
    // 各ガチャで排出されるキャラ

    // 必要な値をConstDataクラスに記述
    // 上記を考慮した処理の実装

    // CharaConstDataにレアリティの情報を追加？このクラス、CharaButton等で使用したい
    // レアリティはレベルに依存しない。所持キャラのレアリティをどこで保存するか。やはり上記クラスに追加するか？
}
