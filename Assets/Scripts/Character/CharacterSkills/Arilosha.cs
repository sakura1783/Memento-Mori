public class Arilosha : CharacterBase
{
    public override int Active1CoolTime => 4;
    public override int Active2CoolTime => 4;


    /// <summary>
    /// 自身に現在HP*10%の自傷ダメージを与え、現在HPが最も低い敵に攻撃力*480%の物理攻撃を行う
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill1(CharaController user)
    {

    }

    public override void ActiveSkill2(CharaController user)
    {

    }

    public override void PassiveSkill1(CharaController user)
    {
        
    }
}
