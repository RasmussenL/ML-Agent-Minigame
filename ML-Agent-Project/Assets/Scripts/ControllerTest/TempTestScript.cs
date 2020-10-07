using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTestScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float triggerValue = Input.GetAxis("Gas");

        float turnValue = Input.GetAxis("Turn");

        Debug.Log("Acceleration Value: " + triggerValue + "      Turning value: " + turnValue);
    }
}
