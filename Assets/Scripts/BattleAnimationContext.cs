using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleAnimationContext : MonoBehaviour
{
    private List<UniTask> animationTasks = new();


    public void AddAnimation(UniTask anime)
    {
        animationTasks.Add(anime);
    }

    public async UniTask WaitAllAnimations()
    {
        await UniTask.WhenAll(animationTasks);
    }
}
