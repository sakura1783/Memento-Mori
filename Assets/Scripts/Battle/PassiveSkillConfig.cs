public enum PassiveActivationTiming
{
    BattleStart,
    TurnStart,
}

public enum PassiveReactivationBasis
{
    None,
    Turn,  // 指定ターン経過で再発動
    Action,  // 指定回数行動終了で再発動
}

/// <summary>
/// 該当パッシブの仕様を管理するクラスS
/// </summary>
public class PassiveSkillConfig
{
    public int startTurn;  // 何ターン目から発動するか
    public int duration;  // ※1回の発動で何ターン続くか
    public PassiveReactivationBasis reactivationBasis;
    public int requiredCountForReactivation;  // 再発動に必要なターン数または行動回数を指定
    public int maxActivationCount;  // ※最大何回発動できるか
    public PassiveActivationTiming activationTiming;


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="startTurn"></param>
    /// <param name="duration"></param>
    /// <param name="requiredCountForReactivation"></param>
    public PassiveSkillConfig(int startTurn, int duration, PassiveReactivationBasis reactivationBasis, int requiredCountForReactivation, int maxActivationCount, PassiveActivationTiming activationTiming)
    {
        this.startTurn = startTurn;
        this.duration = duration;
        this.reactivationBasis = reactivationBasis;
        this.requiredCountForReactivation = requiredCountForReactivation;
        this.maxActivationCount = maxActivationCount;
        this.activationTiming = activationTiming;
    }
}
