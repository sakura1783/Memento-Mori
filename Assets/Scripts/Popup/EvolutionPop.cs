using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionPop : PopupBase
{
    [SerializeField] private Transform charactersTran;

    [SerializeField] private Transform beforeEvolveTran;
    [SerializeField] private Transform afterEvolveTran;

    [SerializeField] private Transform requireCharasTran;

    [SerializeField] private CanvasGroup step1Group;
    [SerializeField] private CanvasGroup step2Group;

    [SerializeField] private Button btnEvovle;
    [SerializeField] private Button btnEvolveAll;

    [SerializeField] private Button btnRelease;

    [SerializeField] private CharaButton charaButtonPrefab;


    public override void Setup()
    {
        base.Setup();

        // TODO 各ボタンの監視処理
    }

    public override void ShowPopup()
    {
        step1Group.alpha = 1;
        step2Group.alpha = 0;
        step2Group.blocksRaycasts = false;

        GameData.instance.ownedCharaDataList.ForEach(data =>
        {
            var charaButton = Instantiate(charaButtonPrefab, charactersTran);  // TODO 必要であれば、メソッドにする(進化するたびに再生成する必要がある)
            charaButton.Setup(data);
            charaButton.transform.localScale = new Vector2(1.2f, 1.2f);
            
            charaButton.Button.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => OnClickCharaButton(charaButton))
                .AddTo(charaButton);
        });
        
        base.ShowPopup();
    }

    /// <summary>
    /// CharaButtonを押した際の処理
    /// </summary>
    /// <param name="pushedButton"></param>
    private void OnClickCharaButton(CharaButton pushedButton)
    {   
        // 進化するキャラを選択していない場合
        if (beforeEvolveTran.childCount <= 0 || pushedButton.IsSelected.Value == false)
        {

        }

        // 進化するキャラが選択されていて、進化するキャラのCharaButtonがすでに選択されている場合

        // 進化するキャラが選択されていて、進化するキャラとは異なるキャラ(=消費するキャラ)を選択した場合
    }

    // TODO 各ボタンを押した際のメソッド
}
