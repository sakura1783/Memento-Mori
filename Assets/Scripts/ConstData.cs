/// <summary>
/// 定数管理用クラス
/// </summary>
public static class ConstData
{
    // TODO ランク(SRやSSRなど)によって変化させる
    public static float POWER_INC_RATE = 0.025064f;  // 通常の戦闘力増加率
    public static float POWER_INC_RATE_BREAK_LIMIT = 0.3f; // 限界突破時の戦闘力増加率

    public static float ATTACK_MODIFIER = 1.7f;  // 攻撃力補正値
    public static float DEFENCE_MODIFIRE = 2f;
    public static float HP_MODIFIRE = 0.08f;

    public static float ATTRIBUTE_BONUS = 0.25f;  // 属性相性がいい場合、ダメージを25%増加
    public static float CRITICAL_BONUS = 0.5f;  // クリティカル発生時、ダメージを50%増加
}
