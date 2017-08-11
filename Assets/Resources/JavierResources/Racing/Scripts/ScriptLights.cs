using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/* 
 * This script should be attached to a car.
 * It takes care of updating the lights of the car
 * according to the recording. The public variables 
 * are changed in the CSVFileReader attached to the 
 * same car.
 * 
 * If the car model is ever changed, you would have to
 * attach its light elements to the public variables of 
 * this script by drag-and-drop. The code in CANDataCollector
 * adds a CSVFileReader to the car automatically, but it doesn't 
 * add a ScriptLights, so that needs to be done manually (this way
 * it's easier to remember to link the lights which are different
 * in each car model).
 */
public class ScriptLights : MonoBehaviour {

    public bool braking;
    public bool signalingLeft;
    public bool signalingRight;
	[Header("Lights")]
    public GameObject backLights;

    // indicators i.e. RB: right-back indicator
    public Renderer RB;
    public Renderer LB;
    public Renderer RF;
    public Renderer LF;

    public Material turnSignalON;
    public Material turnSignalOFF;


	void Start () {
        backLights.SetActive(false);
    }
	

	void Update () {

        backLights.SetActive(braking);

        if (signalingLeft)
        {
            LB.material = turnSignalON;
            LF.material = turnSignalON;
            RB.material = turnSignalOFF;
            RF.material = turnSignalOFF;

            // makes the indicator flicker
            float floor = 0f;
            float ceiling = 1f;
            float emission = floor + Mathf.PingPong(Time.time * 2f, ceiling - floor);
            LB.material.SetColor("_EmissionColor", new Color(1f, 1f, 1f) * emission);
            LF.material.SetColor("_EmissionColor", new Color(1f, 1f, 1f) * emission);
        } else if (signalingRight)
        {
            RB.material = turnSignalON;
            RF.material = turnSignalON;
            LB.material = turnSignalOFF;
            LF.material = turnSignalOFF;

            float floor = 0f;
            float ceiling = 1f;
            float emission = floor + Mathf.PingPong(Time.time * 2f, ceiling - floor);
            RB.material.SetColor("_EmissionColor", new Color(1f, 1f, 1f) * emission);
            RF.material.SetColor("_EmissionColor", new Color(1f, 1f, 1f) * emission);
        } else
        {
            RB.material = turnSignalOFF;
            RF.material = turnSignalOFF;
            LB.material = turnSignalOFF;
            LF.material = turnSignalOFF;
        }

		
	}




}
