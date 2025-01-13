using UniRx;

/// <summary>
/// 状態効果(バフ・デバフ)の種類
/// </summary>
public enum BuffType
{
    /* バフ */
    再生,  // 毎ターン行動開始時にHPを最大HP*5%回復
    ダメージ無効,
    シールド,  // シールドの値を消費して被ダメージを軽減
    バリア,  // バリアを1層消費して、ダメージを無効化する

    /* デバフ */
    不治,  // HPが回復しなくなる
    気絶,  // 行動不能になる
    睡眠,  // 行動不能になる ※ダメージを受けると解除
    沈黙,  // スキルが使用不能になる
    毒,  // 毎ターン行動開始時に現在HP*?%のダメージを受ける
    
    //侵食,  // 毎ターン行動開始時、スキルの総与ダメージ*?%のダメージを与える
    //共鳴,  // お互いが受けたダメージ*?%のダメージを受けるようになる ※バトル開始時、防御力が最も高い敵と最も低い敵に付与
}

/// <summary>
/// すべての状態変化(ネガティブな効果も含む)をバフとする
/// </summary>
public class Buff
{
    public BuffType type;
    public bool isPositiveEffect;  // バフかデバフか(ポジティブな効果か、ネガティブな効果か)
    public bool isIrremovable;  // 解除不可かどうか
    public ReactiveProperty<int> Duration = new();
    public int effectRate;  // 基準となる値(現在HP、総与ダメージ等)の?%分の影響を与えるか。「毒」「侵食」「再生」などで使用する
    public ReactiveProperty<int> EffectValue; // 効果の量(シールドなどで利用)


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="type"></param>
    /// <param name="duration">解除不可のバフは、デフォルト値で大きな値を設定</param>
    /// <param name="damageRate"></param>
    public Buff(BuffType type, bool isPositiveEffect, bool isIrremovable, int duration = 100, int effectRate = 0, int effectValue = 0)
    {
        this.type = type;
        this.isPositiveEffect = isPositiveEffect;
        this.isIrremovable = isIrremovable;
        Duration.Value = duration;
        this.effectRate = effectRate;
        EffectValue.Value = effectValue;
    }
}
