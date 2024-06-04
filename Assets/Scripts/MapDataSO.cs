using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataSO", menuName = "Create MapDataSO")]
public class MapDataSO : ScriptableObject
{
    public List<MapData> mapDataList = new();


    [System.Serializable]
    public class MapData
    {
        public int mapNo;
        public StageDataSO stageDataSO;
    }
}
