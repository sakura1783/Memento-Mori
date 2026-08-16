// TODO 後日実装？一応用意しておく

/// <summary>
/// HitResultクラスが1回のヒット結果を返すのに対し、こちらはRandomAttackやFocusedAttackなどで、複数回ヒットの攻撃結果を返す
/// </summary>
public readonly struct HitSequenceResult
{
    public int TotalDamage { get; }
    public int CriticalCount { get; }
    public int KillCount { get; }

    public HitSequenceResult(int totalDamage, int criticalCount, int killCount)
    {
        TotalDamage = totalDamage;
        CriticalCount = criticalCount;
        KillCount = killCount;
    }
}
