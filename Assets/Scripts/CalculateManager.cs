using System;
using System.Linq;
using UniRx;

public class CalculateManager : AbstractSingleton<CalculateManager>
{
    /// <summary>
    /// レベルやバフに応じて変動するステータスを管理するクラス
    /// </summary>
    public class VariableStatus
    {
        public int combatPower;
        public int attackPower;
        public int defencePower;
        public ReactiveProperty<int> Hp;
        // TODO MaxHp;
        public float criticalRate;

        // TODO 複数の場所で使うのであれば、コンストラクタを作成しても良い
    }
    

    /// <summary>
    /// ダメージ計算
    /// </summary>
    /// <param name="attackPower"></param>
    /// <param name="damagePercentage">攻撃力の何%分のダメージを与えるか</param>
    /// <param name="targetDefencePower">攻撃対象の防御力</param>
    /// <returns></returns>
    public int CalculateDamage(int attackPower, float damagePercentage, int targetDefencePower)
    {
        int damage;

        // 通常(攻撃力*技/補正値)-(敵の防御力/補正値)
        damage = (int)Math.Round(attackPower * (damagePercentage / 100) / ConstData.ATTACK_MODIFIER - (targetDefencePower / ConstData.DEFENCE_MODIFIRE), 0, MidpointRounding.AwayFromZero);  // 少数第一位を四捨五入

        // TODO クリティカルボーナス、属性ボーナス

        return damage;
    }

    /// <summary>
    /// キャラの各ステータスを計算
    /// </summary>
    /// <param name="charaName"></param>
    /// <param name="targetLevel"></param>
    public VariableStatus CalculateCharaStatus(CharaName charaName, int level)
    {
        // キャラの初期データを取得
        //var charaData = (CharaInitialDataSO.CharaInitialData)DataBaseManager.instance.charaInitialDataSO.charaInitialDataList.Where(data => data.englishName == charaName);  // <= Whereは複数の要素を保持する可能性があり、左辺var CharaDataは単一のデータを指す。よって、(CharaInitialDataSO.CharaInitialData)でキャストできないため、エラーになる。
        var charaData = DataBaseManager.instance.charaInitialDataSO.charaInitialDataList.FirstOrDefault(data => data.englishName == charaName);  // ↑より、FirstOrDefaultを使う

        // TODO 元のデータを参照しているので書き換えられてしまうのでは？スクリプタブル・オブジェクトを確認する
        // キャラの各ステータスの初期値(Lv1のとき)を設定
        var status = new VariableStatus
        {
            combatPower = charaData.initialCombatPower,
            attackPower = charaData.initialAttackPower,
            defencePower = charaData.initialDefencePower,
            //Hp.Value = charaData.initialHp,
            Hp = { Value = charaData.initialHp },  // ReactivePropertyの場合、初期化はこのように記述する。↑だとエラーになる
            criticalRate = charaData.initialCriticalRate,
        };

        for (int i = 1; i <= level; i++)
        {
            // TODO キャラのランク(や職業)によってステータス上昇率を変更する

            // 限界突破(Lv20→21,40→41 etc...)
            if (i % 20 == 0)
            {
                // 現在の攻撃力、防御力、HPの30%を足す
                status.attackPower += (int)Math.Round(status.attackPower * ConstData.POWER_INC_RATE_BREAK_LIMIT, 0, MidpointRounding.AwayFromZero);
                status.defencePower += (int)Math.Round(status.defencePower * ConstData.POWER_INC_RATE_BREAK_LIMIT, 0, MidpointRounding.AwayFromZero);
                status.Hp.Value += (int)Math.Round(status.Hp.Value * ConstData.POWER_INC_RATE_BREAK_LIMIT, 0, MidpointRounding.AwayFromZero);

                // クリティカル率は限界突破の際に1%ずつ上昇
                status.criticalRate += 0.01f;
            }
            // 通常
            else
            {
                // 現在の攻撃力、防御力、HPの2.5%を足す
                status.attackPower += (int)Math.Round(status.attackPower * ConstData.POWER_INC_RATE, 0, MidpointRounding.AwayFromZero);
                status.defencePower += (int)Math.Round(status.defencePower * ConstData.POWER_INC_RATE, 0, MidpointRounding.AwayFromZero);
                status.Hp.Value += (int)Math.Round(status.Hp.Value * ConstData.POWER_INC_RATE, 0, MidpointRounding.AwayFromZero);
            }

            // 上で求めた各ステータスから、戦闘力を計算。
            // 戦闘力 = 攻撃力+防御力+補正後HP+補正後クリティカル率
            status.combatPower = (int)Math.Round(status.attackPower + status.defencePower + (status.Hp.Value * ConstData.HP_MODIFIRE) + (status.attackPower + status.attackPower * 0.5) / ConstData.ATTACK_MODIFIER * status.criticalRate, 0, MidpointRounding.AwayFromZero);
        }

        // 呼び出し元に計算後のステータスの情報を返す
        return status;
    }
}
