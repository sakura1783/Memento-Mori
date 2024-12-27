/// <summary>
/// ローゼヴィリアのスキル
/// </summary>
public class Rosevillea : CharacterBase
{
    public override int Active1CoolTime => 2;  // 各キャラ各スキルで個別の値を設定
    public override int Active2CoolTime => 3;

    // 必要であればパッシブのクールタイムも追加


    public override void ActiveSkill1(CalculateManager.VariableStatus status)
    {
        // ローゼヴィリアのアクティブスキル1の処理。
        // SKillManagerのメソッドを組み合わせてスキル処理を作る
    }

    public override void ActiveSkill2(CalculateManager.VariableStatus status)
    {

    }

    public override void PassiveSkill1(CalculateManager.VariableStatus status)
    {

    }

    // 必要であればPassiveSkill2()も記述
}
