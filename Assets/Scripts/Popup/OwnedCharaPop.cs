using System.Collections;
using System.Collections.Generic;
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
            charaButton.Setup(data);  // TODO CharaButtonクラスにボタンの挙動を追加(TeamAssemblyPopのボタンとは違う挙動を行うため)。=> 反映させる
            charaButton.transform.localScale = new Vector2(1.2f, 1.2f);
        });

        base.ShowPopup();
    }
}
