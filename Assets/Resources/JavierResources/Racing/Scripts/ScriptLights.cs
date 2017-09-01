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
public class ScriptLights : MonoBehaviour
{

    public bool braking;
    public bool signalingLeft;
    public bool signalingRight;
    [Header("Lights")]
    public GameObject backLights;

    // indicators i.e. RB: right-back indicator

    public Light RBLight;
    public Light LBLight;
    public Light RFLight;
    public Light LFLight;

    private float ceiling;
    private float floor;

    void Start()
    {
        ceiling = 70;
        floor = 0;
        backLights.SetActive(false);
        RBLight.intensity=floor;
        LBLight.intensity = floor;
        RFLight.intensity = floor;
        LFLight.intensity = floor;
    }


    void Update()
    {

        backLights.SetActive(braking);

        if (signalingLeft)
        {
            RBLight.intensity=floor;
            RFLight.intensity = floor;

            // makes the indicator flicker

            float emission = floor + Mathf.PingPong(Time.time * 60f, ceiling - floor);
            LBLight.intensity = emission;
            LFLight.intensity = emission;
        }
        else if (signalingRight)
        {
            LBLight.intensity = floor;
            LFLight.intensity = floor;



            float emission = floor + Mathf.PingPong(Time.time * 60f, ceiling - floor);
            RBLight.intensity = emission;
           
            RFLight.intensity = emission;
        }
        else
        {
            RBLight.intensity = floor;
            LBLight.intensity = floor;
            RFLight.intensity = floor;
            LFLight.intensity = floor;
        }


    }




}