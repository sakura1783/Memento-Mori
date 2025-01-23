using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GachaKindButton : MonoBehaviour
{
    [SerializeField] private Button button;

    [SerializeField] private Text txtKind;
    [SerializeField] private Text txtKindDetail;  // 必要でないものもある

    [SerializeField] private Image imgChara;
    [SerializeField] private Image imgColor;

    [SerializeField] private CanvasGroup selectGroup;

    private GachaPop gachaPop;

    private Dictionary<Attribute, (Sprite, Sprite, Sprite)> attributeSprites;


    public void Setup(GameData.CurrentGachaDetail gachaData, GachaPop gachaPop)
    {
        this.gachaPop = gachaPop;
        attributeSprites = gachaPop.AttributeSprites;

        selectGroup.alpha = 0;

        SetAppearanceByGachaType(gachaData.gachaType, gachaData.pickupChara, gachaData.attribute);

        button.OnClickAsObservable()
            .Subscribe(_ =>OnClick(gachaData, gachaPop.GeneratedGachaKindPrefabs))
            .AddTo(this);
    }

    /// <summary>
    /// タップした際の処理
    /// </summary>
    /// <param name="gachaData"></param>
    /// <param name="gachaKindPrefabs">GachaPopで生成したGachaKindButtonプレハブ群</param>
    public void OnClick(GameData.CurrentGachaDetail gachaData, List<GachaKindButton> gachaKindPrefabs)
    {
        // GachaPopの画面設定
        gachaPop.SetGachaDetails(gachaData.gachaType, gachaData.pickupChara, gachaData.attribute);

        selectGroup.alpha = 1;
        
        // このオブジェクト以外のselectGroupを非表示にする
        List<GachaKindButton> others = gachaKindPrefabs.Where(prefab => prefab != this).ToList();
        others.ForEach(obj => obj.selectGroup.alpha = 0);
    }

    /// <summary>
    /// ガチャの種類を元に見た目を設定
    /// </summary>
    /// <param name="gachaType"></param>
    /// <param name="pickupChara">ピックアップガチャでのみ使用</param>
    /// <param name="attribute">属性ガチャでのみ使用</param>
    private void SetAppearanceByGachaType(GachaType gachaType, CharaName pickupChara = CharaName.Rosevillea, Attribute attribute = Attribute.藍)
    {
        switch (gachaType)
        {
            case GachaType.ピックアップガチャ:
                SetTextProperties("ピックアップ\nガチャ", 35);
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.DarkPink);
                imgChara.sprite = SpriteManager.instance.GetCharaSprite(pickupChara, CharaSpriteType.Shoulder);
                break;

            case GachaType.プラチナガチャ:
                SetTextProperties("プラチナ\nガチャ", 40);
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Sumire);
                //imgChara.sprite = SpriteManager.instance.GetCharaSprite(, CharaSpriteType.Shoulder);  // TODO 
                break;

            case GachaType.属性ガチャ:
                SetTextProperties(attribute.ToString(), 48, gachaType.ToString(), 27);
                imgColor.sprite = attributeSprites[attribute].Item1;
                imgChara.sprite = attributeSprites[attribute].Item2;
                break;

            case GachaType.運命ガチャ:
                txtKind.text = "運命\nガチャ";
                SetTextProperties("運命", 48, "ガチャ", 35);
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.DarkPurple);
                //imgChara.sprite = SpriteManager.instance.GetCharaSprite()  // TODO ふさわしいキャラの画像を設定
                break;
        }

        // UIのテキスト設定
        void SetTextProperties(string text1, int size1, string text2 = "", int size2 = 0)
        {
            txtKind.text = text1;
            txtKindDetail.text = text2;

            txtKind.fontSize = size1;
            txtKindDetail.fontSize = size2;
        }
    }


    /* 実装すること */
    // AM5:00に切り替え
    // プラチナガチャ、運命ガチャは常設
    // 属性ガチャは24時間で切り替え(ただし、天・冥は除く)。属性ガチャのピックアップ順をconstの配列でどこかに宣言する
    // ピックアップガチャは1週間毎で切り替え

    // GameDataクラスに、現在開催されているガチャの情報(種類)を保存=> 日経過でガチャの種類を更新、GameDataの情報からゲームを開いた際に一度だけ生成(ポップアップを開いた際に毎回生成、ではない)
}
