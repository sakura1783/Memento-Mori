using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DotweenTest : MonoBehaviour
{
    [SerializeField] private Transform doObj;  // 動かしたいUIのオブジェクト

    [SerializeField] private Transform doObj2;  // もう一個


    async void Start()
    {
        //await UniTask.Delay(TimeSpan.FromSeconds(10));  // Update()内のawaitとの競合を調べる
        await doObj2.DOPunchPosition(new Vector2(50f, -30f), 5f, 4).AsyncWaitForCompletion();  // AsyncWaitForCompletionの競合を調べる

        Debug.Log("doObj2 アニメーションが終わりました");
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            await doObj.DOPunchPosition(new Vector2(20f, -10f), 1f, 4).AsyncWaitForCompletion();

            Debug.Log("終わりました");
        }
    }
}
