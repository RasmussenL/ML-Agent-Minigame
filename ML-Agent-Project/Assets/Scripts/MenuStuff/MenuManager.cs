using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject[] panels;
    public GameObject menuDef;
    public GameObject controlsDef;
    public GameObject[] difButtons;

    public void GoTo(string name)
    {
        foreach (GameObject p in panels)
        {
            if (p.name == name)
            {
                p.SetActive(true);
                if(name == "Menu")
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(menuDef);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(controlsDef);
                }
            }
            else
            {
                p.SetActive(false);
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Dif1()
    {
        Button myButton = difButtons[0].GetComponent<Button>();
        myButton.colors = ColorBlock.defaultColorBlock;
    }

    public void Dif2()
    {

    }

    public void Dif3()
    {

    }

    public void QuitGame()
    {
         #if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
         #else
         Application.Quit();
         #endif
    }
}
