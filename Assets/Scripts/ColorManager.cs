using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ColorType
{
    Rarity_R,
    Rarity_SR,
    Rarity_SSR,
    Rarity_UR,
    Rarity_LR,
}

public class ColorManager : AbstractSingleton<ColorManager>
{
    [SerializeField] private ColorDataSO colorDataSO;

    private static readonly Dictionary<Rarity, ColorType> rarityColorDic = new()
    {
        {Rarity.R, ColorType.Rarity_R},
        {Rarity.SR, ColorType.Rarity_SR},
        {Rarity.SSR, ColorType.Rarity_SSR},
        {Rarity.UR, ColorType.Rarity_UR},
        {Rarity.LR, ColorType.Rarity_LR},
    };


    /// <summary>
    /// 色を取得
    /// </summary>
    /// <param name="colorType">取得したい色</param>
    /// <returns></returns>
    public Color GetColor(ColorType colorType)
    {
        var hexColor = colorDataSO.colorDataList.FirstOrDefault(data => data.colorType == colorType).hexadecimalColor;
        ColorUtility.TryParseHtmlString(hexColor, out Color color);

        return color;
    }

    /// <summary>
    /// キャラのレアリティから色を取得
    /// </summary>
    /// <param name="charaRarity"></param>
    /// <returns></returns>
    public Color GetColorByRarity(Rarity charaRarity)
    {
        return GetColor(rarityColorDic[charaRarity]);
    }
}
