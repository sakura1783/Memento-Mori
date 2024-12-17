using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// CharaButtonを押した際にTeamAssemblyPopの画面うえに生成されるボタン
/// </summary>
public class CopyButton : MonoBehaviour
{
    [SerializeField] private Button button;

    [SerializeField] private Image imgChara;
    [SerializeField] private Image imgRank;
    [SerializeField] private Image imgAttribute;
    
    [SerializeField] private Text txtCharaLevel;

    private TeamAssemblyPop teamAssemblyPop;

    private CharaButton baseButton;  // (Characters下に生成してある)本体のボタン


    public void Setup(CharaButton baseButton, TeamAssemblyPop pop)
    {
        teamAssemblyPop = pop;
        this.baseButton = baseButton;

        // TODO 見た目
        imgChara.sprite = SpriteManager.instance.GetCharaSprite(baseButton.CharaData.name, CharaSpriteType.Face);

        button.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.1f))
            .Subscribe(_ => RemoveCopyButton())
            .AddTo(this);
    }

    /// <summary>
    /// Copyボタンの削除と画面うえに残っているボタンの並び替え
    /// </summary>
    public void RemoveCopyButton()
    {
        // 画面うえのCharaButton群を並び替え
        teamAssemblyPop.SortCharaButton(System.Array.FindIndex(teamAssemblyPop.PlayerTeamCharaTran, x => x == transform.parent));

        // このゲームオブジェクトを破壊
        Destroy(gameObject);

        baseButton.IsSelected = false;
        baseButton.CopyButton = null;
    }
}
