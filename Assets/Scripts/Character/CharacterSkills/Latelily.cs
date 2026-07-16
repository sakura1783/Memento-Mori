using System;
using System.Linq;
using UnityEngine;

public class Latelily : CharacterBase
{
    public override int Active1CoolTime => 4;
    public override int Active2CoolTime => 4;

    public override PassiveSkillConfig Passive1Config { get; } = new(0, 1, PassiveReactivationBasis.Turn, 4, 100, PassiveActivationTiming.TurnStart);


    /// <summary>
    /// 攻撃力が最も高い敵に4回攻撃力*200%の攻撃
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill1(CharaController user)
    {
        var target = SkillManager.PickTarget(user, TargetType.Opponent, 1, ValueType.ByAttackPower, true).FirstOrDefault();

        for (int i = 0; i < 4; i++)
            SkillManager.Attack(user, target, user.Status.attackPower, 200, AttackPattern.Focused, i, 4);
    }

    /// <summary>
    /// HP割合が最も低い敵に5回攻撃力*160%の攻撃  // TODO この攻撃は必ず命中する
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill2(CharaController user)
    {
        var target = SkillManager.PickTarget(user, TargetType.Opponent, 1, ValueType.ByCurrentHpRate, false).FirstOrDefault();

        for (int i = 0; i < 5; i++)
            SkillManager.Attack(user, target, user.Status.attackPower, 160, AttackPattern.Focused, i, 5);
    }

    /// <summary>
    /// ターン開始時1ターンの間、攻撃力が最も高い味方の攻撃力を自身の攻撃力*20%増加させる。このスキルは4ターンに1回発動する。
    /// </summary>
    /// <param name="user"></param>
    public override async void PassiveSkill1(CharaController user)
    {
        var target = SkillManager.PickTarget(user, TargetType.Ally, 1, ValueType.ByAttackPower, true).FirstOrDefault();
        int increaseValue = SkillManager.ModifyAttackPower(target, user.Status.attackPower, 20, true);

        await SkillManager.WaitTurnsAsync(1);

        if (target != null)
            target.Status.attackPower -= increaseValue;
    }

    /// <summary>
    /// 自身のHP割合が50%未満で、自身が最大HP*30%を超えるダメージの攻撃を受けた場合、超えた分のダメージを100%遮断する
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill2(CharaController user)
    {
        user.SetInComingDamageModifier(damage =>
        {
            var hpRate = (float)user.Status.Hp.Value / user.Status.MaxHp.Value;

            if (hpRate >= 0.5)
                return damage;

            var limit = CalculateManager.CalculateValueByRate(user.Status.MaxHp.Value, 15);
            return Mathf.Min(damage, limit);
        });
    }
}