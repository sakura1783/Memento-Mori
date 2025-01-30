using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// キャラ選択用ボタン(キャラ一覧、キャラ編成画面、ガチャなどで使用)
/// (このクラスは、見た目の制御のみを担う)
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
    public Image ImgRank
    {
        get => imgRank;
        set => imgRank = value;
    }
    [SerializeField] private Image imgAttribute;

    [SerializeField] private Text txtCharaLevel;

    [SerializeField] private CanvasGroup selectedSet;

    private GameData.CharaConstData charaData;
    public GameData.CharaConstData CharaData
    {
        get => charaData;
        set => charaData = value;
    }

    public ReactiveProperty<bool> IsSelected = new();

    private CharaButton copyButton;  // 自身がベースの場合、ここにコピーの情報が入る(自身がコピーの場合はnull)
    public CharaButton CopyButton
    {
        get => copyButton;
        set => copyButton = value;
    }

    private CharaButton baseButton;  // 自身がコピーの場合、ここにベースの情報が入る(自身がベースの場合はnull)
    public CharaButton BaseButton
    {
        get => baseButton;
        set => baseButton = value;
    }


    /// <summary>
    /// CharaButton(本体)の初期設定
    /// </summary>
    /// <param name="charaData"></param>
    public void Setup(GameData.CharaConstData charaData)
    {
        this.charaData = charaData;

        IsSelected
            .Subscribe(value => selectedSet.alpha = value ? 1 : 0)
            .AddTo(this);

        SetCharaDetails(charaData);
    }

    /// <summary>
    /// CopyButton(CharaButtonのコピー)の初期設定
    /// </summary>
    /// <param name="baseButton">コピーする本体のボタン</param>
    public void Setup(CharaButton baseButton)
    {
        this.baseButton = baseButton;
        charaData = baseButton.CharaData;  // EvolutionPopではコピーのコピーを作成するので、この情報が必要となる

        SetCharaDetails(baseButton.CharaData);
    }

    /// <summary>
    /// キャラの詳細を各UIに設定
    /// </summary>
    /// <param name="charaData"></param>
    private void SetCharaDetails(GameData.CharaConstData charaData)
    {
        imgChara.sprite = SpriteManager.instance.GetCharaSprite(charaData.name, CharaSpriteType.Face);
        imgRank.color = ColorManager.instance.GetColorByRarity(charaData.rarity);
        // TODO imgAttribute
        txtCharaLevel.text = $"Lv{charaData.level}";
    }
}
