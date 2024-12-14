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

    private GameData.CharaConstData charaData;

    private TeamAssemblyPop teamAssemblyPop;

    private bool isSelected = false;  // コピーと本体で独立した値を持っているので、一方の値を変えてももう一方の値は変わらない

    private GameObject copyButton;  // 画面うえに生成した、コピーされたCharaButtonのゲームオブジェクト
    public GameObject CopyButton
    {
        get => copyButton;
        set => copyButton = value;
    }

    private bool isCopied;  // コピーされた(画面うえに生成された)ものかどうか
    public bool IsCopied
    {
        get => isCopied;
        set => isCopied = value;
    }


    public void Setup(GameData.CharaConstData charaData, TeamAssemblyPop teamAssemblyPop)
    {
        this.charaData = charaData;
        this.teamAssemblyPop = teamAssemblyPop;

        // TODO 見た目の設定
        imgChara.sprite = SpriteManager.instance.GetCharaSprite(charaData.name, CharaSpriteType.Face);

        button.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.1f))
            .Subscribe(_ =>
            {
                Debug.Log("ボタンが押されました");

                ModifyPlayerTeam();
            });
    }

    /// <summary>
    /// キャラをプレイヤーのチームに追加・削除
    /// </summary>
    public void ModifyPlayerTeam()
    {
        // TODO 本体のボタンの見た目変更

        // すでに選択されているボタン、またはコピーのボタンを押した場合
        if (isSelected || isCopied)
        {
            // キャラをチームから外す
            teamAssemblyPop.playerTeamInfo.RemoveAll(data => data.name == charaData.name);  // RemoveではなくRemoveAllを使えば、ラムダ式を使ってより簡潔に記述できる

            // 画面うえからCharaButtonを破棄
            teamAssemblyPop.SetCharaButton(false, this);

            isSelected = false;

            return;
        }
        else
        {
            // キャラをチームに追加
            var chara = new GameData.CharaConstData(charaData.name, charaData.level);
            teamAssemblyPop.playerTeamInfo.Add(chara);

            // 画面うえにCharaButtonを生成
            teamAssemblyPop.SetCharaButton(true, this);

            isSelected = true;
        }
    }
}
