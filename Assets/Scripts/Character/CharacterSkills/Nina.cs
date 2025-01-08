public class Nina : CharacterBase
{
    public override int Active1CoolTime => 4;
    public override int Active2CoolTime => 4;


    /// <summary>
    /// ランダムな敵4体に攻撃力*290%の攻撃。さらに、自身のHPをこのスキルの総与ダメージ*10%回復
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill1(CharaController user)
    {
        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 4);
        int totalDamage = 0;

        targets.ForEach(target => totalDamage += SkillManager.Attack(target, user.Status.attackPower, 290));

        SkillManager.Heal(user, totalDamage, 10);
    }

    /// <summary>
    /// ランダムな敵2体に攻撃力*420%の攻撃。さらに、自身に1ターンの間「ダメージ無効」を付与
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill2(CharaController user)
    {
        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 2);
        targets.ForEach(target => SkillManager.Attack(target, user.Status.attackPower, 420));
    
        SkillManager.AddBuff(user, BuffType.ダメージ無効, 1);
    }

    /// <summary>
    /// 自身に「再生」を付与。毎ターン行動開始時にHPを最大HP*5%回復(解除不可)。
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill1(CharaController user)
    {   
        SkillManager.AddBuff(user, BuffType.再生, effectRate: 5);  // 引数:値とすることで、必要な引数のみ指定することができる
    }
}
