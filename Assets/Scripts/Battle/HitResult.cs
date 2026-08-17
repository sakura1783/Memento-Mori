public readonly struct HitResult
{
    public int Damage { get; }
    public bool IsCritical { get; }
    public bool WasBlocked { get; }
    public bool DefeatedTarget { get; }

    public HitResult(int damage, bool isCritical, bool wasBlocked, bool defeatedTarget)
    {
        Damage = damage;
        IsCritical = isCritical;
        WasBlocked = wasBlocked;
        DefeatedTarget = defeatedTarget;
    }
}
