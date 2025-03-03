using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TestingStart : MonoBehaviour
{
    public InteractionManager interactionManager;
    public CarManager carManager;
    public GameObject track;
    public TestMonitor testMonitor;
    ServerConnector serverConnector;

    [SerializeField]
    TMP_InputField trackNameInput;
    [SerializeField]
    TMP_InputField lapCountInput;
    [SerializeField]
    TMP_InputField sessionTimeInput;
    [SerializeField]
    TMP_InputField URLInput;
    [SerializeField]
    UIUtility _UIUtility;

    // Start is called before the first frame update
    async void Start()
    {
        _UIUtility.setUI(UIUtility.UINames.LoadingScreen);
        serverConnector = new ServerConnector();

        interactionManager = new InteractionManager(serverConnector);

        TrackGenerator trackGenerator = track.GetComponent<TrackGenerator>();

        carManager = gameObject.GetComponent<CarManager>();
        carManager.interactionManager = interactionManager;
        carManager.testing = true;

        testMonitor.carManager = carManager;
        testMonitor.interactionManager = interactionManager;

        _UIUtility.setUI(UIUtility.UINames.StartModal);
    }

    private void OnDestroy()
    {
        serverConnector.OnDestroy();
    }

    public async void startScene()
    {
        string trackName = trackNameInput.text;
        int sessionTime = int.Parse(sessionTimeInput.text);
        int batchSize = 1;
        int lapCount = int.Parse(lapCountInput.text);
        string URL = URLInput.text;

        _UIUtility.setUI(UIUtility.UINames.LoadingScreen);
        Debug.Log($"Track: {trackName}, sessionTime: {sessionTime}");

        serverConnector.setURL(URL);
        bool connection = await serverConnector.Start();
        if (!connection || interactionManager==null) 
        { 
            // TODO: GO to menu page
            Debug.Log("Something Wrong can't connect to server");
            _UIUtility.Alert("Error Unable to connect to Network.\nRetry after checking server.\nCheck server URL.");
            return;
        }
        try
        {
            // Generating Track from Track name
            Track trackVert = await interactionManager.GetTrackVerts(trackName);
            TrackGenerator trackGenerator = track.GetComponent<TrackGenerator>();
            trackGenerator.generate(trackVert.track);
            Debug.Log(trackGenerator.centerLine.ToCommaSeparatedString());
            await interactionManager.sendTrackVerts(trackGenerator.centerLine); // Sending back the center line after scale 

            // initialising base info to start testing/Eval
            Vector3 startPoint = trackGenerator.centerLine[0];
            Vector3 nextPoint = trackGenerator.centerLine[1];
            carManager.batchSize = batchSize;
            await interactionManager.sendDetails(trackName, batchSize, lapCount, sessionTime);
            carManager.Setup(startPoint + new Vector3(0, 2f, 0), nextPoint - startPoint);
            carManager.centerLine = trackGenerator.centerLine;

            // TODO: Test UI ---> 
            testMonitor.setTestingDetails(trackName, lapCount, sessionTime);

            _UIUtility.setUI(UIUtility.UINames.TrainingData);
        }
        catch(Exception  e) 
        {
            Debug.LogException(e);
            _UIUtility.Alert(e.Message);
            return;
        }
    }
}
