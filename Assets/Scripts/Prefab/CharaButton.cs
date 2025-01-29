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

    private CopyButton copyButton;  // 画面うえに生成した、コピーされたCharaButtonのゲームオブジェクト
    public CopyButton CopyButton
    {
        get => copyButton;
        set => copyButton = value;
    }

    // CharaButton copyButton;  // 作る場合、上を修正
    // CharaButton baseButton;  // どちらに値が入っているかで、自身がベースかコピーか判断し、それぞれ適切な処理を行わせる
    // コピーを削除、自身のIsSelectedをfalseにする処理


    public void Setup(GameData.CharaConstData charaData)
    {
        this.charaData = charaData;

        imgChara.sprite = SpriteManager.instance.GetCharaSprite(charaData.name, CharaSpriteType.Face);
        imgRank.color = ColorManager.instance.GetColorByRarity(charaData.rarity);
        // TODO imgAttribute
        txtCharaLevel.text = $"Lv{charaData.level}";

        IsSelected
            .Subscribe(value => selectedSet.alpha = value ? 1 : 0)
            .AddTo(this);
    }
}
