public class PassiveSkillState
{
    public bool isDisabled;
    public int remainingDuration;
    public int remainingActionCount;
    public int remainingActivationCount;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="config"></param>
    public PassiveSkillState(PassiveSkillConfig config)
    {
        remainingDuration = config.duration;
        remainingActionCount = config.requiredActionsForReactivation;
        remainingActivationCount = config.maxActivationCount;
        isDisabled = false;
    }
}
