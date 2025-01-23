using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupManager : AbstractSingleton<PopupManager>
{
    [SerializeField] private List<PopupBase> popups = new();

    // TODO テスト。他の場所に移す
    [SerializeField] private GSSReceiver gssReceiver;

    // テスト
    private async void Start()
    {
        await gssReceiver.PrepareGSSLoadStartAsync();

        // ポップアップの初期設定
        popups.ForEach(pop => pop.Setup());

        // TODO テスト
        //Show<TeamAssemblyPop>();
        Show<GachaPop>();
    }

    /// <summary>
    /// PopupBaseを継承した、指定した型のポップアップを開く
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public PopupBase Show<T>() where T : PopupBase  // ジェネリック制約。TはPopupBaseを継承した型でなければならない。
    {
        var targetPop = popups.OfType<T>().FirstOrDefault();  // <= OfTypeでリストの中から指定した型を抽出

        targetPop.ShowPopup();

        return targetPop;
    }

    /// <summary>
    /// 指定した型のポップアップを取得
    /// (該当のポップアップ固有の処理を実行する際などに利用する)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetPopup<T>() where T : PopupBase
    {
        return popups.OfType<T>().FirstOrDefault();
    }
}
