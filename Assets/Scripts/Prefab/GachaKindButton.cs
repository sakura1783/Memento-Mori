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


    public void Setup()
    {
        GameData.instance.currentGachaList.ForEach(gacha =>
        {
            txtKind.text = gacha.gachaType.ToString();

            // 属性ガチャの場合
            if (gacha.gachaType == GachaType.藍 || gacha.gachaType == GachaType.紅 || gacha.gachaType == GachaType.翠 || gacha.gachaType == GachaType.黄) txtKindDetail.text = "属性ガチャ";

            SetAppearanceByGachaType(gacha.gachaType, gacha.pickupChara);
        });

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
    /// <param name="pickupChara">ピックアップガチャでのみ使用する</param>
    private void SetAppearanceByGachaType(GachaType gachaType, CharaName pickupChara = CharaName.Rosevillea)
    {
        switch (gachaType)
        {
            case GachaType.ピックアップガチャ:
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.DarkPink);
                imgChara.sprite = SpriteManager.instance.GetCharaSprite(pickupChara, CharaSpriteType.Shoulder);
                break;

            case GachaType.プラチナガチャ:
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Sumire);
                imgChara.sprite = SpriteManager.instance.GetCharaSprite(CharaName.Rosevillea, CharaSpriteType.Shoulder);
                break;

            case GachaType.運命ガチャ:
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.DarkPurple);
                //imgChara.sprite = SpriteManager.instance.GetCharaSprite()  // TODO ふさわしいキャラの画像を設定
                break;

            case GachaType.藍:
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Blue);
                imgChara.sprite = SpriteManager.instance.GetCharaSprite(CharaName.Setsuna, CharaSpriteType.Shoulder);
                break;

            case GachaType.紅:
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Orange);
                imgChara.sprite = SpriteManager.instance.GetCharaSprite(CharaName.Arilosha, CharaSpriteType.Shoulder);
                break;

            case GachaType.翠:
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Green);
                imgChara.sprite = SpriteManager.instance.GetCharaSprite(CharaName.Nina, CharaSpriteType.Shoulder);
                break;

            case GachaType.黄:
                imgColor.sprite = SpriteManager.instance.GetWatercolorPaintSprite(WatercolorPaintType.Yellow);
                imgChara.sprite = SpriteManager.instance.GetCharaSprite(CharaName.Elliot, CharaSpriteType.Shoulder);
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
