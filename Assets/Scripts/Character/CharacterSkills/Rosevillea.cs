/// <summary>
/// ローゼヴィリアのスキル
/// 各スキルメソッドには、SkillManagerのメソッドを組み合わせて処理を作る
/// </summary>
public class Rosevillea : CharacterBase
{
    public override int Active1CoolTime => 2;  // 各キャラ各スキルで個別の値を設定
    public override int Active2CoolTime => 3;

    public override PassiveSkillConfig Passive2Config { get; } = new(0, 1, 4, 100, PassiveActivationTiming.TurnStart);


    /// <summary>
    /// ランダムな敵1体に攻撃力*390%の攻撃。さらに、2ターンの間「気絶」を付与
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill1(CharaController user)
    {
        // ターゲットを取得(戻り値はList<CharaController>型)    
        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 1);

        // スキル処理
        targets.ForEach(target =>
        {
            SkillManager.Attack(user, target, user.Status.attackPower, 390);
            SkillManager.AddBuff(target, BuffType.気絶, false, false, 2);
        });
    }

    /// <summary>
    /// ランダムな敵1体に攻撃力*420%の攻撃。
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill2(CharaController user)
    {
        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 1);

        targets.ForEach(target => SkillManager.Attack(user, target, user.Status.attackPower, 420));
    }

    /// <summary>
    /// 自身の攻撃力が20%増加する(解除不可)。
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill1(CharaController user)
    {
        SkillManager.ModifyAttackPower(user, user.Status.attackPower, 20, true);
    }

    /// <summary>
    /// 自身のHP割合が60%以上のとき、Passive2を通す
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public override bool MeetsPassive2ActivationCondition(CharaController user)
    {
        if ((float)user.Status.Hp.Value / user.Status.MaxHp.Value >= 0.6)
            return true;
            
        return false;
    }

    /// <summary>
    /// ターン開始時、自身のHP割合が60%以上の場合、自身に現在HP*15%の自傷ダメージを与え、自身の攻撃力を1ターンの間20%増加する。// TODO このスキルは4ターンに1度発動する。
    /// </summary>
    /// <param name="user"></param>
    public override async void PassiveSkill2(CharaController user)
    {
        user.UpdateHp(-CalculateManager.CalculateValueByRate(user.Status.Hp.Value, 15));

        int increaseValue = SkillManager.ModifyAttackPower(user, user.Status.attackPower, 20, true);
        await SkillManager.WaitTurnsAsync(1);
        user.Status.attackPower -= increaseValue;
    }
}
