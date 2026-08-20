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
        // TODO できたらリファクタリングしたい
        int criticalCount = 0;
        int increaseValue = 0;

        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 3, allowDuplicates: true);

        SkillManager.RandomAttack(user, targets, user.Status.attackPower, 200,
            onAttackCompletion: result =>
            {
                if (result.CriticalCount <= 0)
                    return;

                criticalCount = result.CriticalCount;
                increaseValue = SkillManager.ModifyAttackPower(user, user.Status.attackPower, 30, true);

                // TODO 下の処理を入れたら、ターン経過待機処理で無限ループになるよね？
            });

        await SkillManager.WaitTurnsAsync(criticalCount);

        user.Status.attackPower -= increaseValue;
    }

    /// <summary>
    /// ランダムな敵4体に攻撃力*160%の攻撃。このスキルで敵を戦闘不能にした場合、HP割合が最も低い敵に攻撃力*210%の攻撃を行う。敵を戦闘不能にするたび、追加攻撃の回数が1回多くなる(最大4回まで)
    /// </summary>
    /// <param name="user"></param>
    public override void ActiveSkill2(CharaController user)
    {
        // TODO もう少し簡単にできないか、また、RandomAttack()をうまく利用できないか検討。処理が煩雑でわかりにくい。

        const int initialHitCount = 4;
        const int maxHitCount = 8;

        int remainingAttackCount = initialHitCount;
        int currentHitIndex = 0;

        void ExecuteNextHit()
        {
            if (remainingAttackCount <= 0 || currentHitIndex < maxHitCount)
                return;

            bool isAdditionalHit = currentHitIndex >= initialHitCount;

            // 最初4回はランダムな敵、以降の追加攻撃はHP割合が最も低い敵を選択
            CharaController target = currentHitIndex < 4
                ? SkillManager.PickTarget(user, TargetType.Opponent, 1).FirstOrDefault()
                : SkillManager.PickTarget(user, TargetType.Opponent, 1, ValueType.ByCurrentHpRate, false).FirstOrDefault();

            if (target == null)
                return;

            // 最初4回は攻撃力*160%、以降の追加攻撃は攻撃力*210%で攻撃
            int attackRate = currentHitIndex < 4 ? 160 : 210;

            SkillManager.SingleAttack(user, target, user.Status.attackPower, attackRate, AttackPattern.Single,
                onHitCompletion: result =>
                {
                    remainingAttackCount--;
                    currentHitIndex++;

                    // 戦闘不能にするたび、攻撃回数+1
                    if (result.DefeatedTarget) remainingAttackCount++;

                    ExecuteNextHit();
                });
        }
    }

    /// <summary>
    /// バトル開始時、自身に3ターンの間攻撃力*250%の「シールド」を付与
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill1(CharaController user)
    {
        SkillManager.ApplyBuff(user, BuffType.シールド, true, false, 3, effectValue: CalculateManager.CalculateValueByRate(user.Status.attackPower, 250));
    }

    /// <summary>
    /// 自身のHP割合が50%未満の場合、自身のクリティカル率が30%増加する(解除不可)。
    /// </summary>
    /// <param name="user"></param>
    public override void PassiveSkill2(CharaController user)
    {
        user.Status.Hp
            .Where(value => user.IsAlive && value < user.Status.MaxHp.Value / 2)
            .Take(1)  // 最初の一度だけイベントを通す
            .Subscribe(_ => SkillManager.IncreaseCriticalRate(user, 30));
    }
}
