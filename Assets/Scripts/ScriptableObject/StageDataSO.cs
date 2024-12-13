using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDataSO", menuName = "Create StageDataSO")]
public class StageDataSO : ScriptableObject
{
    public List<StageData> stageDataList = new();


    [System.Serializable]
    public class StageData
    {
        public int stageNo;
        //public List<EnemyData> enemyDataList = new();
        public List<GameData.CharaConstData> enemyDataList = new();
    }

    //[System.Serializable]
    //public class EnemyData
    //{
    //    public CharaName name;
    //    public int level;
    //}
}
