using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptHorn : MonoBehaviour {

    public AudioSource audioSource;
    public bool beep;

    void Start ()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (beep)
        {
            audioSource.Play();
         
        }
   
        

    }
}
