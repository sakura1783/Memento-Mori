using UnityEngine;

public class OwnedCharaPop : PopupBase
{
    [SerializeField] private Transform charactersTran;

    [SerializeField] private CharaButton charaButtonPrefab;


    public override void ShowPopup()
    {
        GameData.instance.ownedCharaDataList.ForEach(data =>
        {
            var charaButton = Instantiate(charaButtonPrefab, charactersTran);
            charaButton.Setup(data);
            charaButton.transform.localScale = new Vector2(1.2f, 1.2f);
            // TODO charaButton押下時の処理を追加、ここでOnClickAsObservableのSubscribe内に記述
        });

        base.ShowPopup();
    }
}
