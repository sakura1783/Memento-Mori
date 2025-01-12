using System.Linq;
using UnityEngine;

public class Setsuna : CharacterBase
{
    public override int Active1CoolTime => 5;
    public override int Active2CoolTime => 4;


    /// <summary>
    /// ランダムな敵に3回攻撃力*400%の攻撃。クリティカルヒットした場合、1ターンの間自身の攻撃力が30%増加する。クリティカルヒットするたび攻撃力増加のターン数が1ターン多くなる。
    /// </summary>
    /// <param name="user"></param>
    public override async void ActiveSkill1(CharaController user)
    {
        int criticalCount = 0;

        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 3);  // TODO ターゲットは重複あり
        targets.ForEach(target =>
        {
            SkillManager.Attack(user, target, user.Status.attackPower, 400);
            
            if (target.ReceivedCriticalDamage)
            {
                target.ReceivedCriticalDamage = false;
                criticalCount++;
            }
        });

        int increaseValue = 0;
        if (criticalCount >= 0) increaseValue = SkillManager.ModifyAttackPower(user, user.Status.attackPower, 30, true);

        await SkillManager.WaitTurnsAsync(criticalCount);

        if (user != null) user.Status.attackPower -= increaseValue;
    }

    /// <summary>
    /// ランダムな敵4体に攻撃力*350%の攻撃。このスキルで敵を戦闘不能にした場合、HP割合が最も低い敵に攻撃力*480%の攻撃を行う。敵を戦闘不能にするたび、追加攻撃の回数が1回多くなる(最大4回まで)
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill2(CharaController user)
    {
        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 4);
        targets.ForEach(target => SkillManager.Attack(user, target, user.Status.attackPower, 350));
        
        // 追加攻撃
        int additionalAttackCount = 0;
        if (targets.Any(target => target.Status.Hp.Value <= 0)) additionalAttackCount++;

        while (additionalAttackCount > 0)
        {
            var additionalTargets = SkillManager.PickTarget(user, TargetType.Opponent, 1, ValueType.ByCurrentHp, false);
            additionalTargets.ForEach(target =>
            {
                SkillManager.Attack(user, target, user.Status.attackPower, 480);
                Mathf.Clamp(additionalAttackCount--, 0, 4);  // 追加攻撃の回数を1減少

                // 戦闘不能にするたび、追加で攻撃
                if (target.Status.Hp.Value <= 0) Mathf.Clamp(additionalAttackCount++, 0, 4);
            });
        }
    }

    /// <summary>
    /// 1ターン目のターン開始時、自身に3ターンの間攻撃力*250%の「シールド」を付与
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill1(CharaController user)
    {
        SkillManager.AddBuff(user, BuffType.シールド, true, false, 3, effectValue: CalculateManager.CalculateSkillEffectValue(user.Status.attackPower, 250));
    }
}
