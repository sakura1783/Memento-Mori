using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;

/// <summary>
/// バトルのキャラの状態の可視化のみを担う
/// </summary>
public class CharaStatusPannel : MonoBehaviour
{
    [SerializeField] private Image imgChara;

    [SerializeField] private Text txtCharaInfo;  // txtCharaLv,Nameをアサイン
    [SerializeField] private Text txtHpValue;

    [SerializeField] private Slider hpSlider;

    [SerializeField] private Transform buffPlace;
    [SerializeField] private Image debuffPrefab;


    public void Setup(CharaController charaController, GameData.CharaConstData charaData)
    {
        imgChara.sprite = SpriteManager.instance.GetCharaSprite(charaData.name, CharaSpriteType.Face);
        txtCharaInfo.text = $"Lv{charaData.level} {DataBaseManager.instance.charaInitialDataSO.charaInitialDataList.FirstOrDefault(data => data.englishName == charaData.name).name}";
        txtHpValue.text = $"{charaController.Status.Hp} / {charaController.Status.MaxHp}";
        hpSlider.value = 1;

        // 購読処理
        Observable.Merge(charaController.Status.Hp, charaController.Status.MaxHp)  // Observable.Merge()で、括弧内の値いずれかが変更された場合にSubscribe()の処理が動く
            .Subscribe(value =>
            {
                txtHpValue.text = $"{charaController.Status.Hp} / {charaController.Status.MaxHp}";
                hpSlider.value = charaController.Status.Hp.Value / charaController.Status.MaxHp.Value;
            })
            .AddTo(this);

        charaController.Status.Debuffs
            .ObserveAdd()
            .Subscribe(eventData =>  // <= ObserveAddが提供するイベントデータ
            {
                var debuff = Instantiate(debuffPrefab, buffPlace);
                debuff.sprite = SpriteManager.instance.GetDebuffSprite(eventData.Value.type);  // 引数(eventData).Valueでコレクションに追加された要素を取得
            })
            .AddTo(this);

        charaController.Status.Debuffs
            .ObserveRemove()
            .Subscribe(eventData => Destroy(gameObject))
            .AddTo(this);
    }
}
