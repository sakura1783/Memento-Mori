using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class Setsuna : CharacterBase
{
    public override int Active1CoolTime => 5;
    public override int Active2CoolTime => 4;

    private const int ACTIVE2_INITIAL_HIT_COUNT = 4;
    private const int ACTIVE2_MAX_HIT_COUNT = 8;


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
        int allowedHitCount = ACTIVE2_INITIAL_HIT_COUNT;
        int hitIndex = 0;

        void ExecuteNextHit()
        {
            if (hitIndex >= allowedHitCount)
            return;

            bool isAdditionalHit = hitIndex >= ACTIVE2_INITIAL_HIT_COUNT;

            // 最初4回はランダムな敵、以降の追加攻撃はHP割合が最も低い敵を選択
            CharaController target = isAdditionalHit
                ? SkillManager.PickTarget(user, TargetType.Opponent, 1, ValueType.ByCurrentHpRate, false).FirstOrDefault()
                : SkillManager.PickTarget(user, TargetType.Opponent, 1).FirstOrDefault();

            if (target == null)
                return;

            // 最初4回は攻撃力*160%、以降の追加攻撃は攻撃力*210%で攻撃
            int attackRate = isAdditionalHit ? 210 : 160;

            SkillManager.SingleAttack(user, target, user.Status.attackPower, attackRate, AttackPattern.Single,
                onHitCompletion: result =>
                {
                    // 戦闘不能にするたび、攻撃回数+1
                    if (result.DefeatedTarget)
                        allowedHitCount = Mathf.Min(allowedHitCount + 1, ACTIVE2_MAX_HIT_COUNT);

                    hitIndex++;

                    ExecuteNextHit();
                });
        }
        ExecuteNextHit();
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
