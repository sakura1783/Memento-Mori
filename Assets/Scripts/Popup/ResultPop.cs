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
        // btnRematch.OnClickAsObservable()
        //     .ThrottleFirst(System.TimeSpan.FromSeconds(0.1f))
        //     .Subscribe(_ => PopupManager.instance.Show<BattleManager>(true))
        //     .AddTo(this);
    }

    public void ShowPopup(BattleState battleState)
    {
        if (battleState == BattleState.Win)
        {
            winGroup.alpha = 1;
            winGroup.blocksRaycasts = true;

            txtResult.text = battleState.ToString().ToUpper();  // ToUpper()で大文字に変換
        }
        else
        {
            loseGroup.alpha = 1;
            loseGroup.blocksRaycasts = true;

            txtResult.text = battleState.ToString().ToUpper();
        }

        base.ShowPopup();
    }
}
