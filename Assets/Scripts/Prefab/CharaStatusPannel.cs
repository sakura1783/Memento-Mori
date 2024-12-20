using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;

/// <summary>
/// キャラの状態の可視化のみを担う
/// </summary>
public class CharaStatusPannel : MonoBehaviour
{
    private CharaController charaController;
    public CharaController CharaController => charaController;

    [SerializeField] private Image imgChara;

    [SerializeField] private Text txtCharaInfo;  // txtCharaLv,Nameをアサイン
    [SerializeField] private Text txtHpValue;

    [SerializeField] private Slider hpSlider;

    [SerializeField] private Transform buffPlace;


    public void Setup(CharaController charaController, GameData.CharaConstData charaData)
    {
        this.charaController = charaController;

        imgChara.sprite = SpriteManager.instance.GetCharaSprite(charaData.name, CharaSpriteType.Face);
        txtCharaInfo.text = $"Lv{charaData.level} {DataBaseManager.instance.charaInitialDataSO.charaInitialDataList.FirstOrDefault(data => data.englishName == charaData.name).name}";
        txtHpValue.text = $"{charaController.Status.Hp} / {charaController.MaxHp}";
        hpSlider.value = 1;

        // 購読処理
        charaController.Status.Hp
            .Subscribe(value =>
            {
                txtHpValue.text = $"{charaController.Status.Hp} / {charaController.MaxHp}";
                hpSlider.value = charaController.Status.Hp.Value / charaController.MaxHp;
            });
    }
}
