using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FloatingMessage : MonoBehaviour
{
    [SerializeField] private Text txtFloatingMessage;


    public enum FloatingMessageType {
        Damage,
        Heal,
        Critical,

        // Miss,
    }

    /// <summary>
    /// フロート表示の制御
    /// </summary>
    /// <param name="floatingValue"></param>
    /// <param name="floatingMessageType"></param>
    public void DisplayFloatingMessage(int floatingValue, FloatingMessageType floatingMessageType) 
    {
        txtFloatingMessage.text = floatingMessageType == FloatingMessageType.Critical
            ? $"Critical\n{floatingValue}"
            : floatingValue.ToString();
        txtFloatingMessage.color = GetMessageColor(floatingMessageType);

        // フロート表示を上に移動させ、1.5秒後にフェードアウトを開始し、終わったら破棄する
        DOTween.Sequence()
            .Append(transform.DOLocalMoveY(transform.localPosition.y + 100f, 2.0f))
            .Insert(1.5f, txtFloatingMessage.DOFade(0f, 0.5f).SetEase(Ease.InQuad))
            .OnComplete(() => Destroy(gameObject));
    }

    /// <summary>
    /// フロート表示の色を設定
    /// </summary>
    /// <param name="floatingMessageType"></param>
    /// <returns></returns>
    private Color GetMessageColor(FloatingMessageType floatingMessageType)
    {
        string hexColor = floatingMessageType switch  // TODO ColorManagerを介し色を取得
        {
            FloatingMessageType.Damage   => "#8A3327",
            FloatingMessageType.Heal     => "#458233",
            FloatingMessageType.Critical => "#A9922F",
            _                            => "#FFFFFF",
        };
        return ColorUtility.TryParseHtmlString(hexColor, out Color color) ? color : Color.white;
    }
}
