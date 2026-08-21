using System;

/// <summary>
/// 攻撃に伴うアニメーションの仕様を作成
/// </summary>
public static class AttackSequencePlanBuilder
{
    public const float TRAJECTORY_DURATION = 0.12f;
    public const float SHORT_HIT_DURATION = 0.10f;
    public const float LONG_HIT_DURATION = 0.15f;


    public static AttackSequencePlan Build(AttackPattern attackPattern, HitSequencePosition hit)
    {
        return attackPattern switch
        {
            AttackPattern.Single 
            or AttackPattern.Simultaneous => CreateBasicPlan(),
            AttackPattern.Focused => CreateFocusedPlan(hit),
            AttackPattern.Random => CreateRandomPlan(hit),

            _ => throw new ArgumentOutOfRangeException(nameof(attackPattern))
        };
    }

    private static AttackSequencePlan CreateBasicPlan()
    {
        return new AttackSequencePlan(
            0f, 
            TRAJECTORY_DURATION, 
            TRAJECTORY_DURATION + LONG_HIT_DURATION, 
            true, 
            true);
    }

    private static AttackSequencePlan CreateFocusedPlan(HitSequencePosition hit)
    {
        float hitDelay = TRAJECTORY_DURATION + (SHORT_HIT_DURATION * hit.Index);
        float lastHitDuration = hit.IsLast ? LONG_HIT_DURATION : SHORT_HIT_DURATION;

        return new AttackSequencePlan(
            0f, 
            hitDelay,
            hitDelay + lastHitDuration,
            hit.IsFirst,
            hit.IsLast);
    }

    private static AttackSequencePlan CreateRandomPlan(HitSequencePosition hit)
    {
        float hitDelay = TRAJECTORY_DURATION + (hit.Index * SHORT_HIT_DURATION);

        return new AttackSequencePlan(
            0f,
            hitDelay,
            hitDelay + SHORT_HIT_DURATION,
            hit.IsFirst,
            false);
    }
}
