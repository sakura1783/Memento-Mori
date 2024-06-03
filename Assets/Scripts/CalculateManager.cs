using System;
using UnityEngine;

public class CalculateManager : AbstractSingleton<CalculateManager>
{
    // TODO ConstDataクラスに移動？
    private const float ATTACK_MODIFIER = 1.8f;  // 攻撃力補正値
    private const float DEFENCE_MODIFIRE = 2f;  // 防御力補正値

    private const float ATTRIBUTE_BONUS = 0.25f;  // 属性相性がいい場合、ダメージを25%増加
    private const float CRITICAL_BONUS = 0.5f;  // クリティカル発生時、ダメージを50%増加

    // テスト
    [SerializeField] private GSSReceiver gssReceiver;


    // テスト
    private async void Start()
    {
        //Debug.Log(CalculateDamage(17000, 370, 11000));

        await gssReceiver.PrepareGSSLoadStartAsync();
    }

    /// <summary>
    /// ダメージ計算
    /// </summary>
    /// <param name="attackPower"></param>
    /// <param name="damagePercentage">攻撃力の何%分のダメージを与えるか</param>
    /// <param name="enemyDefencePower">敵の防御力</param>
    /// <returns></returns>
    public int CalculateDamage(int attackPower, float damagePercentage, int enemyDefencePower)
    {
        int damage;

        // 通常(攻撃力*技/補正値)-(敵の防御力/補正値)
        damage = (int)Math.Round(attackPower * (damagePercentage / 100) / ATTACK_MODIFIER - (enemyDefencePower / DEFENCE_MODIFIRE), 0, MidpointRounding.AwayFromZero);  // 少数第一位を四捨五入

        // TODO クリティカルボーナス、属性ボーナス

        return damage;
    }

    /// <summary>
    /// TODO レベルアップ後の各ステータスの計算
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <param name="targetLevel"></param>
    /// <returns></returns>
    public int CalculateCharaStatus(int currentLevel, int targetLevel)
    {
        for (int i = currentLevel; i < targetLevel; i++)
        {
            if (i % 20 == 0)
            {
                // Lv20→21(40→41 etc...)の時だけ、限界突破として、現在の戦闘力の30%を足す
                //combatPower += combatPower * 0.3f;
            }
            else
            {
                // 通常は現在の戦闘力の2.5064%を足す
                //combatPower += combatPower * 0.025064f;
            }
        }

        // TODO 適切な値を返す
        return -1;
    }
}
