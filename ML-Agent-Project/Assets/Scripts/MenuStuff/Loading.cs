using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Image progressBar;
    void Start()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {

        if (PlayerPrefs.GetString("Difficulty") == "easy")
        {
            AsyncOperation gameLevel = SceneManager.LoadSceneAsync(2);
            while (gameLevel.progress < 1)
            {
                Debug.Log(gameLevel.progress);
                progressBar.fillAmount = gameLevel.progress;
                yield return new WaitForEndOfFrame();
            }
        }
        else if (PlayerPrefs.GetString("Difficulty") == "medium")
        {
            AsyncOperation gameLevel = SceneManager.LoadSceneAsync(3);
            while (gameLevel.progress < 1)
            {
                Debug.Log(gameLevel.progress);
                progressBar.fillAmount = gameLevel.progress;
                yield return new WaitForEndOfFrame();
            }
        }
        else if (PlayerPrefs.GetString("Difficulty") == "hard")
        {
            AsyncOperation gameLevel = SceneManager.LoadSceneAsync(4);
            while (gameLevel.progress < 1)
            {
                Debug.Log(gameLevel.progress);
                progressBar.fillAmount = gameLevel.progress;
                yield return new WaitForEndOfFrame();
            }
        }
        
    }
}
