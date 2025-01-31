using System.Collections.Generic;
using System.Linq;
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

    private static readonly Dictionary<Rarity, Rarity> evolutionRarityMap = new()
    {
        {Rarity.R_, Rarity.SR},
        {Rarity.SR_, Rarity.SSR},
        {Rarity.SSR_, Rarity.UR},
        {Rarity.UR_, Rarity.LR},
    };

    // Dictionary<進化するキャラのレアリティ, (消費キャラのレアリティ, 同名キャラか, 必要数)>
    private static readonly Dictionary<Rarity, (Rarity, bool, int)> evolutionRequirements = new()
    {
        {Rarity.R, (Rarity.R, true, 2)},
        {Rarity.R_, (Rarity.R_, false, 2)},
        {Rarity.SR, (Rarity.SR, true, 1)},
        {Rarity.SR_, (Rarity.SR_, false, 2)},
        {Rarity.SSR, (Rarity.SR_, true, 1)},
        {Rarity.SSR_, (Rarity.SSR_, false, 1)},
        {Rarity.UR, (Rarity.SSR_, false, 1)},
        {Rarity.UR_, (Rarity.SR_, true, 2)},
        {Rarity.LR, (Rarity.SR_, true, 1)},
        {Rarity.LR_1, (Rarity.SR_, true, 1)},
        {Rarity.LR_2, (Rarity.SR_, true, 1)},
        {Rarity.LR_3, (Rarity.SR_, true, 1)},
        {Rarity.LR_4, (Rarity.SR_, true, 2)},
    };


    public override void Setup()
    {
        base.Setup();

        // TODO 各ボタンの監視処理
    }

    public override void ShowPopup()
    {
        SwitchDisplay(true);

        GameData.instance.ownedCharaDataList.ForEach(data =>
        {
            var charaButton = CreateCharaButton(data, null, charactersTran, 1.2f);
            
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
        if (beforeEvolveTran.childCount <= 0)
        {
            SwitchDisplay(false);

            // pushedButtonのコピーを生成
            var evolveChara = CreateCharaButton(null, pushedButton, beforeEvolveTran, 1.3f);

            // 進化後のCharaButtonを生成
            var afterEvolveChara = CreateCharaButton(null, evolveChara, afterEvolveTran, 1.3f);
            afterEvolveChara.Button.interactable = false;
            if (evolutionRarityMap.Any(data => data.Key == pushedButton.CharaData.rarity)) afterEvolveChara.ImgRank.color = ColorManager.instance.GetColorByRarity(evolutionRarityMap[pushedButton.CharaData.rarity]);  // 適切なレアリティの色を枠に設定

            // 進化に必要なキャラを表示
            var requirement = evolutionRequirements[pushedButton.CharaData.rarity];
            for (int i = 0; i < requirement.Item3; i++)
            {
                GameData.CharaConstData charaData = new(requirement.Item2 ? pushedButton.CharaData.name : CharaName.None, 0, requirement.Item1);

                var requireChara = CreateCharaButton(charaData, null, requireCharasTran, 0.9f);
                requireChara.TxtCharaLevel.text = "";
            }

            // TODO 消費可能なキャラ以外は選べない(CharaButtonのinteractableをfalseに、色も変更して選べないことを可視化)
        }

        // 進化するキャラが選択されていて、進化するキャラとは異なるキャラ(=消費するキャラ)を選択した場合
        else if (beforeEvolveTran.childCount >= 1 && !pushedButton.BaseButton.IsSelected.Value)
        {
            foreach (Transform child in requireCharasTran)
            {
                // 進化素材がすでに全て選択されている場合は処理しない
                if (child.childCount > 0) return;

                CreateCharaButton(null, pushedButton, child, 0.9f);
            }
        }

        // いずれかで選択しているキャラと全く同じキャラのCharaButtonを押した場合(= キャラを取り消し)
        else
        {
            // 自身が本体のボタンである場合
            if (!pushedButton.IsCopied)
            {
                Destroy(pushedButton.CopyButton.gameObject);
                pushedButton.CopyButton = null;

                // 進化予定であったキャラの場合、requireCharasTran下のオブジェクトを全削除
                if (pushedButton.CopyButton.transform.parent == beforeEvolveTran) foreach (Transform child in requireCharasTran) Destroy(child.gameObject);
            }
            // 自身がコピーのボタンである場合
            else
            {
                pushedButton.BaseButton.CopyButton = null;
                Destroy(pushedButton.gameObject);
                
                if (pushedButton.transform.parent == beforeEvolveTran) foreach (Transform child in requireCharasTran) Destroy(child.gameObject);
            }

            pushedButton.BaseButton.IsSelected.Value = false;
        }
    }

    // TODO 各ボタンを押した際のメソッド

    // TODO GameData.ownedCharaListに、進化後のキャラを追加・進化前のキャラと消費したキャラを削除


    /// <summary>
    /// CharaButtonの作成
    /// </summary>
    /// <param name="charaData"></param>
    /// <param name="baseButton"></param>
    /// <param name="parentTran"></param>
    /// <param name="scaleValue"></param>
    /// <returns></returns>
    private CharaButton CreateCharaButton(GameData.CharaConstData charaData, CharaButton baseButton, Transform parentTran, float scaleValue)
    {
        var charaButton = Instantiate(charaButtonPrefab, parentTran);

        if (charaData != null) charaButton.Setup(charaData);
        else charaButton.Setup(baseButton);
        charaButton.transform.localScale = new Vector2(scaleValue, scaleValue);

        return charaButton;
    }
    
    /// <summary>
    /// 画面表示を切り替え
    /// </summary>
    /// <param name="showStep1Group"></param>
    private void SwitchDisplay(bool showStep1Group)
    {
        step1Group.alpha = showStep1Group ? 1 : 0;
        step1Group.blocksRaycasts = showStep1Group;

        step2Group.alpha = showStep1Group ? 0 : 1;
        step2Group.blocksRaycasts = !showStep1Group;
    }
}
