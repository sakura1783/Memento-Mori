using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// OwnedCharaPopとEvolutionPopの切り替え用ポップアップクラス
/// </summary>
public class CharacterPop : PopupBase
{
    [SerializeField] private Button btnToOwnedCharaPop;
    [SerializeField] private Button btnToEvolutionPop;

    [SerializeField] private CanvasGroup toOwnedCharaButtonNotSelectGroup;
    [SerializeField] private CanvasGroup toEvolutionNotSelectGroup;


    public override void Setup()
    {
        base.Setup();

        Observable.Merge
        (
            btnToOwnedCharaPop.OnClickAsObservable().Select(_ => true),
            btnToEvolutionPop.OnClickAsObservable().Select(_ => false)
        )
        .Subscribe(toOwnedCharaPop =>
        {
            toOwnedCharaButtonNotSelectGroup.alpha = toOwnedCharaPop ? 0 : 1;
            toEvolutionNotSelectGroup.alpha = toOwnedCharaPop ? 1 : 0;
            
            if (toOwnedCharaPop) PopupManager.instance.Show<OwnedCharaPop>(true);
            else PopupManager.instance.Show<EvolutionPop>(true);
        })
        .AddTo(this);
    }

    public override void ShowPopup()
    {
        PopupManager.instance.Show<OwnedCharaPop>(false);

        base.ShowPopup();
    }
}
