using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject player;
    public GameObject opponent;

    public Text pLap;
    public Text oLap;
    public Text winText;

    public int pCurLap = 1;
    public int oCurLap = 1;

    public bool midpointHit = false;
    private bool racing = true;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        winText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (racing)
        {
            if (pCurLap > 3)
            {
                EndGame();
                winText.text = "Player Wins";
                racing = false;
            }
            else if (oCurLap > 3)
            {
                EndGame();
                winText.text = "Opponent Wins";
                racing = false;
            }
            pLap.text = "Lap " + pCurLap + "/3";
            oLap.text = "OpponentLap " + oCurLap + "/3";
        }
    }


    private void EndGame()
    {
        pLap.enabled = false;
        oLap.enabled = false;
        winText.enabled = true;
        Destroy(player);
        Destroy(opponent);
    }


}
