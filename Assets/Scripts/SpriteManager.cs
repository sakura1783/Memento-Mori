using System.Linq;
using UnityEngine;



/// <summary>
/// 画像取得用クラス
/// </summary>
public class SpriteManager : AbstractSingleton<SpriteManager>
{
    public CharaSpriteDataSO charaSpriteDataSO;


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
}
