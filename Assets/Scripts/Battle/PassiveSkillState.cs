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
        remainingDuration = 0;
        remainingActionCount = 0;
        remainingActivationCount = config.maxActivationCount;
        isDisabled = false;
    }
}
