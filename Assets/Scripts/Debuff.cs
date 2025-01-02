using UniRx;


/// <summary>
/// デバフの種類
/// </summary>
public enum DebuffType
{
    不治,  // HPが回復しなくなる
    気絶,  // 行動不能になる
    睡眠,  // 行動不能になる ※ダメージを受けると解除
    沈黙,  // スキルが使用不能になる
    毒,  // 毎ターン行動開始時に現在HP*?%のダメージを受ける

    //侵食,  // 毎ターン行動開始時、スキルの総与ダメージ*?%のダメージを与える
    //共鳴,  // お互いが受けたダメージ*?%のダメージを受けるようになる ※バトル開始時、防御力が最も高い敵と最も低い敵に付与
}

public class Debuff
{
    public DebuffType type;
    public ReactiveProperty<int> Duration = new();

    public int damageRate;  // 基準となる値(現在HP、総与ダメージ等)の?%分のダメージを与えるか。「毒」「侵食」などで使用する


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="type"></param>
    /// <param name="duration"></param>
    /// <param name="damageRate"></param>
    public Debuff(DebuffType type, int duration, int damageRate = 0)
    {
        this.type = type;
        Duration.Value = duration;
        this.damageRate = damageRate;
    }
}
