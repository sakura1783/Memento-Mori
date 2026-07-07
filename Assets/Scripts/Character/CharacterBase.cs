public abstract class CharacterBase
{
    public abstract int Active1CoolTime { get; }  // 外部から参照したいが、値は変更されたくないので、読み取り専用プロパティを定義。
    public abstract int Active2CoolTime { get; }  // 抽象プロパティでは本体を宣言できないが、getかsetどちらかのアクセサの宣言は必須。(= 変数をポリモーフィズムかつ、派生クラスで必ず実装を提供させたい(抽象プロパティを作りたい)場合は自動実装プロパティが必要となる？)

    public virtual PassiveSkillConfig Passive1Config { get; } = new(0, 0, 0, 0, PassiveActivationTiming.BattleStart);  // TODO 必要であれば派生クラスでoverrideする
    public virtual PassiveSkillConfig Passive2Config { get; } = new(0, 0, 0, 0, PassiveActivationTiming.BattleStart);


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
    
    public virtual void PassiveSkill1(CharaController user)  // ※ virtualのわけ= アリロシャの処理
    {
        UnityEngine.Debug.Log($"パッシブスキル1が動きました：{user.Name}");
    }
    public virtual void PassiveSkill2(CharaController user)
    {
        UnityEngine.Debug.Log($"パッシブスキル2が動きました：{user.Name}");
    }  // パッシブスキルは2個あるキャラと1個だけのキャラがいるのでabstractではなくvirtualにして、派生クラスでの実装は自由にする
}
