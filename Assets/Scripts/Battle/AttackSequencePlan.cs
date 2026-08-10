/// <summary>
/// 攻撃に伴う演出の仕様を管理(遅延時間、軌跡エフェクト表示の有無、長/短尺どちらのダメージアニメーションを行うか)
/// </summary>
public readonly struct AttackSequencePlan
{
    public float TrajectoryDelay { get; }
    public float HitDelay { get; }

    public bool PlayTrajectory { get; }
    public bool PlayLongDamageAnimation { get; }

    public AttackSequencePlan(float trajectoryDelay, float hitDelay, bool playTrajectory, bool playLongDamageAnimation)
    {
        TrajectoryDelay = trajectoryDelay;
        HitDelay = hitDelay;
        PlayTrajectory = playTrajectory;
        PlayLongDamageAnimation = playLongDamageAnimation;
    }
}
