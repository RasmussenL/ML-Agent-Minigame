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
    public Color myColor;

    void Start()
    {
        

        if(PlayerPrefs.GetString("Difficulty") == "medium")
        {
            Dif2();
        }
        else if (PlayerPrefs.GetString("Difficulty") == "hard")
        {
            Dif3();
        }
        else
        {
            Dif1();
        }
    }
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
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(1);
    }

    public void Dif1()
    {
        //Button myButton = difButtons[0].GetComponent<Button>();
        //myButton.colors = ColorBlock.defaultColorBlock;
        Button myButton1 = difButtons[0].GetComponent<Button>();
        Button myButton2 = difButtons[1].GetComponent<Button>();
        Button myButton3 = difButtons[2].GetComponent<Button>();

        ColorBlock colorVar = myButton1.colors;
        colorVar.normalColor = myColor;
        myButton1.colors = colorVar;

        ColorBlock colorVar2 = myButton2.colors;
        colorVar2.normalColor = Color.white;
        myButton2.colors = colorVar2;

        ColorBlock colorVar3 = myButton3.colors;
        colorVar3.normalColor = Color.white;
        myButton3.colors = colorVar3;

        PlayerPrefs.SetString("Difficulty", "easy");
    }

    public void Dif2()
    {
        Button myButton1 = difButtons[0].GetComponent<Button>();
        Button myButton2 = difButtons[1].GetComponent<Button>();
        Button myButton3 = difButtons[2].GetComponent<Button>();

        ColorBlock colorVar = myButton1.colors;
        colorVar.normalColor = Color.white;
        myButton1.colors = colorVar;

        ColorBlock colorVar2 = myButton2.colors;
        colorVar2.normalColor = myColor;
        myButton2.colors = colorVar2;

        ColorBlock colorVar3 = myButton3.colors;
        colorVar3.normalColor = Color.white;
        myButton3.colors = colorVar3;

        PlayerPrefs.SetString("Difficulty", "medium");
    }

    public void Dif3()
    {
        Button myButton1 = difButtons[0].GetComponent<Button>();
        Button myButton2 = difButtons[1].GetComponent<Button>();
        Button myButton3 = difButtons[2].GetComponent<Button>();

        ColorBlock colorVar = myButton1.colors;
        colorVar.normalColor = Color.white;
        myButton1.colors = colorVar;

        ColorBlock colorVar2 = myButton2.colors;
        colorVar2.normalColor = Color.white;
        myButton2.colors = colorVar2;

        ColorBlock colorVar3 = myButton3.colors;
        colorVar3.normalColor = myColor;
        myButton3.colors = colorVar3;

        PlayerPrefs.SetString("Difficulty", "hard");
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
