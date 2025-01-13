using UnityEngine;
using UnityEngine.UI;

public class ResultPop : PopupBase
{
    [SerializeField] private Text txtResult;

    [SerializeField] private CanvasGroup winGroup;
    [SerializeField] private CanvasGroup loseGroup;


    public override void Setup()
    {
        winGroup.alpha = 0;
        winGroup.blocksRaycasts = false;

        loseGroup.alpha = 0;
        loseGroup.blocksRaycasts = false;

        base.Setup();
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
