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

    /// <summary>
    /// usingで使うクラス。IDisposableを実装する。
    /// </summary>
    private class AnimationTimingScope : IDisposable
    {
        private readonly Action onDispose;

        /// コンストラクタ
        public AnimationTimingScope(Action action)
        {
            onDispose = action;
        }


        /// <summary>
        /// usingの{}内の処理を終えた際に自動的に動く処理。
        /// (値のセット->何らかの処理->値のリセット、などと単純に処理を回すより安全な方法)
        /// </summary>
        public void Dispose()
        {
            onDispose?.Invoke();
        }
    }

    [SerializeField] private BattleManager battleManager;

    [SerializeField] private RectTransform effectRoot;  // 軌跡エフェクトをこれの子として生成する

    [SerializeField] private List<EffectObjData> effects = new();  // AnimationType順に順番にプレハブを入れる
    [SerializeField] private ParticleSystem trajectoryEffect;

    private List<UniTask> animationTasks = new();

    public float CurrentEffectDelay { get; private set; }  // 現在利用する遅延

    public const float TRAJECTORY_DURATION = 0.2f;
    public const float SHORT_HIT_DURATION = 0.17f;
    public const float LONG_HIT_DURATION = 0.3f;


    /// <summary>
    /// 軌跡エフェクト、ダメージアニメーション以外はここからアニメーション処理を行う
    /// </summary>
    /// <param name="target"></param>
    /// <param name="animationType"></param>
    /// <param name="delay">省略した場合(=中身がnull)、現在のスコープの遅延を使用し、値を指定した場合、その分だけ遅延する</param>
    /// <param name="user"></param>
    /// <param name="playLongDamageAnimation"></param>
    public void AddAnimation(CharaController target, AnimationType animationType, float? delay = null, CharaController user = null, bool playLongDamageAnimation = true)
    {
        float actualDelay = delay ?? CurrentEffectDelay;
        animationTasks.Add(PlayAnimation(target, animationType, actualDelay, user, playLongDamageAnimation));
    }

    /// <summary>
    /// 軌跡エフェクトはここからアニメーション処理を行う
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <param name="attackPattern"></param>
    /// <param name="hitIndex"></param>
    public void AddTrajectoryAnimation(CharaController attacker, CharaController target, AttackPattern attackPattern, int hitIndex)
    {
        bool playTrajectory = attackPattern switch
        {
            AttackPattern.Basic => true,
            AttackPattern.Focused => hitIndex == 0,
            AttackPattern.Random => true,
            AttackPattern.Simultaneous => true,
            _ => true
        };

        if (!playTrajectory)
            return;

        AddAnimation(target, AnimationType.Trajectory, GetTrajectoryDelay(attackPattern, hitIndex), attacker);
    }

    /// <summary>
    /// ダメージアニメーション->ダメージエフェクト用。まとめてここから処理を実行
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attackPattern"></param>
    /// <param name="hitIndex"></param>
    /// <param name="hitCount"></param>
    public void AddHitAnimation(CharaController target, AttackPattern attackPattern, int hitIndex = 0, int hitCount = 1)
    {
        float delay = GetHitDelay(attackPattern, hitIndex);

        bool playLongDamageAnimation = attackPattern switch
        {
            AttackPattern.Basic => true,
            AttackPattern.Focused => hitIndex == hitCount - 1,
            AttackPattern.Random => true,
            AttackPattern.Simultaneous => true,
            _ => true
        };

        AddAnimation(target, AnimationType.Damage, delay, playLongDamageAnimation: playLongDamageAnimation);
        AddAnimation(target, AnimationType.DefaultHit, delay);
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

    private async UniTask PlayAnimation(CharaController target, AnimationType animationType, float delay = 0, CharaController user = null, bool isLongDamageAnimation = true)
    {
        if (delay > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        
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
        
        float duration = isLongAnimation ? LONG_HIT_DURATION : SHORT_HIT_DURATION;
        int vibrato = isLongAnimation ? 13 : 5;
        
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
            .DOMove(targetRect.position, TRAJECTORY_DURATION).SetEase(Ease.Linear).ToUniTask();  // DOMove()にはワールド座標を指定する必要がある

        // TODO
        // effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // float distance = Vector3.Distance(attackerRect.position, targetRect.position);
        // Debug.Log($"Trajectory Distance : {distance}");
    }

    public static float GetHitDelay(AttackPattern attackPattern, int hitIndex)  // クラスインスタンスの状態を何も利用していないため、staticに。
    {
        return attackPattern switch
        {
            AttackPattern.Basic => TRAJECTORY_DURATION,
            AttackPattern.Focused => TRAJECTORY_DURATION + (SHORT_HIT_DURATION * hitIndex),
            AttackPattern.Random => TRAJECTORY_DURATION + (TRAJECTORY_DURATION + LONG_HIT_DURATION) * hitIndex,
            AttackPattern.Simultaneous => TRAJECTORY_DURATION,
            _ => 0f
        };
    }

    public static float GetTrajectoryDelay(AttackPattern attackPattern, int hitIndex)
    {
        return attackPattern switch
        {
            AttackPattern.Basic => 0f,
            AttackPattern.Focused => 0f,
            AttackPattern.Random => hitIndex * (TRAJECTORY_DURATION + LONG_HIT_DURATION),
            AttackPattern.Simultaneous => 0f,
            _ => 0f
        };
    }

    /// <summary>
    /// using(UseHitTiming()){}で、中括弧内に書いたアニメーションを全て同じタイミングで実行する
    /// </summary>
    /// <param name="attackPattern"></param>
    /// <param name="hitIndex"></param>
    /// <returns></returns>
    public IDisposable UseHitTiming(AttackPattern attackPattern, int hitIndex)
    {
        float previousDelay = CurrentEffectDelay;
        
        CurrentEffectDelay = GetHitDelay(attackPattern, hitIndex);
        return new AnimationTimingScope(() => CurrentEffectDelay = previousDelay);
    }
}
