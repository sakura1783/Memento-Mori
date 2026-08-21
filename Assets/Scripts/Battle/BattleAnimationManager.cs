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
        [SerializeField] private AnimationType animationType;
        public AnimationType AnimationType => animationType;

        [SerializeField] private GameObject effectPrefab;
        public GameObject Effectprefab => effectPrefab;

        [SerializeField] int scaleAdjustmentValue = 1;
        public int ScaleAdjustmentValue => scaleAdjustmentValue;
    }

    [SerializeField] private BattleManager battleManager;

    [SerializeField] private RectTransform effectRoot;  // 軌跡エフェクトをこれの子として生成する

    [SerializeField] private List<EffectObjData> effects = new();  // AnimationType順に順番にプレハブを入れる
    [SerializeField] private ParticleSystem trajectoryEffect;


    /// <summary>
    /// アニメーション登録
    /// </summary>
    /// <param name="target"></param>
    /// <param name="animationType"></param>
    /// <param name="delay">省略した場合(=中身がnull)、現在のスコープの遅延を使用し、値を指定した場合、その分だけ遅延する</param>
    /// <param name="user"></param>
    /// <param name="playLongDamageAnimation"></param>
    public void AddAnimation(CharaController target, AnimationType animationType, float additionalDelay = 0f, CharaController user = null, bool playLongDamageAnimation = true)
    {
        BattleActionTimeline.instance.Schedule(()=> PlayAnimation(target, animationType, user, playLongDamageAnimation), additionalDelay);
    }

    private async UniTask PlayAnimation(CharaController target, AnimationType animationType, CharaController user = null, bool isLongDamageAnimation = true)
    {
        var rect = animationType == AnimationType.Attack || animationType == AnimationType.Damage
            ? target.CharaStatusPannel.AnimationRoot
            : target.CharaStatusPannel.ImgChara.rectTransform;

        await (animationType switch
        {
            AnimationType.Attack => 
                PlayAttackAnimation(rect,target),

            AnimationType.Damage =>
                PlayDamageAnimation(rect, target, isLongDamageAnimation),

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

    private async UniTask PlayDamageAnimation(RectTransform animePoint, CharaController target, bool isLongAnimation)
    {
        Vector3 pos = new(battleManager.PlayerTeam.Contains(target) ? -15f : 15f, -5f, 0f);
        
        float duration = isLongAnimation ? AttackSequencePlanBuilder.LONG_HIT_DURATION : AttackSequencePlanBuilder.SHORT_HIT_DURATION;
        int vibrato = isLongAnimation ? 5 : 3;
        
        await animePoint
            .DOPunchAnchorPos(pos, duration, vibrato).ToUniTask();

        // 位置が誤差程度ずれるので、強制的に元の位置に戻す  // TODO タイミング変更？
        animePoint.anchoredPosition = target.CharaStatusPannel.DefaultAnimationRootPos;
    }

    private UniTask InstantiateEffect(RectTransform effectPoint, AnimationType animationType)
    {
        EffectObjData effectData = effects.FirstOrDefault(x => x.AnimationType == animationType);

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
        var attackerRect = attacker.CharaStatusPannel.ImgChara.rectTransform;
        var targetRect = target.CharaStatusPannel.ImgChara.rectTransform;

        var effect = Instantiate(trajectoryEffect, attackerRect.position, Quaternion.identity, effectRoot);  // 指定した親の子として生成
        effect.Clear();
        effect.Play();

        await effect.transform
            .DOMove(targetRect.position, AttackSequencePlanBuilder.TRAJECTORY_DURATION).SetEase(Ease.InQuad).ToUniTask();  // DOMove()にはワールド座標を指定する必要がある

        // TODO これ以外にも色々やってみたけど、だめ
        // effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // float distance = Vector3.Distance(attackerRect.position, targetRect.position);
        // Debug.Log($"Trajectory Distance : {distance}");
    }
}
