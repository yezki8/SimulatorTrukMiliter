using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageDataController : MonoBehaviour
{
    [SerializeField] private StageUIHandler _stageUIHandler;

    [System.Serializable]
    public class StageData
    {
        public StageConfigSO StageSO;
        public bool StageUnlocked = false;
    }

    public List<StageData> StageList;

    [Header("Stage Selection Variables")]
    public int StageSelected;
    public UnityEvent OnClickLevelButtons;

    //Set UI Methods ======================================================================================
    public void InitiateStagePanel()
    {
        for (int i = 0; i < StageList.Count; i++)
        {
            _stageUIHandler.SetUnlockedStageButton(i, StageList[i].StageUnlocked);
        }
        SetStageData(0);
    }

    //Set Data Methods ====================================================================================
    public void SetStageUnlockStatus(int stageIndex, bool status)
    {
        StageList[stageIndex].StageUnlocked = status;
    }

    public string GetStageSceneName()
    {
        return StageList[StageSelected].StageSO.StageScene;
    }

    public void SetStageData(int stageIndex)
    {
        StageSelected = stageIndex;
        _stageUIHandler.SetStageDescription(StageList[StageSelected].StageSO.StageInfo);
    }
}
