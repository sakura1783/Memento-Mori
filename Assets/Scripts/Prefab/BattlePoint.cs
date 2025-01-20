using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 該当のマップ・ステージとの紐付け役
/// </summary>
public class BattlePoint : MonoBehaviour
{
    [SerializeField] private int mapNo;
    [SerializeField] private int stageNo;

    [SerializeField] private Image image;
    public Image Image
    {
        get => image;
        set => image = value;
    }

    
    /// <summary>
    /// ボタンを押した際の処理
    /// </summary>
    public void OnClick()
    {
        Debug.Log("押されました");
        
        var teamAssemblyPop = PopupManager.instance.GetPopup<TeamAssemblyPop>();
        teamAssemblyPop.ShowPopup(mapNo, stageNo);
    }
}
