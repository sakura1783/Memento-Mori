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

    private const float EFFECT_INTERVAL = 0.1f;
    private const float MAX_EFFECT_DELAY = 0.3f;
    private readonly Dictionary<AnimationType, float> effectTypeDelays = new();
    private readonly Dictionary<AnimationType, List<CharaController>> effectUsersByType = new();
    private int nextEffectTypeIndex;


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
        BattleActionTimeline.instance.Schedule(()=> PlayAnimation(target, animationType, user, playLongDamageAnimation), additionalDelay + GetEffectTypeDelay(user, animationType));
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

        // Particleの終了は行動完了条件に含めない
        return UniTask.CompletedTask;
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

    /// <summary>
    /// エフェクトの種類ごとに0.1秒エフェクト再生を遅らせる
    /// </summary>
    /// <param name="animationType"></param>
    /// <returns></returns>
    private float GetEffectTypeDelay(CharaController user, AnimationType animationType)
    {
        if (animationType is not
            (AnimationType.Heal or AnimationType.ReceiveBuff or AnimationType.ReceiveDebuff))
            return 0f;

        // すでに同じAnimationTypeが存在している場合、前に割り当てた遅延を利用。見つからなかった場合、新しく遅延を計算して登録
        if (!effectTypeDelays.TryGetValue(animationType, out float typeDelay))
        {
            typeDelay = Mathf.Min(nextEffectTypeIndex * EFFECT_INTERVAL, MAX_EFFECT_DELAY);
            effectTypeDelays.Add(animationType, typeDelay);

            nextEffectTypeIndex++;
        }

        if (user == null)
            return typeDelay;  // TODO この処理により、userが指定されていない場合(ターン開始時バフなど)はtypeDelayだけを利用するように制御。以下user!= null部分を削除？

        if (!effectUsersByType.TryGetValue(animationType, out var users))
        {   
            // 同じAnimationTypeが存在しない場合、Dicに新しく要素を追加
            users = new List<CharaController>();
            effectUsersByType.Add(animationType, users);
        }

        if (!users.Contains(user)) users.Add(user);

        // 付与者が異なるごとに0.1秒追加
        float userDelay = users.IndexOf(user) * EFFECT_INTERVAL;

        return Mathf.Min(typeDelay + userDelay, MAX_EFFECT_DELAY);
    }

    public void ResetEffectTypeDelays()
    {
        effectTypeDelays.Clear();
        nextEffectTypeIndex = 0;
    }
}
