using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
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
    [SerializeField] private Image imgWatercolorPaint;

    [SerializeField] private Button btnGachaSpecifics;
    [SerializeField] private Button btn1pull;
    [SerializeField] private Button btn10pull;

    [SerializeField] private Text txtGemCost1x;  // 1回のガチャで必要な宝石の数
    [SerializeField] private Text txtGemCost10x;

    [SerializeField] private Text txtGachaName;
    [SerializeField] private Text txtGachaDetail;

    [SerializeField] private Text txtGemCount;

    [SerializeField] private Sprite platinumGachaImage;
    [SerializeField] private Sprite blueAttributeGachaImage;
    [SerializeField] private Sprite redAttributeGachaImage;
    [SerializeField] private Sprite greenAttributeGachaImage;
    [SerializeField] private Sprite yellowAttributeGachaImage;

    private static readonly Vector2 imgGachaDefaultSize = new(1400, 800);

    private List<GachaKindButton> generatedGachaKindPrefabs = new();
    public List<GachaKindButton> GeneratedGachaKindPrefabs => generatedGachaKindPrefabs;

    private Dictionary<Attribute, (Sprite, Sprite, Sprite)> attributeSprites;  // Item1で水彩画像を、Item2でキャラ画像を、Item3でガチャのメイン画像を取得
    public Dictionary<Attribute, (Sprite, Sprite, Sprite)> AttributeSprites => attributeSprites;


    public override void Setup()
    {
        base.Setup();

        // セットする画像の設定  // TODO readonlyにして、宣言時にまとめて定義したいが、SpriteManagerがインスタンスされていない可能性があるため、Nullエラーになる。他の方法を探す
        attributeSprites = new Dictionary<Attribute, (Sprite, Sprite, Sprite)>
        {
            {Attribute.藍, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Blue), SpriteManager.instance.GetCharaSprite(CharaName.Rosevillea, CharaSpriteType.Shoulder), blueAttributeGachaImage)},
            {Attribute.紅, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Orange), SpriteManager.instance.GetCharaSprite(CharaName.Arilosha, CharaSpriteType.Shoulder), redAttributeGachaImage)},
            {Attribute.翠, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Green), SpriteManager.instance.GetCharaSprite(CharaName.Nina, CharaSpriteType.Shoulder), greenAttributeGachaImage)},
            {Attribute.黄, (SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Yellow), SpriteManager.instance.GetCharaSprite(CharaName.Elliot, CharaSpriteType.Shoulder), yellowAttributeGachaImage)},
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

        // 現在開催されているガチャから、GachaKindButtonを生成
        GameData.instance.currentGachaList.ForEach(gachaData =>
        {
            var gachaKindObj = Instantiate(btnGachaKindPrefab, gachaKindTran);
            gachaKindObj.Setup(gachaData, this);

            generatedGachaKindPrefabs.Add(gachaKindObj);
        });

        // ガチャの最初の要素で画面をセット
        var firstGachaData = GameData.instance.currentGachaList[0];
        SetGachaDetails(firstGachaData.gachaType, firstGachaData.pickupChara, firstGachaData.attribute);

        Observable.Merge
            (
                btn1pull.OnClickAsObservable().Select(_ => true),  // RxのSelectで、ストリームを流れるデータを他のデータに変換できる
                btn10pull.OnClickAsObservable().Select(_ => false)
            )
            .ThrottleFirst(TimeSpan.FromSeconds(1f))
            .Subscribe(isSinglePull => PopupManager.instance.GetPopup<GachaExecutionPop>().ShowPopup(isSinglePull))  // Selectで変換したデータをShowPopup()に渡す
            .AddTo(this);

        // TODO btnGachaSpecifics
    }

    /// <summary>
    /// ガチャ詳細を画面に設定
    /// </summary>
    /// <param name="gachaType"></param>
    /// <param name="pickupChara"></param>
    /// <param name="attribute"></param>
    public void SetGachaDetails(GachaType gachaType, CharaName pickupChara = CharaName.Rosevillea, Attribute attribute = Attribute.藍)
    {
        switch (gachaType)
        {
            case GachaType.ピックアップガチャ:
                SetDetails(SpriteManager.instance.GetCharaSprite(pickupChara, CharaSpriteType.Full), SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.DarkPink), "ピックアップガチャ", $"{DataBaseManager.instance.charaInitialDataSO.charaInitialDataList.FirstOrDefault(data => data.englishName == pickupChara).name}が期間限定で登場", new Vector2(800, 800));
                break;

            case GachaType.プラチナガチャ:
                SetDetails(platinumGachaImage, SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Sumire), "プラチナガチャ", "SRまでのキャラが出現");
                break;

            case GachaType.属性ガチャ:
                SetDetails(attributeSprites[attribute].Item3, attributeSprites[attribute].Item1, $"{attribute}属性ガチャ", $"{attribute}属性のキャラが出現");
                break;

            case GachaType.運命ガチャ:  // TODO imgGachaのサイズ設定
                SetDetails(SpriteManager.instance.GetCharaSprite(pickupChara, CharaSpriteType.Full), SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.DarkPurple), "運命ガチャ", "設定したキャラやアイテムが出現", gachaCost1x:500);
                break;
        }

        // 引数に指定した値を各変数に代入
        void SetDetails(Sprite gachaImage, Sprite waterPaintSprite, string gachaName, string gachaDetail, Vector2 imageSize = default, int gachaCost1x = 300)
        {
            // 値が指定されない場合は(1400, 800)を、指定されている場合は指定値のサイズに設定
            imageSize = imageSize == default ? imgGachaDefaultSize : imageSize;
            imgGacha.rectTransform.sizeDelta = imageSize;

            imgGacha.sprite = gachaImage;
            imgWatercolorPaint.sprite = waterPaintSprite;
            txtGachaName.text = gachaName;
            txtGachaDetail.text = gachaDetail;
            txtGemCost1x.text = gachaCost1x.ToString();
            txtGemCost10x.text = (gachaCost1x * 10).ToString();
        }
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
