using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/*
 * This script should be attached to a car prefab.
 * It reads the contents of a CSV file and updates
 * the car accordingly.
 * 
 * When a car is instantiated in CANDataCollector,
 * a CSVFileReader is attached to it through code.
 */
public class CSVFileReader : MonoBehaviour
{
    public FileStream csvFile;
    private int index;
    private string[] records;
    private int length;

    private ScriptLights lights;
    private float startTime;

    // Use this for initialization
    void Start()
    {
        index = 0;
        /*
         * This string uses a lot of memory if the recording is very long.
         * The way the file is being read can thus be improved.
         */
        string fileContents = "";
        using(StreamReader read = new StreamReader(csvFile, true))
        {
            fileContents = read.ReadToEnd();
        }
        
        records = fileContents.Split('\n');
        length = records.Length;

        lights = GetComponent<ScriptLights>();
        lights.braking = false;

        string[] fields = records[index].Split(',');
        startTime = float.Parse(fields[0]);
    }

    // Update is called once per frame
    void Update()
    {
    }

    /*
     * FixedUpdate is called in fixed intervals 
     * More reliabled to do it here because the recording is also
     * done under FixedUpdate.
     */
    private void FixedUpdate()
    {
        // waits to start reading until recording started
        if (startTime < Time.timeSinceLevelLoad)
        {
            readCSV();
            index++;
            // once the recording is over, this loops back to the beginning
            if (index >= length - 1)
                index = 0;
        }
    }

    void readCSV()
    {
        string[] fields = records[index].Split(',');

        Vector3 newPos = new Vector3(float.Parse(fields[1]), float.Parse(fields[2]), float.Parse(fields[3]));
        transform.position = newPos;

        Quaternion rot = new Quaternion(float.Parse(fields[4]), float.Parse(fields[5]), float.Parse(fields[6]), float.Parse(fields[7]));
        transform.rotation = rot;

        // if throttle is negative, the braking pedal is being used
        lights.braking = (float.Parse(fields[8]) < 0);

        lights.signalingLeft = (int.Parse(fields[9]) == 1);
        lights.signalingRight = (int.Parse(fields[10]) == 1);
    }
}