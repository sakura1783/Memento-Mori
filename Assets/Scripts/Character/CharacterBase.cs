/// <summary>
/// 各キャラのクラスに継承させるクラス(基底クラス)
/// 共通して必要な情報のみ定義
/// </summary>
public abstract class CharacterBase
{
    //private int active1CoolTime;  // アクティブスキル1を使用後に追加されるクールタイム(各キャラ・各スキル個別に、派生クラス内で設定)
    //public int Active1CoolTime => active1CoolTime;  // 外部から参照したいが、値は変更されたくないので、読み取り専用プロパティを定義。
    public abstract int Active1CoolTime { get; }  // 変数を派生クラスでoverrideしたいので、抽象(abstract)プロパティを利用(各キャラ各スキルで値が違うので必ず派生クラスで実装したい)。今回は外部から参照した際に派生クラスで設定した値を返せばいいので、getアクセサのみ。
    public abstract int Active2CoolTime { get; }  // 抽象プロパティでは本体を宣言できないが、getかsetどちらかのアクセサの宣言は必須。(= 変数をポリモーフィズムかつ、派生クラスで必ず実装を提供させたい(抽象プロパティを作りたい)場合は自動実装プロパティが必要となる？)

    public virtual int Passive1CoolTime { get; }  // バトル開始時一度きりの発動のパッシブもあるので、virtualを使って派生クラスでの実装は自由にする。(抽象プロパティとは違い、この場合はprivateの変数のプロパティを作ってカプセル化を行っても良い)
    public virtual int Passive2CoolTime { get; }


    /// <summary>
    /// アクティブスキル1
    /// </summary>
    public abstract void ActiveSkill1(CalculateManager.VariableStatus status);

    /// <summary>
    /// アクティブスキル2
    /// </summary>
    public abstract void ActiveSkill2(CalculateManager.VariableStatus status);

    /// <summary>
    /// パッシブスキル1
    /// </summary>
    public abstract void PassiveSkill1(CalculateManager.VariableStatus status);

    /// <summary>
    /// パッシブスキル2
    /// </summary>
    public virtual void PassiveSkill2(CalculateManager.VariableStatus status){}  // パッシブスキルは2個あるキャラと1個だけのキャラがいるのでabstractではなくvirtualにして、派生クラスでの実装は自由にする
}
