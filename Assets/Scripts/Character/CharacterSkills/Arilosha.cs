using System;

public class Arilosha : CharacterBase
{
    public override int Active1CoolTime => 4;
    public override int Active2CoolTime => 4;


    /// <summary>
    /// 通常攻撃時、ランダムな敵1体に攻撃力*220%の攻撃を行う(パッシブ1効果)
    /// </summary>
    /// <param name="user"></param>
    public override void BasicAttack(CharaController user)
    {
        base.BasicAttack(user);

        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 1);
        targets.ForEach(target => SkillManager.Attack(user, target, user.Status.attackPower, 220));
    }

    /// <summary>
    /// 自身に現在HP*10%の自傷ダメージを与え、現在HPが最も低い敵に攻撃力*480%の物理攻撃を行う
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill1(CharaController user)
    {
        user.UpdateHp(CalculateManager.CalculateSkillEffectValue(user.Status.Hp.Value, 10));

        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 1, ValueType.ByCurrentHp, false);
        targets.ForEach(target => SkillManager.Attack(user, target, user.Status.attackPower, 480));
    }

    /// <summary>
    /// 自身に現在HP*10%の自傷ダメージを与え、4ターンの間攻撃力が自身のHP割合*10%分増加する
    /// </summary>
    /// <param name="user"></param>
    public override async void ActiveSkill2(CharaController user)
    {
        user.UpdateHp(CalculateManager.CalculateSkillEffectValue(user.Status.Hp.Value, 10));

        // HP割合*10% = HP(%)
        int hpPercentage = (int)Math.Round((float)user.Status.Hp.Value / user.Status.MaxHp.Value, 0, MidpointRounding.AwayFromZero);
        var increaseValue = SkillManager.ModifyAttackPower(user, hpPercentage, 100, true);

        await SkillManager.WaitTurnsAsync(4);

        if (user != null) user.Status.attackPower -= increaseValue;
    }
}
