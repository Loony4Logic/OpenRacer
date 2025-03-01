using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestMonitor : MonoBehaviour
{
    [Header("Labels")]
    [SerializeField]
    TMP_Text lapLabel;
    [SerializeField]
    RawImage sessionElapseTimeImage;
    [SerializeField]
    TMP_Text progressLabel;
    [SerializeField]
    TMP_Text speedLabel;
    [SerializeField]
    TMP_Text crashCount;

    [Header("Navbar")]
    [SerializeField]
    TMP_Text trackNameLabel;
    [SerializeField]
    RawImage Network;
    [SerializeField]
    Texture noWIFI;

    [Header("End Data")]
    [SerializeField]
    TMP_Text TrackNameValue;
    [SerializeField]
    TMP_Text LapCountValue;
    [SerializeField]
    TMP_Text CrashCountValue;
    [SerializeField]
    TMP_Text SessionTimeValue;


    [Header("Public Settings")]
    [SerializeField]
    UIUtility _UIUtility;

    [Header("Public vars")]
    public string trackName;
    public int laps;
    public int sessionTotalTime;

    public CarManager carManager;
    public CarControl carControl;
    public InteractionManager interactionManager;

    string lapString = "Laps: ";
    string progressString = "Progress: ";
    string speedString = "Speed: ";
    string crashString = "Crash Count: ";
    int currentLap = 0;
    float sessionElapseTime = 0;

    bool systemReady = false;

    public void setTestingDetails(string trackName, int lapCount, int sessionTime)
    { 
        this.trackName = trackName;
        this.laps = lapCount;
        this.sessionTotalTime = sessionTime;

        lapLabel.text = $"{lapString}: {currentLap}/{lapCount}";

        TrackNameValue.text = trackName;
        trackNameLabel.text = $"Track Name: {trackName}";
        SessionTimeValue.text = sessionTime.ToString();
        carControl = carManager.cars[0].GetComponent<CarControl>();
        systemReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!systemReady || carControl == null) return;
        updateElapseTime();
        updateNetworkStatus();
        updateDetails();
    }

    void updateDetails()
    {

        if (sessionElapseTime > sessionTotalTime)
        {
            // End the Eval
            carManager.end();
            // Show end UI
            LapCountValue.text = carControl.lapCount.ToString();
            CrashCountValue.text = carControl.crashCount.ToString();
            SessionTimeValue.text = sessionTotalTime.ToString();
            _UIUtility.setUI(UIUtility.UINames.TrainingCompleted);
        }
        lapLabel.text = $"{lapString} {carControl.lapCount}/{this.laps}";
        Debug.Log($"{lapString} {carControl.lapCount}/{this.laps}");
        progressLabel.text =  $"{progressString} {carControl.progess.ToString("0.00")}";
        speedLabel.text = $"{speedString} {carControl.speed.ToString("0.00")}";
        crashCount.text = $"{crashString} {carControl.crashCount}";
    }

    void updateElapseTime()
    {

        if (sessionTotalTime == 0) return;
        sessionElapseTime += Time.deltaTime;
        sessionElapseTimeImage.rectTransform.sizeDelta = new Vector2(sessionElapseTime / sessionTotalTime * 300f, 15);
    }

    void updateNetworkStatus()
    {
        if (interactionManager.status() == System.Net.WebSockets.WebSocketState.Closed) Network.texture = noWIFI;
    }
}
