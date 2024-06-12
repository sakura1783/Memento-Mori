using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// キャラの状態の可視化のみを担う
/// </summary>
public class CharaStatusPannel : MonoBehaviour
{
    [SerializeField] private Image imgChara;

    [SerializeField] private Text txtCharaInfo;  // txtCharaLv,Nameをアサイン
    [SerializeField] private Text txtHpValue;

    [SerializeField] private Slider hpSlider;

    [SerializeField] private Transform buffPlace;


    public void Setup(CharaController charaController, TeamAssemblyPop.TeamMemberInfo charaInfo)
    {
        imgChara.sprite = SpriteManager.instance.GetCharaSprite(charaInfo.name, CharaSpriteType.Face);
        txtCharaInfo.text = $"{charaInfo.level}Lv {charaInfo.name}";
        txtHpValue.text = $"{charaController.Hp} / {charaController.MaxHp}";
        hpSlider.value = 1;

        // 購読処理
        charaController.Hp
            .Subscribe(value =>
            {
                txtHpValue.text = $"{charaController.Hp} / {charaController.MaxHp}";
                hpSlider.value = charaController.Hp.Value / charaController.MaxHp;
            });
    }
}
