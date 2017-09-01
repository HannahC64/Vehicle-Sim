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
    private int lag;

    //These are all values for the collision detection
    private string[] prevValues;
    private string[] nextValues;
    private string[] currentValues;
    private float[] increase;
    public enum step { half, quarter, eigth, full, stop };
    public step s; //step is the fractional increase for each case
    public int i;
    public bool blocked;//true if another car is in this car's collision
    public bool decelerate;//true for slowing down, false for speeding up after collision ends

    private ScriptLights lights;
    private float startTime;
    private ScriptHorn horn;


    // Use this for initialization
    void Start()
    {
        lag = 0;
        i = 0;
        s = step.full;
        blocked = false;
        decelerate = false;
        index = 0;
        prevValues = new string[12];
        nextValues = new string[12];
        currentValues = new string[12];
        increase = new float[9];
        /*
         * This string uses a lot of memory if the recording is very long.
         * The way the file is being read can thus be improved.
         */
        string fileContents = "";
        using (StreamReader read = new StreamReader(csvFile, true))
        {
            fileContents = read.ReadToEnd();
        }

        records = fileContents.Split('\n');
        length = records.Length;
        horn = GetComponent<ScriptHorn>();
        horn.beep = false;

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
            if (blocked == true)
            {
                if (index + 1 >= length - 1)
                    index = 0;
                if (records[index].Split(',').Length < 12 || records[index + 1].Split(',').Length < 12)
                {
                    index++;
                }
                else
                {

                    switch (s)
                    {
                        // This is triggered by the viewCollide script, which is attached to a collider in front of the car prefab. 
                        // Look at viewCollide for more details
                        case step.eigth:
                            if (i == 0)
                            {
                                prevValues = records[index].Split(',');
                                prevValues.CopyTo(currentValues, 0);
                                nextValues = records[index + 1].Split(',');
                                for (int y = 1; y < 9; y++)
                                {
                                    increase[y] = (float.Parse(nextValues[y]) - float.Parse(prevValues[y])) / 8;
                                }

                                placeCar(prevValues);

                                i++;
                            }
                            if (i == 8)
                            {

                                placeCar(nextValues);
                                if (decelerate)
                                {
                                    s = step.stop;
                                }
                                else
                                {
                                    s = step.quarter;
                                }

                                i = 0;
                                index++;
                                lag--;
                            }
                            else
                            {
                                for (int z = 1; z < 9; z++)
                                {
                                    currentValues[z] = (float.Parse(currentValues[z]) + increase[z]).ToString();
                                }
                                placeCar(currentValues);
                                i++;
                            }
                            break;
                        case step.quarter:

                            if (i == 0)
                            {


                                prevValues = records[index].Split(',');
                                prevValues.CopyTo(currentValues, 0);
                                nextValues = records[index + 1].Split(',');
                                for (int y = 1; y < 9; y++)
                                {
                                    increase[y] = (float.Parse(nextValues[y]) - float.Parse(prevValues[y])) / 4;
                                }

                                placeCar(prevValues);

                                i++;
                            }
                            if (i == 4)
                            {

                                placeCar(nextValues);
                                if (decelerate)
                                {
                                    s = step.eigth;
                                }
                                else
                                {
                                    s = step.half;
                                }

                                i = 0;
                                index++;
                                lag--;
                            }
                            else
                            {
                                for (int z = 1; z < 9; z++)
                                {
                                    currentValues[z] = (float.Parse(currentValues[z]) + increase[z]).ToString();
                                }
                                placeCar(currentValues);
                                i++;
                            }
                            break;
                        case step.half:
                            if (i == 0)
                            {
                                prevValues = records[index].Split(',');
                                nextValues = records[index + 1].Split(',');
                                for (int x = 1; x < 9; x++)
                                {
                                    currentValues[x] = ((float.Parse(prevValues[x]) + float.Parse(nextValues[x])) / 2).ToString();
                                }
                                currentValues[9] = prevValues[9];
                                currentValues[10] = prevValues[10];
                                currentValues[11] = false.ToString();
                                placeCar(prevValues);

                                i++;

                            }
                            else if (i == 1)
                            {

                                placeCar(currentValues);
                                i++;
                            }
                            else if (i == 2)
                            {

                                placeCar(nextValues);
                                if (decelerate)
                                {
                                    s = step.quarter;
                                }
                                else
                                {
                                    blocked = false;
                                    s = step.full;
                                }

                                index++;
                                lag--;
                                i = 0;
                            }
                            else
                            {
                                i = 0;
                            }

                            break;
                        case step.stop:
                            break;
                        case step.full:
                            //should never happen
                            blocked = false;
                            break;
                    }
                }
                //lag increases for each frame it gets behind on the recording
                lag++;
            }
            else
            {

                while (records[index].Split(',').Length < 12)
                {
                    index++;
                    if (index >= length - 1)
                        index = 0;
                }
                placeCar(records[index].Split(','));
                index++;
                if (lag > 0)
                {
                    //goes at double speed to catch up with where it should be
                    index++;
                    lag--;
                }

                // once the recording is over, this loops back to the beginning
                if (index >= length - 1)
                    index = 0;

            }
            
        }
    }

    void placeCar(string[] fields)
    {
        Vector3 newPos = new Vector3(float.Parse(fields[1]), float.Parse(fields[2]), float.Parse(fields[3]));
        transform.position = newPos;

        Quaternion rot = new Quaternion(float.Parse(fields[4]), float.Parse(fields[5]), float.Parse(fields[6]), float.Parse(fields[7]));
        transform.rotation = rot;

        // if throttle is negative, the braking pedal is being used
        lights.braking = (float.Parse(fields[8]) < 0);

        lights.signalingLeft = (int.Parse(fields[9]) == 1);
        lights.signalingRight = (int.Parse(fields[10]) == 1);

        horn.beep = (bool.Parse(fields[11]));
    }
}