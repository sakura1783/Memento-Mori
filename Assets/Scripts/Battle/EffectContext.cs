public enum EffectTrigger
{
    Applied,
    AutoTriggered,
}

public readonly struct EffectContext
{
    public EffectTrigger Trigger { get; }
    public CharaController Source { get; }

    private EffectContext(EffectTrigger trigger, CharaController source)
    {
        Trigger = trigger;
        Source = source;
    }


    // EffectContextを新しく作成し、返すメソッド。staticにして、クラスインスタンスを作らなくても呼び出し可能。
    public static EffectContext AppliedBy(CharaController source)
        => new EffectContext(EffectTrigger.Applied, source);

    public static EffectContext AutoTriggered()
        => new EffectContext(EffectTrigger.AutoTriggered, null);
}
