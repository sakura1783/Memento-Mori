/// <summary>
/// 攻撃に伴う一連の演出処理を登録
/// </summary>
public static class AttackSequenceScheduler
{
    public static void Schedule(CharaController attacker, CharaController target, AttackSequencePlan plan, int displayedHp)
    {
        var animationManager = BattleAnimationManager.instance;
        
        // 軌跡エフェクト再生
        if (plan.PlayTrajectory)
            animationManager.AddAnimation(target, AnimationType.Trajectory, plan.TrajectoryDelay, attacker);

        // ダメージアニメーションとダメージエフェクトの再生
        animationManager.AddAnimation(target, AnimationType.Damage, plan.HitDelay, playLongDamageAnimation: plan.PlayLongDamageAnimation);
        animationManager.AddAnimation(target, AnimationType.DefaultHit, plan.HitDelay);

        // HP表示の更新
        target.SetDisplayedHp(displayedHp, plan.HitDelay);
    }
}
