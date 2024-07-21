using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Config SO", menuName = "Scriptable Objects/Stages/Stage Config", order = 1)]
public class StageConfigSO : ScriptableObject
{
    [Header("Stage Name Stats")]
    public string StageName;
    public Sprite StageIllustration;
    public string StageInfo;

    [Header("Stage target")]
    public string StageScene;
}
