using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ガチャの種類
/// </summary>
public enum GachaType
{
    ピックアップガチャ,
    プラチナガチャ,
    属性ガチャ,
    運命ガチャ,
}

public class GachaPop : PopupBase
{
    [SerializeField] private Transform gachaKindTran;

    [SerializeField] private GachaKindButton btnGachaKindPrefab;

    [SerializeField] private Image imgGacha;

    [SerializeField] private Text txtGemCount;


    public override void Setup()
    {
        // TODO テスト。終わったら消す
        var gachaDatas = new List<GameData.CurrentGachaDetail>
        {
            new(GachaType.属性ガチャ, attribute:Attribute.黄),
            new(GachaType.プラチナガチャ),
            new(GachaType.運命ガチャ),
            new(GachaType.ピックアップガチャ, CharaName.Arilosha),
            new(GachaType.ピックアップガチャ, CharaName.Setsuna),
        };

        //gachaDatas.OrderByDescending(x => x.gachaType == GachaType.ピックアップガチャ).  // TODO ピックアップ→ プラチナ→ 属性→ 運命 で並び替えたい
        

        base.Setup();

        // 現在開催されているガチャから、必要なオブジェクトを生成
        GameData.instance.currentGachaList.ForEach(gachaData =>
        {
            var gachaKindObj = Instantiate(btnGachaKindPrefab, gachaKindTran);
            gachaKindObj.Setup(gachaData);

            // TODO 他の値の設定
        });
    }
}
