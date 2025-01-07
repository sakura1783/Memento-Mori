using UnityEngine;

public class ResultPop : PopupBase
{
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

    public override void ShowPopup(BattleState battleState)
    {
        if (battleState == BattleState.Win)
        {
            winGroup.alpha = 1;
            winGroup.blocksRaycasts = true;
        }
        else
        {
            loseGroup.alpha = 1;
            loseGroup.blocksRaycasts = true;
        }

        base.ShowPopup();
    }
}
