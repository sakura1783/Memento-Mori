using System.Collections.Generic;
using System.Linq;

public class Elliot : CharacterBase
{
    public override int Active1CoolTime => 0;
    public override int Active2CoolTime => 0;

    public override int Passive2CoolTime => 1;


    /// <summary>
    /// 2ターンの間、味方全体の攻撃力を自身の攻撃力*15%増加
    /// </summary>
    /// <param name="user"></param>
    public override async void ActiveSkill1(CharaController user)  // <= asyncはシグネチャでないため(メソッドのシグネチャは、戻り値の型・名前・引数)、派生クラスでasyncを追加してオーバーライドしても問題ない(矛盾が生じない、エラーにならない)
    {
        // 変更前の攻撃力をリストで保持
        //List<int> unmodifiedValues = new();  // 変更前の値を保持してターン経過後に元の値に戻す場合、効果が重複していない場合はそれで問題ないが、効果が重複している場合は意図しない挙動になってしまうため、下の方法でアプローチ
        
        List<int> targetStatuses = new();
        List<int> increaseValues = new();

        var targets = SkillManager.PickTarget(user, TargetType.Ally);
        targets.ForEach(target =>
        {
            targetStatuses.Add(target.Status.attackPower);
            increaseValues.Add(SkillManager.ModifyAttackPower(target, user.Status.attackPower, 15, true));
        });

        await SkillManager.WaitTurnsAsync(2);
        
        // 増加した値だけ、攻撃力から引く
        //SkillManager.RevertStateIncrease()
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
    public override async void PassiveSkill2(CharaController user)
    {
        //int increaseValue;

        if (user.ReceivedCriticalDamage)
        {
            var targets = SkillManager.PickTarget(user, TargetType.Aggressor);
            targets.ForEach(target => SkillManager.ModifyAttackPower(target, target.Status.attackPower, 15, false));

            user.ReceivedCriticalDamage = false;  // これを忘れるとこのメソッドが実行されるたび毎回発動してしまうので注意
        }

        await SkillManager.WaitTurnsAsync(1);

        
    }
}
