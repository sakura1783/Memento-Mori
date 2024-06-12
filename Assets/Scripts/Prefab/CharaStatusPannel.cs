using UnityEngine;
using UnityEngine.UI;

public class CharaStatusPannel : MonoBehaviour
{
    [SerializeField] private Image imgChara;

    [SerializeField] private Text txtCharaInfo;  // txtCharaLv,Nameをアサイン
    [SerializeField] private Text txtHpValue;

    [SerializeField] private Slider hpSlider;

    [SerializeField] private Transform buffPlace;


    public void Setup()
    {

    }
}
