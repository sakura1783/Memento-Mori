using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// GSSReaderが取得した情報をシート単位で任意のスクリプタブルオブジェクトに値として取り込む
/// </summary>
[RequireComponent(typeof(GSSReader))]
public class GSSReceiver : MonoBehaviour
{
    public bool IsLoading { get; set; }


    /// <summary>
    /// GSSのデータ取得準備
    /// </summary>
    /// <returns></returns>
    public async UniTask PrepareGSSLoadStartAsync()
    {
        IsLoading = true;

        // スプレッドシートを取得し、スクリプタブルオブジェクトに取り込む
        await GetComponent<GSSReader>().GetFromWebAsync();

        IsLoading = false;

        Debug.Log("GSSデータをスクリプタブルオブジェクトに取得");
    }

    /// <summary>
    /// インスペクターからGSSReaderにこのメソッドを追加することでGSSの読み込み完了時にコールバックされる
    /// </summary>
    public void OnGSSLoadEnd()
    {
        GSSReader reader = GetComponent<GSSReader>();

        List<SheetData> sheetDatasList = reader.sheetDatas.ToList();

        if (sheetDatasList != null)
        {
            // 各スクリプタブルオブジェクトに代入
            DataBaseManager.instance.charaInitialDataSO.charaInitialDataList =
                new List<CharaInitialDataSO.CharaInitialData>(sheetDatasList.Find(x => x.sheetName == SheetName.CharaInitialData).datasList.Select(x => new CharaInitialDataSO.CharaInitialData(x)).ToList());
        }
    }
}
