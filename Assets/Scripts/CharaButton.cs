using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

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

    private BattleManager battleManager;

    private bool isSelected;


    public void Setup(GameData.OwnedCharaData charaData, BattleManager battleManager)
    {
        this.charaData = charaData;
        this.battleManager = battleManager;

        // TODO ImageやTextの設定、Observerでの監視処理
        button.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ =>
            {
                ModifyPlayerTeam();
            });
    }

    /// <summary>
    /// キャラをプレイヤーのチームに追加・削除
    /// </summary>
    public void ModifyPlayerTeam()
    {
        // TODO 画面上に追加・削除、一覧のボタンの見た目変更

        if (!isSelected)
        {
            // キャラをチームに追加
            var chara = new BattleManager.TeamCharaData(charaData.name, charaData.level);
            battleManager.playerTeam.Add(chara);

            isSelected = true;
        }
        else
        {
            // キャラをチームから外す
            battleManager.playerTeam.RemoveAll(data => data.name == charaData.name);  // RemoveではなくRemoveAllを使えば、ラムダ式を使ってより簡潔に記述できる

            isSelected = false;
        }
    }
}
