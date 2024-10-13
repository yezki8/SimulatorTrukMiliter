using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum StateOfGame
{
    Intro,
    Match,
    End,
    ViewScore
}
public class GameStateController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameStateUIHandler _gameStateUIHandler;

    [Header("Game State Parameter")]
    public StateOfGame GameState = StateOfGame.Intro;

    public static GameStateController Instance;
    public UnityEvent OnChangeStateToAny;           //specific task will run on any gamestate change
    public UnityEvent [] OnChangeGameState;         //specific task will run on specific state changed to
    public UnityEvent OnRestartStage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RestartStage();
    }

    public void ChangeGameState(int targetState)
    {
        GameState = (StateOfGame)targetState;
        OnChangeStateToAny?.Invoke();

        _gameStateUIHandler.ActivatePanel((int)GameState);
        OnChangeGameState[targetState]?.Invoke();
    }

    public void RestartStage()
    {
        OnRestartStage?.Invoke();
        ChangeGameState((int)StateOfGame.Intro);
    }

    // reconsider how to handle this
    public void ViewScore()
    {
        _gameStateUIHandler.ActivatePanel((int)StateOfGame.ViewScore);
    }

    public void LeaveScore()
    {
        _gameStateUIHandler.ActivatePanel((int)GameState);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CloseStage(string SceneName)       //As in, finish the stage / back to menu
    {
        //SceneNavigatorController.Instance.LoadAndClose(SceneName, this.gameObject.scene.name);
    }
}
