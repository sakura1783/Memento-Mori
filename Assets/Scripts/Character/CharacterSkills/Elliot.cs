public class Elliot : CharacterBase
{
    public override int Active1CoolTime => 0;
    public override int Active2CoolTime => 0;


    /// <summary>
    /// 2ターンの間、味方全体の攻撃力を自身の攻撃力*15%増加
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill1(CharaController user)
    {
        var targets = SkillManager.PickTarget(user, TargetType.Ally);
        targets.ForEach(target => SkillManager.ModifyAttackPower(target, user.Status.attackPower, 15, true));

        // TODO 2ターンの間。UniTaskでターン経過するまで待機する？ →元の値を保持、ターン経過後、元の値に戻す
    }

    /// <summary>
    /// ランダムな敵3体に攻撃力*200%の攻撃。さらに、HP割合が低い味方2体のHPを自身の攻撃力*50%回復し、1ターンの間「再生」を付与。毎ターン行動開始時にHPを自身の最大HP*3%回復。
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill2(CharaController user)
    {
        var attackTargets = SkillManager.PickTarget(user, TargetType.Opponent, 3);
        attackTargets.ForEach(target => SkillManager.Attack(user, target, user.Status.attackPower, 200));

        var healTargets = SkillManager.PickTarget(user, TargetType.Ally, 2);  // TODO HP割合が低い味方をターゲットにする
        healTargets.ForEach(target =>
        {
            SkillManager.Heal(target, user.Status.attackPower, 50);
            SkillManager.AddBuff(target, BuffType.再生, 1, 3);
        });
    }

    /// <summary>
    /// 自身の最大HPが20%増加する(解除不可)。
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill1(CharaController user)
    {
        SkillManager.IncreaseMaxHp(user, 20);
    }

    /// <summary>
    /// クリティカルヒットを受けた場合、1ターンの間攻撃してきた敵の攻撃力を15%減少させる
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill2(CharaController user)
    {
        if (user.ReceivedCriticalDamage)
        {
            // TODO 直前に行動した相手をPickTargetで取得できないか。BattleManager内に、CharaController型で直前に行動したキャラを保持
        }
    }
}
