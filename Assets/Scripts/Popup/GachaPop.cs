using System;
using System.Collections.Generic;
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

    private Dictionary<Attribute, (Sprite, Sprite)> attributeSprites;


    public override void Setup()
    {
        base.Setup();

        // セットする画像の設定  // TODO readonlyにして、宣言時にまとめて定義したいが、SpriteManagerがインスタンスされていない可能性があるため、Nullエラーになる。他の方法を探す
        attributeSprites = new Dictionary<Attribute, (Sprite, Sprite)>  // Item1で水彩画像を、Item2でキャラ画像を取得
        {
            {Attribute.藍, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Blue), SpriteManager.instance.GetCharaSprite(CharaName.Rosevillea, CharaSpriteType.Shoulder))},
            {Attribute.紅, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Orange), SpriteManager.instance.GetCharaSprite(CharaName.Arilosha, CharaSpriteType.Shoulder))},
            {Attribute.翠, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Green), SpriteManager.instance.GetCharaSprite(CharaName.Nina, CharaSpriteType.Shoulder))},
            {Attribute.黄, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Yellow), SpriteManager.instance.GetCharaSprite(CharaName.Elliot, CharaSpriteType.Shoulder))},
        };

        // TODO テスト。終わったら消す
        var gachaDatas = new List<GameData.CurrentGachaDetail>
        {
            new(GachaType.属性ガチャ, attribute:Attribute.黄),
            new(GachaType.プラチナガチャ),
            new(GachaType.運命ガチャ),
            new(GachaType.ピックアップガチャ, CharaName.Arilosha),
            new(GachaType.ピックアップガチャ, CharaName.Setsuna),
        };
        GameData.instance.currentGachaList.AddRange(gachaDatas);
        
        // GachaSorterクラスのcustomOrderでリストを並び替え
        GameData.instance.currentGachaList.Sort(new GachaSorter());

        // 現在開催されているガチャから、必要なオブジェクトを生成
        GameData.instance.currentGachaList.ForEach(gachaData =>
        {
            var gachaKindObj = Instantiate(btnGachaKindPrefab, gachaKindTran);
            gachaKindObj.Setup(gachaData, attributeSprites);

            // TODO 他の値の設定
        });
    }
}

/// <summary>
/// 開催中のガチャの並び替え用クラス
/// </summary>
public class GachaSorter : IComparer<GameData.CurrentGachaDetail>
{
    private static readonly GachaType[] customOrder = new GachaType[]
    {
        GachaType.ピックアップガチャ,
        GachaType.プラチナガチャ,
        GachaType.属性ガチャ,
        GachaType.運命ガチャ,
    };


    /// <summary>
    /// customOrder配列のインデックス番号で値同士を比較
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(GameData.CurrentGachaDetail x, GameData.CurrentGachaDetail y)
    {
        // CompareTo()で2つの値を比較し、代償関係を返す。(x < yで負、x == yで0、x > yで正の値を返す)
        return Array.IndexOf(customOrder, x.gachaType).CompareTo(Array.IndexOf(customOrder, y.gachaType));
    }
}
