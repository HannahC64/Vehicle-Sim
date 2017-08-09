using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CSVFileReader : MonoBehaviour
{

    //public TextAsset csvFile;
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
        //records = csvFile.text.Split('\n');
        string csvFile2 = "";
        using(StreamReader read = new StreamReader(csvFile, true))
        {
            csvFile2 = read.ReadToEnd();
        }

        records = csvFile2.Split('\n');

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

    private void FixedUpdate()
    {
        if (startTime < Time.timeSinceLevelLoad)
        {
            readCSV();
            index++;
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

        lights.braking = (float.Parse(fields[8]) < 0);

        if ("True".Equals(fields[9])) { }
            //lights.leftSignalOn = !lights.leftSignalOn;

        if ("True".Equals(fields[10])) { }
            //lights.rightSignalOn = !lights.rightSignalOn;

        //float newX = float.Parse(fields[11]);
        //float newY = float.Parse(fields[12]);

        //fl.localRotation = Quaternion.Euler(newX, newY, 0);
        //fr.localRotation = Quaternion.Euler(newX, newY, 0);
        //rl.localRotation = Quaternion.Euler(newX, 0, 0);
        //rr.localRotation = Quaternion.Euler(newX, 0, 0);
    }
}