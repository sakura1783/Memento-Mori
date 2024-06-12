using UnityEngine;
using UniRx;

/// <summary>
/// バトル時、キャラクター制御クラス
/// </summary>
public class CharaController : MonoBehaviour
{
    private int attackPower;
    private int defencePower;
    private readonly int maxHp;
    public int MaxHp => maxHp;
    public ReactiveProperty<int> Hp = new();
    private float criticalRate;

    // TODO バフなど


    /// <summary>
    /// コンストラクタ
    /// 計算後の各ステータスの値を受け取り、キャラに反映させる
    /// </summary>
    /// <param name="statusData"></param>
    public CharaController(CalculateManager.VariableStatus statusData)
    {
        attackPower = statusData.attackPower;
        defencePower = statusData.defencePower;
        maxHp = statusData.hp;
        Hp.Value = maxHp;
        criticalRate = statusData.criticalRate;
    }

    /// <summary>
    /// HPの更新
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateHp(int amount)
    {
        Hp.Value += amount;
    }

    // BattleManagerの各チームListの型をこのクラスに変更

    // GameDataのCharaDataは所持キャラの情報の管理のみに充てる

    // 上記もまだちょっと良いかわからない。CharaDataを利用している各箇所を見直す。CalculateStatus()など、型を一致させた方が良いかもしれない

    // GameDataの中にCharaDataがあるのは不便であり、わかりづらくもある
}
