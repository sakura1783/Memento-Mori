using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class Setsuna : CharacterBase
{
    public override int Active1CoolTime => 5;
    public override int Active2CoolTime => 4;


    /// <summary>
    /// ランダムな敵に3回攻撃力*200%の攻撃。クリティカルヒットした場合、1ターンの間自身の攻撃力が30%増加する。クリティカルヒットするたび攻撃力増加のターン数が1ターン多くなる。
    /// </summary>
    /// <param name="user"></param>
    public override async void ActiveSkill1(CharaController user)
    {
        int criticalCount = 0;

        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 3, allowDuplicates: true);
        for (int i = 0; i < targets.Count; i++)
        {
            SkillManager.Attack(user, targets[i], user.Status.attackPower, 200, hitIndex: i, maxHitCount: targets.Count);
            
            if (targets[i].ReceivedCriticalDamage.Value)
                criticalCount++;
        }

        if (criticalCount > 0)
        {
            int increaseValue = SkillManager.ModifyAttackPower(user, user.Status.attackPower, 30, true);

            await SkillManager.WaitTurnsAsync(criticalCount);

            if (user != null) user.Status.attackPower -= increaseValue;
        }
    }

    /// <summary>
    /// ランダムな敵4体に攻撃力*160%の攻撃。このスキルで敵を戦闘不能にした場合、HP割合が最も低い敵に攻撃力*210%の攻撃を行う。敵を戦闘不能にするたび、追加攻撃の回数が1回多くなる(最大4回まで)
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill2(CharaController user)
    {
        int remainingAttackCount = 4;
        int attackIndex = 0;  // 現在何回目の攻撃か

        while (remainingAttackCount > 0 && attackIndex < 8)
        {   
            // 最初4回はランダムな敵、以降の追加攻撃はHP割合が最も低い敵を選択
            CharaController target = attackIndex < 4
                ? SkillManager.PickTarget(user, TargetType.Opponent, 1).FirstOrDefault()
                : SkillManager.PickTarget(user, TargetType.Opponent, 1, ValueType.ByCurrentHp, false).FirstOrDefault();

            // 最初4回は攻撃力*160%、以降の追加攻撃は攻撃力*210%で攻撃
            int attackRate = attackIndex < 4 ? 160 : 210;
            SkillManager.Attack(user, target, user.Status.attackPower, attackRate, 1, 1);  // TODO 引数(1, 1)はわかりにくい
            remainingAttackCount--;

            // 戦闘不能にするたび、攻撃回数+1
            if (target.Status.Hp.Value <= 0)
                remainingAttackCount++;

            attackIndex++;
        }
    }

    /// <summary>
    /// バトル開始時、自身に3ターンの間攻撃力*250%の「シールド」を付与
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill1(CharaController user)
    {
        SkillManager.AddBuff(user, BuffType.シールド, true, false, 3, effectValue: CalculateManager.CalculateValueByRate(user.Status.attackPower, 250));
    }

    /// <summary>
    /// 自身のHP割合が50%未満の場合、自身のクリティカル率が30%増加する(解除不可)。
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill2(CharaController user)
    {
        user.Status.Hp
            .Where(value => value < user.Status.MaxHp.Value / 2)
            .Take(1)  // 最初の一度だけイベントを通す
            .Subscribe(_ => SkillManager.IncreaseCriticalRate(user, 30));
    }
}
