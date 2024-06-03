using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public enum SheetName
{
    CharacterData,
}

[System.Serializable]
public class SheetData
{
    public SheetName sheetName;
    public List<string[]> datasList = new();
}

/// <summary>
/// スプレッドシートへアクセスして情報を取得する
/// </summary>
public class GSSReader : MonoBehaviour
{
    public string SheetID = "読み込むスプレッドシートのアドレス";

    public UnityEvent OnLoadEnd;  // この変数にインスペクターからメソッドを登録すると、シート読み込み後にコールバックする(登録した処理を行う)

    [Header("読み込みたいシート名を登録")]
    public SheetData[] sheetDatas;


    public async UniTask Reload() => await GetFromWebAsync();

    /// <summary>
    /// スプレッドシートの取得
    /// </summary>
    /// <returns></returns>
    public async UniTask GetFromWebAsync()
    {
        // Cancellationの作成
        var token = this.GetCancellationTokenOnDestroy();

        for (int i = 0; i < sheetDatas.Length; i++)
        {
            // 毎回読み込むシートを変更し、取得
            string url = "https://docs.google.com/spreadsheets/d/" + SheetID + "/gviz/tq?tqx=out:csv&sheet=" + sheetDatas[i].sheetName.ToString();
            UnityWebRequest request = UnityWebRequest.Get(url);

            // 取得できるまで待機
            await request.SendWebRequest().WithCancellation(token);
            Debug.Log(request.downloadHandler.text);

            // エラーが発生しているか確認
            bool protocol_error = request.result == UnityWebRequest.Result.ProtocolError ? true : false;
            bool connection_error = request.result == UnityWebRequest.Result.ConnectionError ? true : false;

            if (protocol_error || connection_error)
            {
                // エラーがある場合、ログ表示して処理を終了
                Debug.LogError(request.error);
                return;
            }

            // 各シートごとのデータをList<string[]>の形で取得
            sheetDatas[i].datasList = ConvertToArrayListFromCSV(request.downloadHandler.text);
        }

        OnLoadEnd.Invoke();
    }

    /// <summary>
    /// 取得したシートのCSVファイル(データをコンマで区切る形式で、スプレッドシートのセルやテキストファイルなどで一般的に使用される)の情報をArrayList形式に変換
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private List<string[]> ConvertToArrayListFromCSV(string text)
    {
        // StringReaderを利用して文字列を読み取る
        StringReader reader = new StringReader(text);
        reader.ReadLine();  // 1行目はヘッダー情報なので、読み飛ばす(StringReader.ReadLine()で、現在の文字列から一行分の文字を読み取る)

        List<string[]> rows = new();

        while (reader.Peek() >= 0)  // Peekメソッドを使うと、戻り値の値によりファイルの末尾まで達しているか確認できる。末尾になると-1が戻るので、それまで繰り返す
        {
            string line = reader.ReadLine();
            string[] elements = line.Split(',');  // 文字列は「,」で区切られている

            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i] == "\"\"")  // 「\"\"」は空白のセルを表す
                {
                    continue;
                }

                // 文字列の最初と最後の"を削除する
                elements[i] = elements[i].TrimStart('"').TrimEnd('"');
            }

            rows.Add(elements);
        }

        return rows;
    }
}
