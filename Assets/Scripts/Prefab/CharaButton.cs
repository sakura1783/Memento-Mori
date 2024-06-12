using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// キャラ選択用ボタン(キャラ一覧、キャラ編成画面)
/// </summary>
public class CharaButton : MonoBehaviour
{
    [SerializeField] private Button button;

    [SerializeField] private Image imgChara;
    [SerializeField] private Image imgRank;
    [SerializeField] private Image imgAttribute;

    [SerializeField] private Text txtCharaLevel;

    private GameData.CharaData charaData;

    private TeamAssemblyPop teamAssemblyPop;

    private bool isSelected;


    public void Setup(GameData.CharaData charaData, TeamAssemblyPop teamAssemblyPop)
    {
        this.charaData = charaData;
        this.teamAssemblyPop = teamAssemblyPop;

        // TODO ImageやTextの設定
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
        // TODO 画面うえに追加・削除、一覧のボタンの見た目変更

        if (!isSelected)
        {
            // キャラをチームに追加
            var chara = new TeamAssemblyPop.TeamMemberInfo(charaData.name, charaData.level);
            teamAssemblyPop.playerTeamInfo.Add(chara);

            isSelected = true;
        }
        else
        {
            // キャラをチームから外す
            teamAssemblyPop.playerTeamInfo.RemoveAll(data => data.name == charaData.name);  // RemoveではなくRemoveAllを使えば、ラムダ式を使ってより簡潔に記述できる

            isSelected = false;
        }
    }
}
