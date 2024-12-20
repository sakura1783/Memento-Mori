using UnityEngine;
using UniRx;

/// <summary>
/// バトル時、キャラクター制御クラス
/// (このクラスはバトルで使われるキャラクターの数だけインスタンスが作られるので、singletonやstaticにはしない)
/// </summary>
public class CharaController
{
    private CharaName charaName;

    private CalculateManager.VariableStatus status = new();
    public CalculateManager.VariableStatus Status => status;

    private int maxHp;
    public int MaxHp => maxHp;

    // TODO バフなど


    /// <summary>
    /// コンストラクタ
    /// 計算後の各ステータスの値を受け取り、キャラに反映させる
    /// </summary>
    /// <param name="statusData"></param>
    public CharaController(CalculateManager.VariableStatus statusData)
    {
        status = statusData;
        maxHp = status.Hp.Value;
    }

    /// <summary>
    /// HPの更新
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateHp(int amount)
    {
        status.Hp.Value += amount;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ExecuteAction()
    {
        // スキル使用(スキル1、スキル2...の順番、全てのスキルがクールタイム中の場合、通常攻撃。)
    }

    /// <summary>
    /// 
    /// </summary>
    public void ExecutePassiveSkill()  // TODO 全てのパネルとCharaControllerを生成し終わって、バトルの一番最初で実行
    {

    }
}
