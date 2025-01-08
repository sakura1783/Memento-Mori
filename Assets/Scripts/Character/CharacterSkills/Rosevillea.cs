/// <summary>
/// ローゼヴィリアのスキル
/// 各スキルメソッドには、SkillManagerのメソッドを組み合わせて処理を作る
/// </summary>
public class Rosevillea : CharacterBase
{
    public override int Active1CoolTime => 2;  // 各キャラ各スキルで個別の値を設定
    public override int Active2CoolTime => 3;

    // TODO 必要であればパッシブのクールタイムも追加


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
            SkillManager.Attack(target, user.Status.attackPower, 390);
            SkillManager.AddBuff(target, BuffType.気絶, 2);
        });
    }

    /// <summary>
    /// ランダムな敵1体に攻撃力*420%の攻撃。
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill2(CharaController user)
    {
        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 1);

        targets.ForEach(target => SkillManager.Attack(target, user.Status.attackPower, 420));
    }

    /// <summary>
    /// 自身の攻撃力が20%増加する(解除不可)。
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill1(CharaController user)
    {
        SkillManager.ModifyAttackPower(user, user.Status.attackPower, 20, true);
    }

    // TODO 必要であればPassiveSkill2()も記述
}
