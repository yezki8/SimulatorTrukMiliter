using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private StageDataController _stageDataController;

    [Header("Fixed Stage Scene Format")]
    public bool IsTesting = false;
    public string StageTestingName;

    public void StartGameplay()
    {
        string levelSceneName = _stageDataController.GetStageSceneName();

        if (IsTesting)
        {
            levelSceneName = (StageTestingName);
        }

        Debug.Log($"Loading level name: {levelSceneName}");
        SceneLoader.LoadAndClose(levelSceneName, this.gameObject.scene.name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
