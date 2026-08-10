using System;

/// <summary>
/// 攻撃に伴うアニメーションの仕様を作成
/// </summary>
public static class AttackSequencePlanBuilder
{
    private const float TRAJECTORY_DURATION = 0.2f;
    private const float SHORT_HIT_DURATION = 0.17f;
    private const float LONG_HIT_DURATION = 0.3f;


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
        return new AttackSequencePlan(0f, TRAJECTORY_DURATION, true, true);
    }

    private static AttackSequencePlan CreateFocusedPlan(HitSequencePosition hit)
    {
        return new AttackSequencePlan(
            0f, 
            TRAJECTORY_DURATION + (SHORT_HIT_DURATION * hit.Index),
            hit.isFirst,
            hit.isLast);
    }

    private static AttackSequencePlan CreateRandomPlan(HitSequencePosition hit)
    {
        return new AttackSequencePlan(
            hit.Index * (TRAJECTORY_DURATION + LONG_HIT_DURATION),
            TRAJECTORY_DURATION + (hit.Index * (TRAJECTORY_DURATION + LONG_HIT_DURATION)),
            true,
            true);
    }
}
