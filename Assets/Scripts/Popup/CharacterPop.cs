using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPop : PopupBase
{
    [SerializeField] private Button btnToOwnedCharaPop;
    [SerializeField] private Button btnToEvolutionPop;

    [SerializeField] private CanvasGroup toOwnedCharaButtonNotSelectGroup;
    [SerializeField] private CanvasGroup toEvolutionNotSelectGroup;


    public override void Setup()
    {
        base.Setup();

        // TODO Rx.Selectでストリームデータを変換、toOwnedCharaPopのtrue,falseでalphaと開くポップアップを制御
        btnToOwnedCharaPop.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.1f))
            .Subscribe(_ =>
            {
                toOwnedCharaButtonNotSelectGroup.alpha = 1;
                toEvolutionNotSelectGroup.alpha = 0;

                PopupManager.instance.Show<OwnedCharaPop>();  // TODO Show()に前回のポップアップを閉じる処理を追加、trueで前回のポップアップを閉じる
            })
            .AddTo(this);

        btnToEvolutionPop.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.1f))
            .Subscribe(_ =>
            {
                toOwnedCharaButtonNotSelectGroup.alpha = 0;
                toEvolutionNotSelectGroup.alpha = 1;

                PopupManager.instance.Show<EvolutionPop>();
            })
            .AddTo(this);
    }

    // TODO ShowPopup()
}
