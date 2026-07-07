using System;
using UniRx;


/// <summary>
/// パッシブスキルのクールタイム減少タイミング
/// </summary>
// public enum PassiveCoolTimeReductionBasis  // TODO 必要であればコメントアウト解除
// {
//     None,  // クールタイムを使用しない
//     Turn,
//     Action,
// }

/// <summary>
/// 各キャラのクラスに継承させるクラス(基底クラス)
/// 共通して必要な情報のみ定義
/// </summary>
public abstract class CharacterBase
{
    public abstract int Active1CoolTime { get; }  // 外部から参照したいが、値は変更されたくないので、読み取り専用プロパティを定義。
    public abstract int Active2CoolTime { get; }  // 抽象プロパティでは本体を宣言できないが、getかsetどちらかのアクセサの宣言は必須。(= 変数をポリモーフィズムかつ、派生クラスで必ず実装を提供させたい(抽象プロパティを作りたい)場合は自動実装プロパティが必要となる？)

    public virtual PassiveSkillConfig Passive1Config { get; } = new(0, 0, 0, 0, PassiveActivationTiming.BattleStart);  // TODO 必要であれば派生クラスでoverrideする
    //public virtual PassiveCoolTimeReductionBasis passive1CoolTimeReductionBasis => PassiveCoolTimeReductionBasis.None;
    public virtual PassiveSkillConfig Passive2Config { get; } = new(0, 0, 0, 0, PassiveActivationTiming.BattleStart);
    //public virtual PassiveCoolTimeReductionBasis passive2CoolTimeReductionBasis => PassiveCoolTimeReductionBasis.None;


    /// <summary>
    /// 通常攻撃
    /// </summary>
    /// <param name="user"></param>
    public virtual void BasicAttack(CharaController user)
    {
        var targets = SkillManager.PickTarget(user, TargetType.Opponent, 1);
        targets.ForEach(target => SkillManager.Attack(user, target, user.Status.attackPower, 100));

        // アニメーション再生
        BattleAnimationManager.instance.AddAnimation(user, AnimationType.Attack);

        UnityEngine.Debug.Log("通常攻撃");
    }

    public abstract void ActiveSkill1(CharaController user);
    public abstract void ActiveSkill2(CharaController user);
    
    public virtual void PassiveSkill1(CharaController user){}  // ※ virtualのわけ= アリロシャの処理
    public virtual void PassiveSkill2(CharaController user){}  // パッシブスキルは2個あるキャラと1個だけのキャラがいるのでabstractではなくvirtualにして、派生クラスでの実装は自由にする

    /// <summary>
    /// バトル開始時、最初に1回だけ行う処理
    /// </summary>
    /// <param name="user"></param>
    public virtual void OnBattleStarted(CharaController chara){}

    /// <summary>
    /// ターン開始時に行う処理
    /// </summary>
    /// <param name="chara"></param>
    public virtual void OnTurnStarted(CharaController chara){}
}
