public class PassiveSkillState
{
    public bool isDisabled;
    public int remainingDuration;
    public int remainingCountForReactivation;  // 再発動のために必要な、残り行動回数またはターン数
    public int remainingActivationCount;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="config"></param>
    public PassiveSkillState(PassiveSkillConfig config)
    {
        remainingDuration = 0;
        remainingCountForReactivation = 0;
        remainingActivationCount = config.maxActivationCount;
        isDisabled = false;
    }
}
