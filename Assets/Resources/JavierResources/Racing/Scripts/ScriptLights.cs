using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class ScriptLights : MonoBehaviour {

    public bool braking;
	[Header("Lights")]
	public bool frontLightsOn;
	public bool backLightsOn;
	public GameObject frontLighs;
    public GameObject backLights;


	void Start () {
		frontLightsOn 	= false;
		backLightsOn 	= false;
	}
	

	void Update () {

		
	}




}
