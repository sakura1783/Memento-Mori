using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バトル時、キャラクター制御クラス
/// </summary>
public class CharaController : MonoBehaviour
{
    // BattleManagerの各チームListの型をこのクラスに変更

    // GameDataのCharaDataは所持キャラの情報の管理のみに充てる

    // 上記もまだちょっと良いかわからない。CharaDataを利用している各箇所を見直す。CalculateStatus()など、型を一致させた方が良いかもしれない

    // GameDataの中にCharaDataがあるのは不便であり、わかりづらくもある
}
