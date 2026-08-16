using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class BattleActionTimeline : AbstractSingleton<BattleActionTimeline>
{
    /// <summary>
    /// usingで利用するクラス
    /// </summary>
    private class TimingScope : IDisposable
    {
        private readonly Action onDispose;

        public TimingScope(Action onDispose)
        {
            this.onDispose = onDispose;
        }

        public void Dispose()
        {
            onDispose?.Invoke();
        }
    }

    private readonly List<UniTask> tasks = new();

    public float CurrentDelay { get; private set; }  // 現在のスコープに適用されている遅延時間

    
    /// <summary>
    /// 実処理をタイムラインに登録
    /// additionalDelayは引数で渡されたactionにのみ適用される
    /// </summary>
    /// <param name="action"></param>
    /// <param name="additionalDelay"></param>
    public void Schedule(Action action, float additionalDelay = 0f)
    {
        float delay = CurrentDelay + additionalDelay;
        tasks.Add(ExecuteAsync(action, delay));
    }

    /// <summary>
    /// 非同期処理をタイムラインに登録
    /// </summary>
    /// <param name="action"></param>
    /// <param name="additionalDelay"></param>
    public void Schedule(Func<UniTask> action, float additionalDelay = 0f)
    {
        float delay = CurrentDelay + additionalDelay;
        tasks.Add(ExecuteAsync(action, delay));
    }

    private async UniTask ExecuteAsync(Action action, float delay)
    {
        if (delay > 0f)
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

        action();
    }

    private async UniTask ExecuteAsync(Func<UniTask> action, float delay)
    {
        if (delay > 0f)
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

        await action();
    }
    
    /// <summary>
    /// using範囲内の処理に追加の遅延を適用
    /// </summary>
    /// <param name="additionalDelay"></param>
    /// <returns></returns>
    public IDisposable UseDelay(float delay)
    {
        float previousDelay = CurrentDelay;
        CurrentDelay += delay;

        return new TimingScope(() => CurrentDelay = previousDelay);
    }

    public async UniTask WaitAllAsync()
    {
        while(tasks.Count > 0)  // タスクの終了を待っている間に追加された遅延処理を逃さないようにする
        {
            var currentTasks = tasks.ToArray();  // ToArray()でリストの中身をコピー
            tasks.Clear();

            await UniTask.WhenAll(currentTasks);
        }

        CurrentDelay = 0f;
    }
}
