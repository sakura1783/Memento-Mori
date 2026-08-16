using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// バトルのキャラの状態の可視化のみを担う
/// </summary>
public class CharaStatusPannel : MonoBehaviour
{
    [SerializeField] private Image imgChara;
    public Image ImgChara => imgChara;

    [SerializeField] private Text txtCharaInfo;  // txtCharaLv,Nameをアサイン
    [SerializeField] private Text txtHpValue;

    [SerializeField] private Slider hpSlider;

    [SerializeField] private RectTransform buffPlace;
    [SerializeField] private Image buffPrefab;

    [SerializeField] private CanvasGroup inactiveGroup;

    [SerializeField] private RectTransform animationRoot;
    public RectTransform AnimationRoot => animationRoot;
    
    public Vector2 DefaultAnimationRootPos { get; private set; }

    private readonly Dictionary<Buff, Image> buffs = new();


    public void Setup(CharaController chara, GameData.CharaConstData charaData)
    {
        chara.CharaStatusPannel = this;

        imgChara.sprite = SpriteManager.instance.GetCharaSprite(charaData.name, CharaSpriteType.Face);
        txtCharaInfo.text = $"Lv{charaData.level} {DataBaseManager.instance.charaInitialDataSO.charaInitialDataList.FirstOrDefault(data => data.englishName == charaData.name).name}";
        txtHpValue.text = $"{chara.Status.Hp} / {chara.Status.MaxHp}";
        hpSlider.value = 1;

        DefaultAnimationRootPos = animationRoot.anchoredPosition;

        // 購読処理
        Observable.Merge(chara.Status.Hp, chara.Status.MaxHp)  // Observable.Merge()で、括弧内の値いずれかが変更された場合にSubscribe()の処理が動く
            .Subscribe(_ =>
            {
                int hp = chara.Status.Hp.Value;
                int maxHp = chara.Status.MaxHp.Value;

                txtHpValue.text = $"{hp} / {maxHp}";
                hpSlider.value = (float)hp / maxHp;

                // 戦闘不能になった時、パネルを暗くする/文字を赤色にする
                if (hp <= 0)
                {
                    inactiveGroup.alpha = 1;

                    var defeatedColor = new Color32(210, 55, 55, 255);
                    txtCharaInfo.color = defeatedColor;
                    txtHpValue.color = defeatedColor;
                }
            })
            .AddTo(this);

        chara.Status.Buffs
            .ObserveAdd()
            .Subscribe(eventData =>  // <= ObserveAddが提供するイベントデータ
            {
                var buff = Instantiate(buffPrefab, buffPlace);
                buff.sprite = SpriteManager.instance.GetDebuffSprite(eventData.Value.type);
                buffs.Add(eventData.Value, buff);
            })
            .AddTo(this);

        chara.Status.Buffs
            .ObserveRemove()
            .Subscribe(eventData =>
            {
                var buff = eventData.Value;

                if (!buffs.TryGetValue(buff, out var icon))
                    return;
                
                Destroy(icon.gameObject);
                buffs.Remove(buff);
            })
            .AddTo(this);

        chara.Status.Buffs
            .ObserveReset()  // Clear()された時(= キャラが戦闘不能になった時)
            .Subscribe(_ =>
            {
                // バフのオブジェクトを全て削除
                foreach (Transform child in buffPlace)
                    Destroy(child.gameObject);

                buffs.Clear();
            })
            .AddTo(this);
    }
}
