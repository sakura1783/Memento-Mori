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

    [SerializeField] private CanvasGroup selectedSet;

    private GameData.CharaConstData charaData;
    public GameData.CharaConstData CharaData
    {
        get => charaData;
        set => charaData = value;
    }

    private TeamAssemblyPop teamAssemblyPop;

    public ReactiveProperty<bool> IsSelected = new();

    private CopyButton copyButton;  // 画面うえに生成した、コピーされたCharaButtonのゲームオブジェクト
    public CopyButton CopyButton
    {
        get => copyButton;
        set => copyButton = value;
    }


    public void Setup(GameData.CharaConstData charaData, TeamAssemblyPop teamAssemblyPop = null)
    {
        this.charaData = charaData;
        this.teamAssemblyPop = teamAssemblyPop;

        imgChara.sprite = SpriteManager.instance.GetCharaSprite(charaData.name, CharaSpriteType.Face);
        imgRank.color = ColorManager.instance.GetColorByRarity(charaData.rarity);
        // TODO imgAttribute
        txtCharaLevel.text = $"Lv{charaData.level}";

        if (!teamAssemblyPop)
        {
            return;
        }

        button.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.1f))
            .Subscribe(_ => ModifyPlayerTeam())
            .AddTo(this);
        
        IsSelected
            .Subscribe(IsSelected => selectedSet.alpha = IsSelected ? 1 : 0)
            .AddTo(this);
    }

    /// <summary>
    /// キャラをプレイヤーのチームに追加・削除
    /// </summary>
    public void ModifyPlayerTeam()
    {
        // TODO 本体のボタンの見た目変更(選択されていることがわかるようにする)

        // すでに選択されているボタン、またはコピーのボタンを押した場合
        if (IsSelected.Value) //|| isCopied)
        {
            // キャラをチームから外す
            teamAssemblyPop.playerTeamInfo.RemoveAll(data => data.name == charaData.name);  // RemoveではなくRemoveAllを使えば、ラムダ式を使ってより簡潔に記述できる

            // 画面うえからCopyButtonを破棄
            teamAssemblyPop.SetCopyButton(false, this);

            IsSelected.Value = false;

            return;
        }
        else
        {
            // チームがすでに満員の場合、処理しない
            if (teamAssemblyPop.IsTeamAtMaxCapacity()) return;
            // {
            //     return;
            // }

            // キャラをチームに追加
            var chara = new GameData.CharaConstData(charaData.name, charaData.level, charaData.rarity);
            teamAssemblyPop.playerTeamInfo.Add(chara);

            // 画面うえにCopyButtonを生成
            teamAssemblyPop.SetCopyButton(true, this);

            IsSelected.Value = true;
        }
    }
}
