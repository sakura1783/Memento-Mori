public enum PassiveActivationTiming
{
    BattleStart,
    TurnStart,
}

/// <summary>
/// 該当パッシブの仕様を管理するクラスS
/// </summary>
public class PassiveSkillConfig
{
    public int startTurn;  // 何ターン目から発動するか
    public int duration;  // ※1回の発動で何ターン続くか
    public int requiredActionsForReactivation;  // 何回行動終了したら再発動するか
    public int maxActivationCount;  // ※最大何回発動できるか
    public PassiveActivationTiming activationTiming;


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="startTurn"></param>
    /// <param name="duration"></param>
    /// <param name="requiredActionsForReactivation"></param>
    public PassiveSkillConfig(int startTurn, int duration, int requiredActionsForReactivation, int maxActivationCount, PassiveActivationTiming activationTiming)
    {
        this.startTurn = startTurn;
        this.duration = duration;
        this.requiredActionsForReactivation = requiredActionsForReactivation;
        this.maxActivationCount = maxActivationCount;
        this.activationTiming = activationTiming;
    }
}
