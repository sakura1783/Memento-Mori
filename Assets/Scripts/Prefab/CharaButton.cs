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
    public Image ImgChara
    {
        get => imgChara;
        set => imgChara = value;
    }
    [SerializeField] private Image imgRank;
    public Image ImgRank
    {
        get => imgRank;
        set => imgRank = value;
    }
    [SerializeField] private Image imgAttribute;

    [SerializeField] private Text txtCharaLevel;
    public Text TxtCharaLevel
    {
        get => txtCharaLevel;
        set => txtCharaLevel = value;
    }

    [SerializeField] private CanvasGroup selectedSet;

    private GameData.CharaConstData charaData;
    public GameData.CharaConstData CharaData
    {
        get => charaData;
        set => charaData = value;
    }

    private CharaButton copyButton;  // 自身がベースの場合、ここにコピーの情報が入る(自身がコピーの場合はnull)
    public CharaButton CopyButton
    {
        get => copyButton;
        set => copyButton = value;
    }

    private CharaButton baseButton;  // 自身が元かコピーかに関係なく、必ず情報が入る
    public CharaButton BaseButton
    {
        get => baseButton;
        set => baseButton = value;
    }

    public ReactiveProperty<bool> IsSelected = new();

    private bool isCopied;
    public bool IsCopied => isCopied;


    /// <summary>
    /// CharaButton(本体)の初期設定
    /// </summary>
    /// <param name="charaData"></param>
    public void Setup(GameData.CharaConstData charaData)
    {
        baseButton = this;

        IsSelected
            .Subscribe(value => selectedSet.alpha = value ? 1 : 0)
            .AddTo(this);

        SetDetails(charaData);
    }

    /// <summary>
    /// CopyButton(CharaButtonのコピー)の初期設定
    /// </summary>
    /// <param name="baseButton">コピーする本体のボタン</param>
    public void Setup(CharaButton baseButton)
    {
        this.baseButton = baseButton;
        baseButton.copyButton = this;
        isCopied = true;

        SetDetails(baseButton.CharaData);
    }

    /// <summary>
    /// キャラの詳細を各UIに設定
    /// </summary>
    /// <param name="charaData"></param>
    private void SetDetails(GameData.CharaConstData charaData)
    {
        this.charaData = charaData;

        imgChara.sprite = SpriteManager.instance.GetCharaSprite(charaData.name, CharaSpriteType.Face);
        imgRank.color = ColorManager.instance.GetColorByRarity(charaData.rarity);
        // TODO imgAttribute
        txtCharaLevel.text = $"Lv{charaData.level}";
    }
}
