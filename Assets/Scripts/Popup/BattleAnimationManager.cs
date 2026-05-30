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

    private UniTask PlayAnimation(CharaController target, AnimationType animationType)
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
            .DOPunchPosition(pos, 1f, 6).ToUniTask();
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
}
