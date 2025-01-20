using System.Linq;
using UnityEngine;

/// <summary>
/// 画像取得用クラス
/// </summary>
public class SpriteManager : AbstractSingleton<SpriteManager>
{
    public CharaSpriteDataSO charaSpriteDataSO;
    public BuffSpriteDataSO buffSpriteDataSO;
    public WatercolorPaintDataSO watercolorPaintDataSO;


    /// <summary>
    /// キャラの画像を取得
    /// </summary>
    /// <param name="name"></param>
    /// <param name="spriteType"></param>
    /// <returns></returns>
    public Sprite GetCharaSprite(CharaName name, CharaSpriteType spriteType)
    {
        var spriteData = charaSpriteDataSO.charaSpriteDataList.FirstOrDefault(data => data.charaName == name);

        //switch (spriteType)
        //{
        //    case CharaSpriteType.Full:
        //        return spriteData.full;

        //    case CharaSpriteType.Face:
        //        return spriteData.face;

        //    case CharaSpriteType.Shoulder:
        //        return spriteData.shoulder;

        //    default:
        //        Debug.Log("キャラ画像を取得できませんでした");
        //        break;
        //}

        // 上記をリファクタリング
        return spriteType switch
        {
            CharaSpriteType.Full => spriteData.full,
            CharaSpriteType.Face => spriteData.face,
            CharaSpriteType.Shoulder => spriteData.shoulder,

            // switch式は網羅的ではないらしい
            _ => throw new System.ArgumentException(),
        };
    }

    /// <summary>
    /// デバフの画像を取得
    /// </summary>
    /// <param name="buffType"></param>
    /// <returns></returns>
    public Sprite GetDebuffSprite(BuffType buffType)
    {
        return buffSpriteDataSO.buffSpriteDataList.FirstOrDefault(data => data.buffType == buffType).sprite;
    }

    /// <summary>
    /// 水彩ぼかし画像を取得
    /// </summary>
    /// <param name="watercolorPaintType"></param>
    /// <returns></returns>
    public Sprite GetWatercolorPaintSprite(WatercolorPaintType watercolorPaintType)
    {
        return watercolorPaintDataSO.watercolorPaintDataList.FirstOrDefault(data => data.watercolorPaintType == watercolorPaintType).sprite;
    }
}
