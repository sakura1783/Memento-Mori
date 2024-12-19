using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;

    [SerializeField] protected Button btnClose;


    public virtual void Setup()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        // btnCloseが存在する場合だけ
        if (btnClose)
        {
            btnClose.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => HidePopup())
                .AddTo(this);
        }
    }

    /// <summary>
    /// ポップアップを表示
    /// </summary>
    public virtual void ShowPopup()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// ポップアップの非表示
    /// </summary>
    public virtual void HidePopup()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
