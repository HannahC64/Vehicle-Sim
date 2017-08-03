using System.Collections.Generic;
using UnityEngine;
using System.Collections;




public class ScriptLights : MonoBehaviour {

	//public bool blink;
	[Header("Lights")]
	public bool frontLightsOn;
	public bool backLightsOn;

    public bool leftSignalOn;
    public bool rightSignalOn;

	public GameObject frontLights;
	public GameObject backLights;
    public GameObject brakeLights;
    public Renderer leftSignal;
    //public GameObject rightSignal;

	private IEnumerator coroutine;

    private CSVFileReader csvFile;

	void Start () {
		//blink 			= false;
		frontLightsOn 	= false;
		backLightsOn 	= false;

        csvFile = GetComponent<CSVFileReader>();
        /*
        backLights.SetActive(false);
        frontLights.SetActive(false);
        brakeLights.SetActive(false);
        */

        //leftSignal.enabled = false;
        //leftSignal.SetActive(leftSignalOn);
        //rightSignal.SetActive(rightSignalOn);
		
        //coroutine = WaitAndPrint(6.0f);
		//StartCoroutine(coroutine);
	}
	

	void Update () {
        /*
        brakeLights.SetActive(csvFile.braking);
        //leftSignal.SetActive(leftSignalOn);



        if (leftSignalOn || rightSignalOn)
        {
            //leftSignal.enabled = true;
            float floor = 0f;
            float ceiling = 3f;
            float emission = floor + Mathf.PingPong(Time.time * 3f, ceiling - floor);
            leftSignal.material.SetColor("_EmissionColor", new Color(1f, 1f, 1f) * emission);
        } else
        {
            leftSignal.material.SetColor("_EmissionColor", new Color(0f, 0f, 0f) * 0f);
        }
        */
        
    }





    /*
	public void TurnOnFrontLights()
	{
		if (frontLightsOn) {
			frontLighs.SetActive (true);
		} else {
			frontLighs.SetActive (false);
		}
	}

	public void TurnOnBackLights()
	{
		if (backLightsOn) {
			backLights.SetActive (true);
		} else {
			backLights.SetActive (false);
		}
	}



	private IEnumerator WaitAndPrint(float waitTime) {
		while (true) {
			yield return new WaitForSeconds(waitTime);
			frontLightsOn = !frontLightsOn;
			backLightsOn = !backLightsOn;
		}
	}
    */


}
