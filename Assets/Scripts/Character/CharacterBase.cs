public abstract class CharacterBase
{
    protected int active1CoolTime;  // アクティブスキル1を使用後に追加されるクールタイム(各キャラ・各スキル個別に、派生クラス内で設定)
    public int Active1CoolTime => active1CoolTime;  // 外部から参照したいが、値は変更されたくないので、読み取り専用プロパティを定義。
    protected int active2CoolTime;
    public int Active2CoolTime => active2CoolTime;

    protected int passive1CoolTime;
    public int Passive1CoolTime => passive1CoolTime;
    protected int passive2CoolTime;
    public int Passive2CoolTime => passive2CoolTime;


    /// <summary>
    /// アクティブスキル1
    /// </summary>
    public abstract void ActiveSkill1();

    /// <summary>
    /// アクティブスキル2
    /// </summary>
    public abstract void ActiveSkill2();

    /// <summary>
    /// パッシブスキル1
    /// </summary>
    public abstract void PassiveSkill1();

    /// <summary>
    /// パッシブスキル2
    /// </summary>
    public void PassiveSkill2(){}  // パッシブスキルは2個あるキャラと1個だけのキャラがいるのでabstractにはせず、派生クラスでの実装は自由にする
}
