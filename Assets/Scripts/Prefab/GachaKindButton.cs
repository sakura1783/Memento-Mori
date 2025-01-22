using System;
using System.Collections.Generic;
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

    private Dictionary<Attribute, (Sprite, Sprite)> attributeSprites = new()  // Item1で水彩画像を、Item2でキャラ画像を取得
    {
        {Attribute.藍, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Blue), SpriteManager.instance.GetCharaSprite(CharaName.Rosevillea, CharaSpriteType.Shoulder))},
        {Attribute.紅, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Orange), SpriteManager.instance.GetCharaSprite(CharaName.Arilosha, CharaSpriteType.Shoulder))},
        {Attribute.翠, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Green), SpriteManager.instance.GetCharaSprite(CharaName.Nina, CharaSpriteType.Shoulder))},
        {Attribute.黄, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Yellow), SpriteManager.instance.GetCharaSprite(CharaName.Elliot, CharaSpriteType.Shoulder))},
    };


    public void Setup(GameData.CurrentGachaDetail gachaData)
    {
        txtKind.text = gachaData.gachaType.ToString();
        if (gachaData.gachaType == GachaType.属性ガチャ)
        {
            txtKind.text = gachaData.attribute.ToString();
            txtKindDetail.text = "属性ガチャ";
        }

        SetAppearanceByGachaType(gachaData.gachaType, gachaData.pickupChara, gachaData.attribute);

        button.OnClickAsObservable()
            .Subscribe(_ =>OnClick())
            .AddTo(this);
    }

    public void OnClick()
    {
        selectGroup.alpha = 1;   // TODO 外す際の処理、どうするか
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
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.DarkPink);
                imgChara.sprite = SpriteManager.instance.GetCharaSprite(pickupChara, CharaSpriteType.Shoulder);
                break;

            case GachaType.プラチナガチャ:
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Sumire);
                //imgChara.sprite = SpriteManager.instance.GetCharaSprite(, CharaSpriteType.Shoulder);  // TODO 
                break;

            case GachaType.属性ガチャ:
                imgColor.sprite = attributeSprites[attribute].Item1;
                imgChara.sprite = attributeSprites[attribute].Item2;
                break;

            case GachaType.運命ガチャ:
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.DarkPurple);
                //imgChara.sprite = SpriteManager.instance.GetCharaSprite()  // TODO ふさわしいキャラの画像を設定
                break;
        }   
    }


    /* 実装すること */
    // AM5:00に切り替え
    // プラチナガチャ、運命ガチャは常設
    // 属性ガチャは24時間で切り替え(ただし、天・冥は除く)
    // ピックアップガチャは1週間毎で切り替え

    // GameDataクラスに、現在開催されているガチャの情報(種類)を保存=> 日経過でガチャの種類を更新、GameDataの情報からゲームを開いた際に一度だけ生成(ポップアップを開いた際に毎回生成、ではない)

    // 属性ガチャのピックアップ順をconstの配列でどこかに宣言する
}
