using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public enum AnimationType
{
    Attack,  // 通常攻撃・追撃時
    Damage,
    ApplyEffect,  // バフ・デバフの付与時
    ReceiveBuff,
    ReceiveDebuff,
}

public class BattleAnimationManager : AbstractSingleton<BattleAnimationManager>
{
    [SerializeField] private BattleManager battleManager;

    private List<UniTask> animationTasks = new();

    // パーティクルシステムなどのプレハブ用変数


    public void AddAnimation(CharaController target, AnimationType animationType)
    {
        animationTasks.Add(PlayAnimation(target, animationType));
    }

    public async UniTask WaitAllAnimations()
    {
        if (animationTasks.Count == 0)
            return;

        await UniTask.WhenAll(animationTasks);
        animationTasks.Clear();
    }

    public UniTask PlayAnimation(CharaController target, AnimationType animationType)
    {
        RectTransform rect = target.CharaStatusPannel as RectTransform;

        return animationType switch
        {
            AnimationType.Attack => 
                PlayAttackAnimation(rect, target),

            AnimationType.Damage =>
                PlayDamageAnimation(rect, target),

            AnimationType.ApplyEffect => 
                PlayApplyEffectAnimation(rect),

            AnimationType.ReceiveBuff => 
                PlayReceiveBuffAnimation(rect),

            AnimationType.ReceiveDebuff => 
                PlayReceiveDebuffAnimation(rect),

            _ => UniTask.CompletedTask
        };
    }

    private UniTask PlayAttackAnimation(RectTransform statusPanelRect, CharaController target)
    {
        Vector3 pos = new(40f, 0f, 0f);
        return statusPanelRect
            .DOPunchPosition(pos, 0.7f, 2).ToUniTask();
    }

    private UniTask PlayDamageAnimation(RectTransform statusPanelRect, CharaController target)
    {
        Vector3 pos = new(battleManager.playerTeam.Contains(target) ? -20f : 20f, 0f, 0f);
        
        return statusPanelRect
            .DOPunchPosition(pos, 1f, 6).ToUniTask();  // TODO 下TODO部分のようにCancellationToken設定する必要あり？
    }

    private UniTask PlayApplyEffectAnimation(RectTransform pos)
    {
        // TODO パネルのキャラクターの顔の位置にエフェクトを生成したい→ 引数違うかも

        // パーティクルを生成してアニメーション
        return UniTask.CompletedTask;
    }

    private UniTask PlayReceiveBuffAnimation(RectTransform targetPos)
    {
        return UniTask.CompletedTask;
    }

    private UniTask PlayReceiveDebuffAnimation(RectTransform targetPos)
    {
        return UniTask.CompletedTask;
    }

    // TODO 消す
    // DOTweenでアニメーション
    //Vector2 punchPos = new(battleManager.playerTeam.Any(chara => chara == target) ? 20f : -20f, -10f);
    //Debug.Log($"punchPos = {punchPos}");
    //target.CharaStatusPannel.DOKill();  // 既存のトゥイーンを停止してから新しいトゥイーンを開始
    // await target.CharaStatusPannel  
    //     .DOPunchPosition(punchPos, 1f, 2).AsyncWaitForPosition(1f);  // AsyncWaitForCompletion()でトゥイーンのTaskを返す
    // var cts = new CancellationTokenSource();
    // var token = cts.Token;
    // await target.CharaStatusPannel.DOPunchPosition(punchPos, 1f, 2)
    //     .ToUniTask(cancellationToken: token);　　// TODO 

    // クラッシュしないが、待ちもしない？もう一度確認してみる
    // target.CharaStatusPannel.DOPunchPosition(punchPos, 1f, 2).OnComplete(() =>
    // {
    //     Debug.Log($"{user.Name}が{target.Name}に、{damageValue}の攻撃");
    // });
}
