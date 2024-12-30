/// <summary>
/// ローゼヴィリアのスキル
/// 各スキルメソッドには、SkillManagerのメソッドを組み合わせて処理を作る
/// </summary>
public class Rosevillea : CharacterBase
{
    public override int Active1CoolTime => 2;  // 各キャラ各スキルで個別の値を設定
    public override int Active2CoolTime => 3;

    // 必要であればパッシブのクールタイムも追加


    /// <summary>
    /// ランダムな敵1体に攻撃力*190%の攻撃  // TODO 2ターンの間「気絶」を付与
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill1(CharaController user)
    {
        // ターゲットを取得(戻り値はList<CharaController>型)    
        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 1);

        // 攻撃
        targets.ForEach(target => SkillManager.Attack(target, user.Status.attackPower, 390));
    }

    /// <summary>
    /// ローゼヴィリアのアクティブスキル2
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill2(CharaController user)
    {

    }

    /// <summary>
    /// ローゼヴィリアのパッシブスキル1
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill1(CharaController user)
    {

    }

    // 必要であればPassiveSkill2()も記述
}
