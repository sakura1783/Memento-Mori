public class PassiveSkillConfig
{
    public int startTurn;  // 何ターン目から発動するか
    public int duration;
    public int requiredActionsForReactivation;  // 何回行動終了したら再発動するか


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="startTurn"></param>
    /// <param name="duration"></param>
    /// <param name="requiredActionsForReactivation"></param>
    public PassiveSkillConfig(int startTurn, int duration, int requiredActionsForReactivation)
    {
        this.startTurn = startTurn;
        this.duration = duration;
        this.requiredActionsForReactivation = requiredActionsForReactivation;
    }
}
