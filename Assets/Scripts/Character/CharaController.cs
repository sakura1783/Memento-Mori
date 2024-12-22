using UnityEngine;
using UniRx;

/// <summary>
/// バトル時、キャラクター制御クラス
/// (このクラスはバトルで使われるキャラクターの数だけインスタンスが作られるので、singletonやstaticにはしない)
/// </summary>
public class CharaController
{
    //private CharaName charaName;

    private CharacterBase chara;

    private CalculateManager.VariableStatus status = new();
    public CalculateManager.VariableStatus Status => status;

    private int maxHp;
    public int MaxHp => maxHp;

    // TODO バフなど

    private int active1RemainingCoolTime;  // アクティブスキル1の残りのクールタイム
    private int active2RemainingCoolTime = 1;  // スキル1を最初に優先して発動するため、初期値は1に設定

    public ReactiveProperty<int> Passive1RemainingCoolTime = new(1);  // ExecutePassiveSkill()内、コメントの処理を実現するために、ReactivePropertyで監視処理・初期値を1に設定
    public ReactiveProperty<int> Passive2RemainingCoolTime = new(1);

    private CompositeDisposable disposables = new();


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
    /// アクティブスキルを発動
    /// </summary>
    public void ExecuteAction()
    {
         // スキル使用(スキル1、スキル2...の順番、全てのスキルがクールタイム中の場合、通常攻撃。)
        if (active1RemainingCoolTime <= 0)
        {
            chara.ActiveSkill1();

            // クールタイムを追加
            active1RemainingCoolTime += chara.Active1CoolTime;
        }
        else if (active2RemainingCoolTime <= 0)
        {
            chara.ActiveSkill2();
            active2RemainingCoolTime += chara.Active2CoolTime;
        }
        //else  // TODO 通常攻撃
    }

    /// <summary>
    /// パッシブスキルを発動
    /// </summary>
    public void ExecutePassiveSkill()  // TODO 全てのパネルとCharaControllerを生成し終わって、バトルの一番最初で実行
    {
        // パッシブのみ、ReactivePropertyの監視処理でクールタイムが0になった時に自動で処理(最初に発動してバトル中ずっと効果が続くものもあれば、何回か行動終了(ターン経過)して再発動するものもあるため)
        Passive1RemainingCoolTime
            .Where(coolTime => coolTime <= 0)
            .Subscribe(_ =>
            {
                chara.PassiveSkill1();
                Passive1RemainingCoolTime.Value += chara.Passive1CoolTime;
            })
            .AddTo(disposables);

        Passive2RemainingCoolTime
            .Where(coolTime => coolTime <= 0)
            .Subscribe(_ =>
            {
                chara.PassiveSkill2();
                Passive2RemainingCoolTime.Value += chara.Passive2CoolTime;
            })
            .AddTo(disposables);  

        disposables.Add(Passive1RemainingCoolTime);
        disposables.Add(Passive2RemainingCoolTime);  

        // 初期値を1に設定した変数の値を1減らすことで、バトルの一番最初で監視処理を動かす(バトルの最初で全てのパッシブスキルが動き、以降はターン経過で再度発動されるもののみ、監視処理で動く)
        Passive1RemainingCoolTime.Value = Mathf.Clamp(Passive1RemainingCoolTime.Value - 1, 0, int.MaxValue);
        Passive2RemainingCoolTime.Value = Mathf.Clamp(Passive2RemainingCoolTime.Value - 1, 0, int.MaxValue);
    }

    /// <summary>
    /// ターン毎に全てのスキルのクールタイムを1減らす
    /// </summary>
    public void ReduceCoolTimeByTurn()
    {
        active1RemainingCoolTime = Mathf.Clamp(active1RemainingCoolTime - 1, 0, int.MaxValue);
        active2RemainingCoolTime = Mathf.Clamp(active2RemainingCoolTime - 1, 0, int.MaxValue);

        Passive1RemainingCoolTime.Value = Mathf.Clamp(Passive1RemainingCoolTime.Value - 1, 0, int.MaxValue);
        Passive2RemainingCoolTime.Value = Mathf.Clamp(Passive2RemainingCoolTime.Value - 1, 0, int.MaxValue);
    }

    /// <summary>
    /// ReactivePropertyの監視処理を停止(CharaController破棄時に使う)
    /// </summary>
    public void Dispose()
    {
        disposables.Dispose();
    }
}
