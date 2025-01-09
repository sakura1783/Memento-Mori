using System.Collections.Generic;


/// <summary>
/// 定数管理用クラス
/// 静的クラスのため、非静的メンバは宣言できない。(constは暗黙的にstaticとなる)
/// </summary>
public static class ConstData
{
    // TODO ランク(SRやSSRなど)によって変化させる
    public const float POWER_INC_RATE = 0.025064f;  // 通常の戦闘力増加率
    public const float POWER_INC_RATE_BREAK_LIMIT = 0.3f; // 限界突破時の戦闘力増加率

    public const float ATTACK_MODIFIER = 1.7f;  // 攻撃力補正値
    public const float DEFENCE_MODIFIRE = 2f;
    public const float HP_MODIFIRE = 0.08f;

    public const float ATTRIBUTE_BONUS = 0.25f;  // 属性相性がいい場合、ダメージを25%増加
    public const float CRITICAL_BONUS = 0.5f;  // クリティカル発生時、ダメージを50%増加

    public static readonly Dictionary<Attribute, Attribute> ATTRIBUTE_RELATIONSHIP = new()  // 属性相性  // 組み込み型以外の型はconstで宣言できないし、静的クラスで非静的メンバは宣言できないので、static readonlyを使用。
    {
        {Attribute.藍, Attribute.紅},  // KeyはValueに対して有利
        {Attribute.紅, Attribute.翠},
        {Attribute.翠, Attribute.黄},
        {Attribute.黄, Attribute.藍},

        {Attribute.天, Attribute.冥},
        {Attribute.冥, Attribute.天},
    };
}
