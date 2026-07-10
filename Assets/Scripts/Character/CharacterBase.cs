public abstract class CharacterBase
{
    public abstract int Active1CoolTime { get; }  // 外部から参照したいが、値は変更されたくないので、読み取り専用プロパティを定義。
    public abstract int Active2CoolTime { get; }  // 抽象プロパティでは本体を宣言できないが、getかsetどちらかのアクセサの宣言は必須。(= 変数をポリモーフィズムかつ、派生クラスで必ず実装を提供させたい(抽象プロパティを作りたい)場合は自動実装プロパティが必要となる？)

    public virtual PassiveSkillConfig Passive1Config { get; } = new(0, 0, 0, 1, PassiveActivationTiming.BattleStart);  // TODO 必要であれば派生クラスでoverrideする
    public virtual PassiveSkillConfig Passive2Config { get; } = new(0, 0, 0, 1, PassiveActivationTiming.BattleStart);


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
    }

    public abstract void ActiveSkill1(CharaController user);
    public abstract void ActiveSkill2(CharaController user);

    public virtual bool MeetsPassive1ActivationCondition(CharaController user)  // キャラ固有の発動条件をクリアするか判定
    {
        return true;
    }
    public virtual bool MeetsPassive2ActivationCondition(CharaController user)
    {
        return true;
    }
    
    public virtual void PassiveSkill1(CharaController user){}  // パッシブを持つキャラと持たないキャラがいるので、abstractではなくvirtualにして派生クラスでの実装は自由にする
    public virtual void PassiveSkill2(CharaController user){}
}
