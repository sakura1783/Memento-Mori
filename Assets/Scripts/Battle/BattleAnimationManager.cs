using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public enum AnimationType
{
    // DOTWeen使用アニメーション
    Attack,  // 通常攻撃・追撃時
    Damage,

    // パーティクルシステム使用アニメーション
    DefaultHit,
    SwordHit,
    GunHit,
    Heal,
    ActiveSkill,  // アクティブスキル使用時
    ReceiveBuff,
    ReceiveDebuff,
}

public class BattleAnimationManager : AbstractSingleton<BattleAnimationManager>
{
    [System.Serializable]
    private class EffectObjData
    {
        [SerializeField] private GameObject effectPrefab;
        public GameObject Effectprefab => effectPrefab;

        [SerializeField] int scaleAdjustmentValue = 1;
        public int ScaleAdjustmentValue => scaleAdjustmentValue;
    }

    [SerializeField] private BattleManager battleManager;

    [SerializeField] private EffectObjData[] effects = new EffectObjData[6];  // AnimationType順に順番にプレハブを入れる

    private List<UniTask> animationTasks = new();


    public void AddAnimation(CharaController target, AnimationType animationType)
    {
        animationTasks.Add(PlayAnimation(target, animationType));
    }

    public async UniTask WaitAllAnimations()
    {
        if (animationTasks.Count == 0)
            return;

        var waitAnimations = UniTask.WhenAll(animationTasks);
        var waitMinimumTime = UniTask.Create(async () =>  // UniTask.Createで、メソッド化せずその場で記述
        {
            // 最低でも1秒は待つ
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.8f));
            await battleManager.SkillUserImageGroup.DOFade(0, 0.2f).SetEase(Ease.Linear).AsyncWaitForCompletion();  // DOTweenの完了を待ちたいときは、AsyncWaitForCompletion()を使う
        });
        
        await UniTask.WhenAll(waitAnimations, waitMinimumTime);

        animationTasks.Clear();
    }

    private UniTask PlayAnimation(CharaController target, AnimationType animationType)
    {
        var rect = animationType == AnimationType.Attack || animationType == AnimationType.Damage
            ? target.CharaStatusPannel.AnimationRoot
            : target.CharaStatusPannel.ImgChara.transform as RectTransform;

        return animationType switch
        {
            AnimationType.Attack => 
                PlayAttackAnimation(rect,target),

            AnimationType.Damage =>
                PlayDamageAnimation(rect, target),

            AnimationType.DefaultHit
            or AnimationType.SwordHit
            or AnimationType.GunHit
            or AnimationType.Heal
            or AnimationType.ActiveSkill
            or AnimationType.ReceiveBuff
            or AnimationType.ReceiveDebuff =>
                InstantiateEffect(rect, animationType),

            _ => UniTask.CompletedTask
        };
    }

    private UniTask PlayAttackAnimation(RectTransform animePoint, CharaController target)
    {
        Vector3 pos = new(battleManager.PlayerTeam.Contains(target) ? 40f : -40f, 0f, 0f);

        return animePoint
            .DOPunchPosition(pos, 0.7f, 2).ToUniTask();
    }

    private UniTask PlayDamageAnimation(RectTransform animePoint, CharaController target)
    {
        Vector3 pos = new(battleManager.PlayerTeam.Contains(target) ? -15f : 15f, -5f, 0f);
        
        return animePoint
            .DOPunchPosition(pos, 0.5f, 8).ToUniTask();
    }

    private UniTask InstantiateEffect(RectTransform effectPoint, AnimationType animationType)
    {
        var effectData = effects[(int)animationType - 2];  // AnimationType列挙型の最初の2つはDOTweenアニメーションなので、配列要素のインデックスを合わせるために-2する

        var obj = Instantiate(effectData.Effectprefab, effectPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * effectData.ScaleAdjustmentValue;

        // receiveBuffエフェクトはゲーム実行中に複数の子が生成されるため、OrderInLayerも動的に変更
        if (animationType == AnimationType.ReceiveBuff)
        {
            var renderers = obj.GetComponentsInChildren<ParticleSystemRenderer>(true);

            foreach (var renderer in renderers)
                renderer.sortingOrder = 1;
        }

        // パーティクルの再生が終了し、破棄されるまで待つ
        return UniTask.WaitUntil(() => obj == null);
    }
}
