using System;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

public class CalculateManager : AbstractSingleton<CalculateManager>
{
    // TODO ConstDataクラスに移動？
    private const float ATTACK_MODIFIER = 1.7f;  // 攻撃力補正値
    private const float DEFENCE_MODIFIRE = 2f;
    private const float HP_MODIFIRE = 0.08f;


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
    /// レベルアップ後の各ステータスの計算
    /// </summary>
    /// <param name="charaData"></param>
    /// <param name="targetLevel"></param>
    /// <returns></returns>
    public void CalculateCharaStatus(GameData.OwnedCharaData charaData, int targetLevel)
    {
        for (int i = charaData.level; i < targetLevel; i++)
        {
            // TODO キャラのランクによってステータス上昇率を変更する

            // 限界突破(Lv20→21,40→41 etc...)
            if (i % 20 == 0)
            {
                // 攻撃力、防御力、HPの30%を足す
                charaData.attackPower += (int)Math.Round(charaData.attackPower * 0.3f, 0, MidpointRounding.AwayFromZero);
                charaData.defencePower += (int)Math.Round(charaData.defencePower * 0.3f, 0, MidpointRounding.AwayFromZero);
                charaData.hp += (int)Math.Round(charaData.hp * 0.3f, 0, MidpointRounding.AwayFromZero);

                // クリティカル率は限界突破の際に1%ずつ上昇
                charaData.criticalRate += 0.01f;
            }
            else
            {
                // 通常は現在の各ステータスの2.5%を足す
                charaData.attackPower += (int)Math.Round(charaData.attackPower * 0.025f, 0, MidpointRounding.AwayFromZero);
                charaData.defencePower += (int)Math.Round(charaData.defencePower * 0.025f, 0, MidpointRounding.AwayFromZero);
                charaData.hp += (int)Math.Round(charaData.hp * 0.025f, 0, MidpointRounding.AwayFromZero);
            }
        }

        // 上で求めた各ステータスから、戦闘力を計算。
        // 戦闘力 = 攻撃力+防御力+補正後HP+補正後クリティカル率
        charaData.combatPower = (int)Math.Round(charaData.attackPower + charaData.defencePower + (charaData.hp * HP_MODIFIRE) + (charaData.attackPower + charaData.attackPower * 0.5) / ATTACK_MODIFIER * charaData.criticalRate, 0, MidpointRounding.AwayFromZero);
    }
}
