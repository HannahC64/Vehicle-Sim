using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCollide : MonoBehaviour {
    public AudioSource audioSource;
    private CSVFileReader reader;

    // To get this script to work for a new car model, the car must have 3 colliders
    // 1. One normal collider (Not trigger) attached to the car chasis mesh
    // 2. Another mesh collider on the car, but this one IsTrigger and should have a rigidBody component attached (make sure useGravity and IsKinematic are both left unticked)
    // 3. A frontView mesh that is a box in front of the car, and a little bit wider than it. Should be IsTrigger and have this script attached as a component

    //See Racing Car scene for an example
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        reader = gameObject.GetComponentInParent<CSVFileReader>();
    }

    void OnTriggerEnter(Collider c)
    {
        audioSource.Play();
       if (reader != null)
        {
            reader.blocked=true;
            reader.s = CSVFileReader.step.half;
            reader.i = 0;
            reader.decelerate = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited");
        reader.decelerate = false;
        reader.s = CSVFileReader.step.eigth;
    }

}
