using System.Collections.Generic;
using System.Linq;

public class Elliot : CharacterBase
{
    public override int Active1CoolTime => 4;
    public override int Active2CoolTime => 4;

    public override int Passive2CoolTime => 1;


    /// <summary>
    /// 2ターンの間、味方全体の攻撃力を自身の攻撃力*15%増加
    /// </summary>
    /// <param name="user"></param>
    public override async void ActiveSkill1(CharaController user)  // <= asyncはシグネチャでないため(メソッドのシグネチャは、戻り値の型・名前・引数)、派生クラスでasyncを追加してオーバーライドしても問題ない(矛盾が生じない、エラーにならない)
    {
        // 変更前の攻撃力をリストで保持
        // List<int> unmodifiedValues = new();  // 変更前の値を保持してターン経過後に元の値に戻す場合、効果が重複していない場合はそれで問題ないが、効果が重複している場合は意図しない挙動になってしまうため、下の方法でアプローチ
        
        List<int> increaseValues = new();
        
        var targets = SkillManager.PickTarget(user, TargetType.Ally);
        targets.ForEach(target => increaseValues.Add(SkillManager.ModifyAttackPower(target, user.Status.attackPower, 15, true)));

        await SkillManager.WaitTurnsAsync(2);
        
        // 増加した攻撃力を元に戻す(増加させた分だけ、攻撃力から引く)
        // for (int i = 0; i < increaseValues.Count; i++)
        // {
        //     targets[i].Status.attackPower -= increaseValues[i];
        // }
        targets.Zip(increaseValues, (target, value) => target.Status.attackPower -= value).ToList();  // 上記を簡略化。Linqは遅延評価されるため、ToList()で強制的に即時実行(ToList()を行わない場合、attackPowerの変更が適用されないので注意)。
    }

    /// <summary>
    /// ランダムな敵3体に攻撃力*200%の攻撃。さらに、HP割合が低い味方2体のHPを自身の攻撃力*50%回復し、1ターンの間「再生」を付与。毎ターン行動開始時にHPを自身の最大HP*3%回復。
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill2(CharaController user)
    {
        var attackTargets = SkillManager.PickTarget(user, TargetType.Opponent, 3);
        attackTargets.ForEach(target => SkillManager.Attack(user, target, user.Status.attackPower, 200));

        var healTargets = SkillManager.PickTarget(user, TargetType.Ally, 2, ValueType.ByCurrentHp, false);
        healTargets.ForEach(target =>
        {
            SkillManager.Heal(target, user.Status.attackPower, 50);
            SkillManager.AddBuff(target, BuffType.再生, true, false, 1, 3);
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
        if (user.ReceivedCriticalDamage)
        {
            int decreaseValue = 0;

            user.ReceivedCriticalDamage = false;  // これを忘れるとこのメソッドが実行されるたび毎回発動してしまうので注意

            var targets = SkillManager.PickTarget(user, TargetType.Aggressor);
            targets.ForEach(target => decreaseValue = SkillManager.ModifyAttackPower(target, target.Status.attackPower, 15, false));

            await SkillManager.WaitTurnsAsync(1);

            // 減少させた攻撃力を元に戻す
            targets.ForEach(target => target.Status.attackPower += decreaseValue);
        }
    }
}
