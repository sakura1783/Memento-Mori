using System;


/// <summary>
/// ヒットがどこに位置するか
/// </summary>
public readonly struct HitSequencePosition
{
    public int Index { get; }
    public int Count { get; }

    public bool IsFirst => Index == 0;
    public bool IsLast => Index == Count - 1;

    public static readonly HitSequencePosition Single = new(0, 1);

    public HitSequencePosition(int index, int count)
    {
        // 不適切な値が設定された際に例外を投げる
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));  // nameof(count)は"count"と書くのと同じ
        if (index < 0 || index >= count)
            throw new ArgumentOutOfRangeException(nameof(index));

        Index = index;
        Count = count;
    }
}
