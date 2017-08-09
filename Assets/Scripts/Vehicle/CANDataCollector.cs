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

// my code
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

    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float rotationW;
    public float throttle;
    public bool leftPaddle;
    public bool rightPaddle;
    public float localRotationX;
    public float frontLocalRotationY;

    public string ToCSV()
    {
        /*
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
        string data = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", time, positionX, positionY, positionZ, rotationX, rotationY, rotationZ, rotationW, throttle, leftPaddle, rightPaddle, localRotationX, frontLocalRotationY);        
        
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

    // my code
    private List<GameObject> traffic;
    private float startOfScene;

    void Awake() {
		rb = GetComponent<Rigidbody>();
		vehicleController = GetComponent<VehicleController>();
        dataStream = DataStreamServer.Instance;
        lastYaw = transform.localRotation.eulerAngles.y;
        lastRoll = transform.localRotation.eulerAngles.z;
        DriverCamera driverCam = GetComponent<DriverCamera>();

        // my code
        //string template = "Assets/Scripts/Javier/Recordings/recording{0}.txt";
        //string template = "JavierResources/Recordings/recording{0}.txt";

        traffic = new List<GameObject>();
        startOfScene = Time.time;


        string template = "/recording{0}.txt";
        //Debug.Log(Application.persistentDataPath);
        int i = 1;
        //var myprevcar2 = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/JavierResourcces/Racing/Racing_Car_05/Prefabs/racing_car_05.prefab"); //, typeof(GameObject));

        //var myprevcar2 = (GameObject)Resources.Load("JavierResourcces/Racing/Racing_Car_05/Prefabs/racing_car_05", typeof(GameObject));
        //var mycar2 = Instantiate(myprevcar2, new Vector3(898, 11, 370), Quaternion.identity);
        //if (myprevcar2 == null)
        //Debug.Log("didn't find the car model");
        //mycar2.AddComponent<CSVFileReader>();

        var myprevcar = (GameObject)Resources.Load("JavierResources/Racing/Racing_Car_05/Prefabs/racing_car_05", typeof(GameObject));
        while (File.Exists(Application.persistentDataPath + string.Format(template, i)))
        {
            //GameObject mycar = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Scripts/Javier/Racing/Racing_Car_05/Prefabs/racing_car_05.prefab", typeof(GameObject));
            //var mycar = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Scripts/Javier/Racing/Racing_Car_05/Prefabs/racing_car_05.prefab", typeof(GameObject)), new Vector3(-100, -100, -100), Quaternion.identity);
            //Debug.Log("found a file");
            var mycar = Instantiate(myprevcar, new Vector3(-100 + i*7, -100, -100), Quaternion.identity);
            if (mycar == null)
                Debug.Log("car not instantiated");
            mycar.AddComponent<CSVFileReader>();
            
            //mycar.GetComponent<CSVFileReader>().csvFile = (TextAsset)AssetDatabase.LoadAssetAtPath(string.Format(template, i), typeof(TextAsset));
            //mycar.GetComponent<CSVFileReader>().csvFile = (TextAsset)Resources.Load(string.Format(template, i), typeof(TextAsset));
            mycar.GetComponent<CSVFileReader>().csvFile = File.Open(Application.persistentDataPath + string.Format(template, i), FileMode.Open);
            if (mycar.GetComponent<CSVFileReader>().csvFile == null)
                Debug.Log("file not opened correctly");
            // add script lights if not already included
            traffic.Add(mycar);
            //Debug.Log("no problem here");
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

    private string generateFileName()
    {
        //string template = "Assets/Scripts/Javier/Recordings/recording{0}.txt";
        //string template = "AssetsTest2/Resources/JavierResources/Recordings/recording{0}.txt";
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
    private bool leftP;
    private bool rightP;

    private void OnGUI()
    {
        if (recording)
            GUI.Label(new Rect(Screen.width - 100, Screen.height - 50, 100, 100), "RECORDING");
        if(GUI.Button(new Rect(Screen.width - 200, 50, 175, 30), "Delete Prev Recording"))
        {
            deletePrevRecording();
        }
        if (GUI.Button(new Rect(Screen.width - 200, 90, 175, 30), "Delete All Recordings"))
        {
            deleteAllRecordings();
        }
        //GUI.Label(new Rect(Screen.width - 100, 100, 100, 100), string.Format("{0}",Time.time));
    }

    void Update() {

        if (Input.GetButtonDown("rightRed"))
        {
            if (recording)
            {
                recording = false;
                writer.Close();
            }
            else
            {
                recording = true;
                string path = generateFileName();
                writer = File.CreateText(path);
                //Debug.Log("recording");
            }
        }

        leftP = Input.GetButtonDown("LeftPaddle");
        rightP = Input.GetButtonDown("RightPaddle");
        /*
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



        dataStream.SendAsText(frame);*/

    }





    private void FixedUpdate()
    {
        //float time = Time.time;
        //time -= startOfScene;

        float time = Time.timeSinceLevelLoad;


        // right paddle is button 4
        //left paddle is button 5
        // right red button is 6
        //btter if these are in update()

        //StreamWriter writer = new StreamWriter(path, true);



        //vehicleController.WheelFL.transform.rotation.x,


        FullDataFrame frame = new FullDataFrame()
        {
            time = time,
            cruiseSpeed = 0f,
            rpm = 0f,
            gearPosActual = 0f,
            gearPosTarget = vehicleController.Gear,
            accelleratorPos = 0f,
            deceleratorPos = 0f,
            rollRate = 0f,
            steeringWheelAngle = 0f,
            vehicleSpeed = 0f,
            vehicleSpeedOverGround = 0f,
            wheelSpeedFL = vehicleController.WheelFL.rpm * 60,
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
            leftPaddle = leftP,
            rightPaddle = rightP,
            localRotationX = vehicleController.WheelFL.transform.localEulerAngles.x,
            frontLocalRotationY = vehicleController.WheelFL.transform.localEulerAngles.y
        };

        if (recording)
        {
            writer.WriteLine(frame.ToCSV());
            //AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);
        }

        //dataStream.SendAsText(frame);
    }
}
