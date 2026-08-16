public readonly struct HitResult
{
    public int Damage { get; }
    public bool IsCritical { get; }
    public bool WasBlocked { get; }

    public HitResult(int damage, bool isCritical, bool wasBlocked)
    {
        Damage = damage;
        IsCritical = isCritical;
        WasBlocked = wasBlocked;
    }
}
