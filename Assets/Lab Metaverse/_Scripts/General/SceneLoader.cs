using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static async void LoadAndClose(string sceneToOpen, string sceneToClose)
    {
        await Task.Run(() => LoadSceneOnly(sceneToOpen));
        CloseSceneAsync(sceneToClose);
    }

    public static async void LoadSceneOnly(string sceneToOpen)
    {
        LoadSceneAsync(sceneToOpen);
        await Task.Yield();
    }
    public static void CloseSceneAsync(string sceneToClose)
    {
        SceneManager.UnloadSceneAsync(sceneToClose);
    }

    async static void LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            //Output the current progress
            string loadingDebug = $"Loading scene {sceneName}: " + (asyncLoad.progress * 100) + "%";

            Debug.Log(loadingDebug);
            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("Loading is almost complete");
                asyncLoad.allowSceneActivation = true;
            }
            await Task.Yield();
        }
    }
}
