using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キャラ一覧、キャラ編成画面で使うキャラ選択用ボタン
/// </summary>
public class CharaButton : MonoBehaviour
{
    [SerializeField] private Button button;

    [SerializeField] private Image imgChara;
    [SerializeField] private Image imgRank;
    [SerializeField] private Image imgAttribute;

    [SerializeField] private Text txtCharaLevel;

    private GameData.OwnedCharaData charaData;

    private bool isSelected;


    public void Setup(GameData.OwnedCharaData data)
    {
        charaData = data;

        // TODO ImageやTextの設定
    }

    /// <summary>
    /// キャラをチームに編成する
    /// </summary>
    public void AddCharaToTeam(CharaName charaName, int charaLevel)
    {
        if (!isSelected)
        {
            // TODO キャラをチームに編成

            isSelected = true;
        }
        else
        {
            // TODO キャラをチームから外す

            isSelected = false;
        }
    }
}
