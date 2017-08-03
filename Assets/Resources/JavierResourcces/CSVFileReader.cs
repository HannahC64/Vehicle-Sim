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

    public bool braking;

    private float startTime;

    private Transform fl;
    private Transform fr;
    private Transform rl;
    private Transform rr;

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

        braking = false;

        string[] fields = records[index].Split(',');
        startTime = float.Parse(fields[0]);
        //fl = transform.Find("wheels/PivotFL");
        //fr = transform.Find("wheels/PivotFR");
        //rl = transform.Find("wheels/PivotRL");
        //rr = transform.Find("wheels/PivotRR");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        //Debug.Log("reading file");
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
        //Debug.Log("it's somewhere");
        //if (float.Parse(fields[0]) < Time.time)
        //{
            
            Vector3 newPos = new Vector3(float.Parse(fields[1]), float.Parse(fields[2]), float.Parse(fields[3]));
            transform.position = newPos;

            Quaternion rot = new Quaternion(float.Parse(fields[4]), float.Parse(fields[5]), float.Parse(fields[6]), float.Parse(fields[7]));
            transform.rotation = rot;

            braking = (float.Parse(fields[8]) < 0);



            if ("True".Equals(fields[9]))
                lights.leftSignalOn = !lights.leftSignalOn;

            if ("True".Equals(fields[10]))
                lights.rightSignalOn = !lights.rightSignalOn;

            //float newX = float.Parse(fields[11]);
            //float newY = float.Parse(fields[12]);

            //fl.localRotation = Quaternion.Euler(newX, newY, 0);
            //fr.localRotation = Quaternion.Euler(newX, newY, 0);
            //rl.localRotation = Quaternion.Euler(newX, 0, 0);
            //rr.localRotation = Quaternion.Euler(newX, 0, 0);

        //}
    }

    /*
    private void OnGUI()
    {

        GUI.Label(new Rect(Screen.width - 100, 150, 100, 100), string.Format("CSV File Time: {0}", Time.time));


    }*/
}