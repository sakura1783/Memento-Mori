using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// キャラ選択用ボタン(キャラ一覧、キャラ編成画面)
/// </summary>
public class CharaButton : MonoBehaviour
{
    [SerializeField] private Button button;
    public Button Button
    {
        get => button;
        set => button = value;
    }

    [SerializeField] private Image imgChara;
    [SerializeField] private Image imgRank;
    [SerializeField] private Image imgAttribute;

    [SerializeField] private Text txtCharaLevel;

    private GameData.OwnedCharaData charaData;

    private TeamAssemblyPop teamAssemblyPop;

    private bool isSelected;

    private CharaButton copyButton;  // 画面うえに生成した、コピーされたボタン。本体かコピーいずれかを押した時、処理が紐付くようにする
    public CharaButton CopyButton
    {
        get => copyButton;
        set => copyButton = value;
    }


    public void Setup(GameData.OwnedCharaData charaData, TeamAssemblyPop teamAssemblyPop)
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

        // TODO isSelectedの値でコピーボタンの処理を追加、本体ボタンの挙動をコピーボタンと紐付け
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

            // 画面うえにCharaButtonを生成
            teamAssemblyPop.SetCharaButton(true, this);

            isSelected = true;
        }
        else
        {
            // キャラをチームから外す
            teamAssemblyPop.playerTeamInfo.RemoveAll(data => data.name == charaData.name);  // RemoveではなくRemoveAllを使えば、ラムダ式を使ってより簡潔に記述できる

            // 画面うえからCharaButtonを破棄
            teamAssemblyPop.SetCharaButton(false, this);

            isSelected = false;
        }
    }
}
