/*
 * Copyright (C) 2016, Jaguar Land Rover
 * This program is licensed under the terms and conditions of the
 * Mozilla Public License, version 2.0.  The full text of the
 * Mozilla Public License is at https://www.mozilla.org/MPL/2.0/
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using System;
using System.IO;

public struct FullDataFrame
{
    public float time;
    public float cruiseSpeed;
    public float rpm;
    public float gearPosActual;
    public float gearPosTarget;
    public float accelleratorPos;
    public float deceleratorPos;
    public float rollRate;
    public float steeringWheelAngle;
    public float vehicleSpeed;
    public float vehicleSpeedOverGround;
    public float wheelSpeedFL;
    public float wheelSpeedFR;
    public float wheelSpeedRL;
    public float wheelSpeedRR;
    public float yawRate;
    public float triggeredEvent1TimeStamp;
    public float triggeredEvent2TimeStamp;
    public float triggeredEvent3TimeStamp;

    // added variables JEC
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float rotationW;
    public float throttle;
    public int leftPaddle;
    public int rightPaddle;
    public bool horn;

    public string ToCSV()
    {
        /* This was the old format JEC
        string data = string.Format("EMSSetSpeed, {1:F4}, {0}\n" +
                "EngineSpeed, {2:F4}, {0}\n" +
                "GearPosActual, {3:N}, {0}\n" +
                "GearPosTarget, {4:N}, {0}\n" +
                "AcceleratorPedalPos, {5:F4}, {0}\n" +
                "DeceleratorPedalPos, {6:F4}, {0}\n" +
                "RollRate, {7:F4}, {0}\n" +
                "SteeringWheelAngle, {8:F4}, {0}\n" +
                "VehicleSpeed, {9:F4}, {0}\n" +
                "VehicleSpeedOverGnd, {10:F4}, {0}\n" +
                "WheelSpeedFrL, {11:F4}, {0}\n" +
                "WheelSpeedFrR, {12:F4}, {0}\n" +
                "WheelSpeedReL, {13:F4}, {0}\n" +
                "WheelSpeedReR, {14:F4}, {0}\n" +
                "YawRate, {15:F4}, {0}\n", time, cruiseSpeed, rpm, gearPosActual, gearPosTarget, accelleratorPos, deceleratorPos, rollRate, steeringWheelAngle, vehicleSpeed, vehicleSpeedOverGround, wheelSpeedFL, wheelSpeedFR, wheelSpeedRL, wheelSpeedRR, yawRate);
        */
        string data = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", time, positionX, positionY, positionZ, rotationX, rotationY, rotationZ, rotationW, throttle, leftPaddle, rightPaddle,horn);        
        
        //TODO: handle this better
        if (triggeredEvent1TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent1, 0.0, {0}\n", triggeredEvent1TimeStamp);
        }

        if (triggeredEvent2TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent2, 0.0, {0}\n", triggeredEvent2TimeStamp);
        }

        if (triggeredEvent3TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent3, 0.0, {0}\n", triggeredEvent3TimeStamp);
        }

        return data;
    }

    public string ToICCSV()
    {
        return string.Format(
            "EngineSpeed, {1:F4}, {0}\n" +
            "VehicleSpeed, {2:F4}, {0}\n", time, rpm, vehicleSpeed);
    }
}

public struct TriggeredEventFrame
{

    public float triggeredEvent1TimeStamp;
    public float triggeredEvent2TimeStamp;
    public float triggeredEvent3TimeStamp;

    public string ToCSV()
    {
        string data = "";
        if (triggeredEvent1TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent1, 0.0, {0}\n", triggeredEvent1TimeStamp);
        }

        if (triggeredEvent2TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent2, 0.0, {0}\n", triggeredEvent2TimeStamp);
        }

        if (triggeredEvent3TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent3, 0.0, {0}\n", triggeredEvent3TimeStamp);
        }
        return data;
    }
}

[StructLayout(LayoutKind.Sequential)]
public class CANDataCollector : MonoBehaviour {
	private Rigidbody rb;
	private VehicleController vehicleController;

    private DataStreamServer dataStream;

    public const float sendRate = 0.1f;
    private float lastSend = 0f;


    private float lastYaw = 0f;
    private float lastRoll = 0f;

    private float triggerTimeStamp1 = 0f;
    private float triggerTimeStamp2 = 0f;
    private float triggerTimeStamp3 = 0f;

    // keeps track of all the cars from recordings JEC
    private List<GameObject> traffic;

    void Awake() {
		rb = GetComponent<Rigidbody>();
		vehicleController = GetComponent<VehicleController>();
        dataStream = DataStreamServer.Instance;
        lastYaw = transform.localRotation.eulerAngles.y;
        lastRoll = transform.localRotation.eulerAngles.z;
        DriverCamera driverCam = GetComponent<DriverCamera>();

        /*
         * This part of the code takes care of instantiating a car
         * for each recording that exists. JEC
         */
        traffic = new List<GameObject>();
        string template = "/recording{0}.txt";
        int i = 1;

        /*
         * As of right now, all of the cars use this prefab. 
         * It would be useful to create an array of different
         * prefabs to increase the diversity of models in the scene JEC
         */
        var myprevcar = (GameObject)Resources.Load("JavierResources\\Racing\\Police_car\\PREFABS\\police_car_V1_LAPD", typeof(GameObject));
        Debug.Log(Application.persistentDataPath);
        if (myprevcar == null)
            Debug.Log("Car prefab wasn't found");
        while (File.Exists(Application.persistentDataPath + string.Format(template, i)))
        {
            var mycar = Instantiate(myprevcar, new Vector3(-100 + i*7, -100, -100), Quaternion.identity);
            mycar.AddComponent<CSVFileReader>();
            mycar.GetComponent<CSVFileReader>().csvFile = File.Open(Application.persistentDataPath + string.Format(template, i), FileMode.Open);
            if (mycar.GetComponent<CSVFileReader>().csvFile == null)
                Debug.Log("Couldn't open recording");
            traffic.Add(mycar);
            i++;
        }
    }

    private void OnEnable()
    {
        AppController.Instance.AdminInput.DataStreamEvent1 += OnTriggerEvent1;
        AppController.Instance.AdminInput.DataStreamEvent2 += OnTriggerEvent2;
        AppController.Instance.AdminInput.DataStreamEvent3 += OnTriggerEvent3;
    }

    private void OnDisable()
    {
        if(AppController.IsInstantiated && AppController.Instance.AdminInput != null)
        {
            AppController.Instance.AdminInput.DataStreamEvent1 -= OnTriggerEvent1;
            AppController.Instance.AdminInput.DataStreamEvent2 -= OnTriggerEvent2;
            AppController.Instance.AdminInput.DataStreamEvent3 -= OnTriggerEvent3;
        }
    }

    void OnTriggerEvent1()
    {
        triggerTimeStamp1 = Time.time;
    }

    void OnTriggerEvent2()
    {
        triggerTimeStamp2 = Time.time;
    }

    void OnTriggerEvent3()
    {
        triggerTimeStamp3 = Time.time;
    }

    /*
     * Checks to see what recordings already exist,
     * and then returns a new filename accordingly. JEC
     */
    private string generateFilePath()
    {
        string template = "/recording{0}.txt";
        int i = 1;
        while (File.Exists(Application.persistentDataPath + string.Format(template, i)))
        {
            i++;
        }
        return Application.persistentDataPath + string.Format(template, i);
    }

    private bool deletePrevRecording()
    {
        string template = "/recording{0}.txt";
        int i = 1;
        while (File.Exists(Application.persistentDataPath + string.Format(template, i)))
        {
            i++;
        }
        if (i == 1)
            return false;
        Destroy(traffic[i - 2], 0.1f);
        File.Delete(Application.persistentDataPath + string.Format(template, i - 1));
        return true;
    }

    private void deleteAllRecordings()
    {
        while (deletePrevRecording()) { }
    }

    private StreamWriter writer;
    private bool recording = false;
    // left and right paddle are used as indicators
    private bool leftP = false;
    private bool rightP = false;
    private bool bHorn = false;
    private float actionPerformedAt = 0f;
    private string message;

    // Takes care of buttons and messages on screen regarding recordings JEC
    private void OnGUI()
    {
        if (recording)
            GUI.Label(new Rect(Screen.width - 100, Screen.height - 50, 100, 100), "RECORDING");
        if (GUI.Button(new Rect(Screen.width - 200, 50, 175, 30), "Delete Prev Recording"))
        {
            actionPerformedAt = Time.time;
            if (!recording)
            {
                deletePrevRecording();
                message = "Succesfully deleted the previous recording.";
            } else
            {
                message = "Cannot delete files while recording.";
            }
        }
        if (GUI.Button(new Rect(Screen.width - 200, 90, 175, 30), "Delete All Recordings"))
        {
            actionPerformedAt = Time.time;
            if (!recording)
            {
                deleteAllRecordings();
                message = "Succesfully deleted all the recordings.";
            } else
            {
                message = "Cannot delete files while recording";
            }
        }
        if (actionPerformedAt > 0f && Time.time < actionPerformedAt + 4f)
        {
            GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(1 - (Time.time - actionPerformedAt - 3f)));
            //GUI.Label(new Rect(200, 10, 300, 400), message);
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2, 300, 400), message);
            GUI.color = Color.white;
        }
    }

    /* 
     * All button actions are better handled in Update
     * because FixedUpdate skips frames occasionaly. JEC
     */
    void Update() {

        /*
        * Changes under Edit/Project Settings/Input:
        * -rightRed is set to joystick button 6 or 'r' JEC
        */

        if (Input.GetButtonDown("LeftRed"))
        {
            bHorn = true;
        }


        if (Input.GetButtonDown("RightRed"))
        {
            if (recording)
            {
                recording = false;
                writer.Close();
            } else
            {
                recording = true;
                string path = generateFilePath();
                writer = File.CreateText(path);
            }
        }

        if (Input.GetButtonDown("LeftPaddle"))
        {
            leftP = !leftP;
            rightP = false;
        }
        if (Input.GetButtonDown("RightPaddle"))
        {
            rightP = !rightP;
            leftP = false;
        }

        /* 
         * This was the old function. I have kept it because it is useful 
         * to see how some information about the car can be accessed.
         * For instance, if the speed of the car ever needs to be recorded,
         * this is how it would be done. JEC

        float yaw = (transform.localRotation.eulerAngles.y - lastYaw) / Time.deltaTime;
        float roll = (transform.localRotation.eulerAngles.z - lastRoll) / Time.deltaTime;
        lastRoll = transform.localRotation.eulerAngles.z;
        lastYaw = transform.localRotation.eulerAngles.y;

        if (Time.time - lastSend < sendRate)
            return;

        float time = Time.time;

        //GearPosActual
        int gear = vehicleController.IsShifting ? -3 : vehicleController.Gear;

        //AcceleratorPedalPos
        int pedalPos = Mathf.RoundToInt(Mathf.Clamp01(vehicleController.accellInput) * 100);

        //DeceleratorPedalPos
        int brakePos = Mathf.RoundToInt(Mathf.Clamp(-1, 0, vehicleController.accellInput) * 100);

        //SteeringWheelAngle
        int wheelAngle = Mathf.RoundToInt(vehicleController.steerInput * 720);

        //VehicleSpeed
        float kmh = rb.velocity.magnitude * 3.6f;
        //TODO: calculate this from wheel rpm

        FullDataFrame frame = new FullDataFrame()
        {
            time = time,
            cruiseSpeed = 0f,
            rpm = vehicleController.RPM,
            gearPosActual = gear,
            gearPosTarget = vehicleController.Gear,
            accelleratorPos = pedalPos,
            deceleratorPos = brakePos,
            rollRate = roll,
            steeringWheelAngle = wheelAngle,
            vehicleSpeed = kmh,
            vehicleSpeedOverGround = kmh,
            wheelSpeedFL = vehicleController.WheelFL.rpm * 60,
            wheelSpeedFR = vehicleController.WheelFR.rpm * 60,
            wheelSpeedRL = vehicleController.WheelRL.rpm * 60,
            wheelSpeedRR = vehicleController.WheelRR.rpm * 60,
            yawRate = yaw,
            triggeredEvent1TimeStamp = 0f
        };

        if(triggerTimeStamp1 > 0f || triggerTimeStamp2 > 0f || triggerTimeStamp3 > 0f)
        {
            TriggeredEventFrame eventFrame = new TriggeredEventFrame()
            {
                triggeredEvent1TimeStamp = triggerTimeStamp1,
                triggeredEvent2TimeStamp = triggerTimeStamp2,
                triggeredEvent3TimeStamp = triggerTimeStamp3
            };

            dataStream.SendAsText(eventFrame);
        }

        //add triggered events if required
        if(triggerTimeStamp1 > 0f)
        {
            frame.triggeredEvent1TimeStamp = triggerTimeStamp1;
            triggerTimeStamp1 = 0f;
        }
        if (triggerTimeStamp2 > 0f)
        {
            frame.triggeredEvent2TimeStamp = triggerTimeStamp2;
            triggerTimeStamp2 = 0f;
        }
        if (triggerTimeStamp3 > 0f)
        {
            frame.triggeredEvent3TimeStamp = triggerTimeStamp3;
            triggerTimeStamp3 = 0f;
        }

        dataStream.SendAsText(frame);
        */
    }

    private void FixedUpdate()
    {
        /*
         * It is important to use the time since the level
         * was loaded as oppoed to the global application time.
         * This way the cars are in synch with the scene. JEC
         */
        float time = Time.timeSinceLevelLoad;

        // most of these parameters are not being recorded JEC
        FullDataFrame frame = new FullDataFrame()
        {
            time = time,
            cruiseSpeed = 0f,
            rpm = 0f,
            gearPosActual = 0f,
            gearPosTarget = 0f,
            accelleratorPos = 0f,
            deceleratorPos = 0f,
            rollRate = 0f,
            steeringWheelAngle = 0f,
            vehicleSpeed = 0f,
            vehicleSpeedOverGround = 0f,
            wheelSpeedFL = 0f,
            wheelSpeedFR = vehicleController.WheelFR.rpm * 60,
            wheelSpeedRL = vehicleController.WheelRL.rpm * 60,
            wheelSpeedRR = vehicleController.WheelRR.rpm * 60,
            yawRate = 0f,
            triggeredEvent1TimeStamp = 0f,
            positionX = vehicleController.transform.position.x,
            positionY = vehicleController.transform.position.y,
            positionZ = vehicleController.transform.position.z,
            rotationX = vehicleController.transform.rotation.x,
            rotationY = vehicleController.transform.rotation.y,
            rotationZ = vehicleController.transform.rotation.z,
            rotationW = vehicleController.transform.rotation.w,
            throttle = vehicleController.accellInput,
            leftPaddle = leftP ? 1 : 0,
            rightPaddle = rightP ? 1 : 0,
            horn= bHorn ? true: false

        };
        bHorn = false;
        if (recording)
        {
            writer.WriteLine(frame.ToCSV());
        }
    }
}
