using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject player;
    public GameObject opponent;
    public GameObject pause;
    public GameObject pauseFirstButton;
    public GameObject win;
    public GameObject winFirstButton;
    public GameObject bg;
    public Camera playerCam;

    public Text pLap;
    public Text oLap;
    public Text winText;
    //public Image bgImg;

    public Camera playerCamera;

    public int pCurLap = 1;
    public int oCurLap = 1;

    public bool midpointHit = false;
    private bool racing = true;
    private bool paused = false;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // This Continue() prevents pauses on reload
        Continue();
        //winText.enabled = false;
        opponent.GetComponent<VehicleAgent>().FreezeAgent();
        player.GetComponent<VehicleAgent>().FreezeAgent();
        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()
    {
        if (racing)
        {
            if (Input.GetButtonDown("Pause"))
            {
                PauseGame();
            }
            if (pCurLap > 3)
            {
                EndGame();
                winText.text = "Player Wins";
                winText.color = Color.green;
                racing = false;
            }
            else if (oCurLap > 3)
            {
                EndGame();
                winText.color = Color.red;
                winText.text = "Opponent Wins";
                racing = false;
            }
            pLap.text = pCurLap + "/3";
            oLap.text = oCurLap + "/3";
        }
    }

    private IEnumerator StartGame()
    {
        
        winText.text = "3";
        yield return new WaitForSeconds(1f);
        winText.text = "2";
        yield return new WaitForSeconds(1f);
        winText.text = "1";
        yield return new WaitForSeconds(1f);
        winText.text = "RACE!";
        opponent.GetComponent<VehicleAgent>().UnfreezeAgent();
        player.GetComponent<VehicleAgent>().UnfreezeAgent();
        yield return new WaitForSeconds(1f);
        winText.text = "";
        //bgImg.enabled = false;
    }

    private void EndGame()
    {
        pLap.enabled = false;
        oLap.enabled = false;
        bg.SetActive(false);
        playerCam.enabled = false;
       // bgImg.enabled = true;
        opponent.GetComponent<VehicleAgent>().FreezeAgent();
        player.GetComponent<VehicleAgent>().FreezeAgent();
        //playerCamera.enabled = false;
        win.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(winFirstButton);
    }

    private void PauseGame()
    {
        if (!paused)
        {
            Time.timeScale = 0f;
            pause.SetActive(true);
            paused = true;
            //winText.text = "Paused";
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);
        }
        else
        {
            Continue();
        }
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        pause.SetActive(false);
        paused = false;
        //winText.text = "";
    }

    public void BackToMenu()
    {
        var objects = GameObject.FindObjectsOfType<GameObject>();
        Resources.UnloadUnusedAssets();
        foreach (GameObject o in objects)
        {
            Destroy(o);
        }
        SceneManager.LoadScene(0);
    }
}
