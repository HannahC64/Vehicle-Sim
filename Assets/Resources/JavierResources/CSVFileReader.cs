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
        using(StreamReader read = new StreamReader(csvFile, true))
        {
            fileContents = read.ReadToEnd();
        }
        
        records = fileContents.Split('\n');
        length = records.Length;
        horn=GetComponent<ScriptHorn>();
        horn.beep = false;
        
        lights = GetComponent<ScriptLights>();
        lights.braking = false;

        string[] fields = records[index].Split(',');
        startTime = float.Parse(fields[0]);
        Debug.Log("hello");
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
                            Debug.Log("Increase " + increase[3]);
                            Debug.Log("Previous " + prevValues[3]);
                            placeCar(prevValues);

                            i++;
                        }
                        if (i == 8)
                        {
                            Debug.Log("Final " + nextValues[3]);
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
                        }
                        else
                        {
                            for (int z = 1; z < 9; z++)
                            {
                                currentValues[z] = (float.Parse(currentValues[z]) + increase[z]).ToString();
                            }
                            Debug.Log(i + "th " + currentValues[3]);
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
                        }
                        else
                        {
                            for (int z=1; z<9; z++)
                            {
                                currentValues[z] = (float.Parse(currentValues[z]) + increase[z]).ToString();
                            }
                            placeCar(currentValues);
                            i++;
                        }
                        break;
                    case step.half:
                        Debug.Log("half");
                        
                        if (i == 0)
                        {
                            prevValues = records[index].Split(',');
                            nextValues = records[index+1].Split(',');
                            for (int x = 1; x < 9; x++)
                            {
                                currentValues[x] = ((float.Parse(prevValues[x]) + float.Parse(nextValues[x]) )/ 2).ToString();
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
                            i = 0;
                        }
                        else
                        {
                            Debug.Log("should never get here");
                            i=0;
                        }
                        
                        break;
                    case step.stop:
                        break;
                    case step.full:
                        //should never happen
                        Debug.Log("full");
                        blocked = false;
                        break;
                }
            }
            else
            {
                placeCar(records[index].Split(','));
                index++;
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