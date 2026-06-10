using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ResultPop : PopupBase
{
    [SerializeField] private Text txtResult;

    [SerializeField] private CanvasGroup winGroup;
    [SerializeField] private CanvasGroup loseGroup;

    [SerializeField] private Button btnRematch;


    public override void Setup()
    {
        winGroup.alpha = 0;
        winGroup.blocksRaycasts = false;

        loseGroup.alpha = 0;
        loseGroup.blocksRaycasts = false;

        base.Setup();

        // TODO 実装
        btnRematch.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.1f))
            .Subscribe(_ => PopupManager.instance.Show<BattleManager>(true))
            .AddTo(this);
    }

    public void ShowPopup(BattleState battleState)
    {
        var canvasGroup = battleState == BattleState.Win ? winGroup : loseGroup;

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        txtResult.text = battleState.ToString().ToUpper();  // ToUpper()で大文字に変換

        base.ShowPopup();
        PopupManager.instance.PreviousPop = this;
    }

    public override void HidePopup()
    {
        base.HidePopup();
    }
}
