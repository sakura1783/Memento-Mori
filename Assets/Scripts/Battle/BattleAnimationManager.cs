using System;
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

    Trajectory,  // 攻撃者→ターゲットへの軌跡エフェクト
}

public class BattleAnimationManager : AbstractSingleton<BattleAnimationManager>
{
    [Serializable]
    private class EffectObjData
    {
        [SerializeField] private GameObject effectPrefab;
        public GameObject Effectprefab => effectPrefab;

        [SerializeField] int scaleAdjustmentValue = 1;
        public int ScaleAdjustmentValue => scaleAdjustmentValue;
    }

    [SerializeField] private BattleManager battleManager;

    [SerializeField] private EffectObjData[] effects = new EffectObjData[6];  // AnimationType順に順番にプレハブを入れる
    [SerializeField] private GameObject trajectoryEffect;

    private List<UniTask> animationTasks = new();

    public const float HIT_DELAY = 0.17f;  // ヒット間のインターバル


    public void AddAnimation(CharaController target, AnimationType animationType, int hitIndex = 0, int maxHitCount = 1, CharaController user = null)
    {
        animationTasks.Add(PlayAnimation(target, animationType, hitIndex, maxHitCount, user));
    }

    public async UniTask WaitAllAnimations()
    {
        if (animationTasks.Count == 0)
            return;

        var waitAnimations = UniTask.WhenAll(animationTasks);
        var waitMinimumTime = UniTask.Create(async () =>  // UniTask.Createで、メソッド化せずその場で記述
        {
            // 最低でも1秒は待つ
            await UniTask.Delay(TimeSpan.FromSeconds(0.8f));
            await battleManager.SkillUserImageGroup.DOFade(0, 0.2f).SetEase(Ease.Linear).AsyncWaitForCompletion();  // DOTweenの完了を待ちたいときは、AsyncWaitForCompletion()を使う
        });
        
        await UniTask.WhenAll(waitAnimations, waitMinimumTime);

        animationTasks.Clear();
    }

    private async UniTask PlayAnimation(CharaController target, AnimationType animationType, int hitIndex = 0, int maxHitCount = 1, CharaController user = null)
    {
        if (hitIndex > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(hitIndex * HIT_DELAY));

        var rect = animationType == AnimationType.Attack || animationType == AnimationType.Damage
            ? target.CharaStatusPannel.AnimationRoot
            : target.CharaStatusPannel.ImgChara.transform as RectTransform;

        await (animationType switch
        {
            AnimationType.Attack => 
                PlayAttackAnimation(rect,target),

            AnimationType.Damage =>
                PlayDamageAnimation(rect, target, hitIndex, maxHitCount),

            AnimationType.DefaultHit
            or AnimationType.SwordHit
            or AnimationType.GunHit
            or AnimationType.Heal
            or AnimationType.ActiveSkill
            or AnimationType.ReceiveBuff
            or AnimationType.ReceiveDebuff 
                => InstantiateEffect(rect, animationType),
            
            AnimationType.Trajectory when user != null =>
                InstantiateTrajectoryEffect(user, target),

            _ => UniTask.CompletedTask
        });
    }

    private UniTask PlayAttackAnimation(RectTransform animePoint, CharaController target)
    {
        Vector3 pos = new(battleManager.PlayerTeam.Contains(target) ? 40f : -40f, 0f, 0f);

        return animePoint
            .DOPunchAnchorPos(pos, 0.7f, 2).ToUniTask();
    }

    private async UniTask PlayDamageAnimation(RectTransform animePoint, CharaController target, int hitIndex, int maxHitCount)
    {
        Vector3 pos = new(battleManager.PlayerTeam.Contains(target) ? -15f : 15f, -5f, 0f);
        
        // 最後のダメージアニメーションだけ長く、それ以外は短くアニメーションさせる
        if (hitIndex < maxHitCount - 1)  // indexは0始まりなのでmaxHitCount-1をする
        {
            await animePoint
                .DOPunchAnchorPos(pos, HIT_DELAY, 5).ToUniTask();
        }
        else
        {
            await animePoint
                .DOPunchAnchorPos(pos, 0.5f, 8).ToUniTask();
            
            // 位置が誤差程度ずれるので、強制的に元の位置に戻す
            animePoint.anchoredPosition = target.CharaStatusPannel.DefaultAnimationRootPos;
        }
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

    private async UniTask InstantiateTrajectoryEffect(CharaController attacker, CharaController target)
    {
        var startPos = attacker.CharaStatusPannel.ImgChara.rectTransform.position;
        var endPos = target.CharaStatusPannel.ImgChara.rectTransform.position;

        var effect = Instantiate(trajectoryEffect, startPos, Quaternion.identity);

        await effect.transform
            .DOMove(endPos, 0.1f).SetEase(Ease.InQuad).ToUniTask();
    }
}
