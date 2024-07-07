using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine.Timeline;

public class TrainingMonitor : MonoBehaviour
{
    [Header("Comman Data")]
    [SerializeField]
    TMP_Text EpochLabel;
    [SerializeField]
    RawImage SessionElapseTimeImage;
    [SerializeField]
    TMP_Dropdown carDropdown;
    [SerializeField]
    TMP_Text CarLeaderLabel;

    [Header("Navbar")]
    [SerializeField]
    TMP_Text trackNameLabel;
    [SerializeField]
    RawImage Network;
    [SerializeField]
    Texture noWIFI;

    [Header("Comparision")]
    [SerializeField]
    GameObject LeaderInfo;
    [SerializeField]
    GameObject ObserverInfo;
    [SerializeField]
    List<string> LeaderDefaultText = new List<string> {"", "Leader: ", "Progress: ", "Speed: ", "Crash Count: " };
    [SerializeField]
    List<string> ObserverDefaultText = new List<string> {"",  "Observer: ", "Progress: ", "Speed: ", "Crash Count: " };


    [Header("Public Settings")]
    [SerializeField]
    UIUtility _UIUtility;

    [Header("Public vars")]
    public string trackName;
    public int batchSize;
    public int epoch;
    public int sessionTotalTime;

    public CarManager carManager;
    public InteractionManager interactionManager;


    string EpochString = "Epoch ";
    int currentEpoch = 0;
    float sessionElapseTime = 0;
    string carLeaderString = "Leader is car";

    public void setTrainingDetails(string trackName, int batchSize, int epoch, int sessionTime)
    {
        this.trackName = trackName;
        this.batchSize = batchSize; 
        this.epoch = epoch;
        this.sessionTotalTime = sessionTime;
        carDropdown.ClearOptions();
        List<string> cars= new List<string>();
        for(int i = 0; i < batchSize; i++)
        {
            cars.Add($"Car{i + 1}");
        }
        carDropdown.AddOptions(cars);
        EpochLabel.text = EpochString + $"{currentEpoch} / {epoch}";
        trackNameLabel.text = $"Track name: {trackName}";
    }

    async void Update()
    {
        updateElapseTime();
        updateDetails();
        updateComparisionPanel();
        updateNetworkStatus();
    }

    void updateNetworkStatus()
    {
        if (interactionManager.status() == System.Net.WebSockets.WebSocketState.Closed) Network.texture = noWIFI;
    }

    void updateDetails()
    {
        if(sessionElapseTime > sessionTotalTime)
        {
            carManager.activeEpochNumber = currentEpoch;
            carManager.isEpochActive = false;
            sessionElapseTime = 0;
            currentEpoch++;
            EpochLabel.text = EpochString+$"{currentEpoch} / {epoch}";
            if (currentEpoch == epoch) 
            {
                _UIUtility.setUI(UIUtility.UINames.TrainingCompleted);
                carManager.end();
            }
        }
        CarLeaderLabel.text = $"{carLeaderString}{carManager.carLeader + 1}";

    }

    void updateElapseTime()
    {

        if (sessionTotalTime == 0) return;
        sessionElapseTime += Time.deltaTime;
        SessionElapseTimeImage.rectTransform.sizeDelta = new Vector2(sessionElapseTime / sessionTotalTime * 300f, 15);
    }

    void updateComparisionPanel()
    {
        string getText(int i, bool isLeader)
        {
            int car = isLeader ? carManager.carLeader : carManager.getCurrentCar();
            CarControl carControl = carManager.cars[car].GetComponent<CarControl>();
            switch (i)
            {
                case 1:
                    return isLeader ? $"Car{carManager.carLeader+1}" : $"Car{carManager.getCurrentCar()+1}";
                case 2:
                    return carControl.progess.ToString("0.00");
                case 3:
                    return carControl.speed.ToString("0.00");
                case 4:
                    return carControl.crashCount.ToString();
                default:
                    return "";
            }
        }

        for(int i = 1; i < 5; i++)
        {
            LeaderInfo.GetComponent<RectTransform>().GetChild(i).gameObject.GetComponent<TMP_Text>().text = $"{LeaderDefaultText[i]} {getText(i, true)}";
            ObserverInfo.GetComponent<RectTransform>().GetChild(i).gameObject.GetComponent<TMP_Text>().text = $"{ObserverDefaultText[i]} {getText(i, false)}";
        }
    }

    public void changeCar(int index)
    {
        carManager.followLeader = false;
        carManager.setCurrentCar(index);
    }

    public void setFollowLeader(bool follow)
    {
        carManager.followLeader = follow;
        carDropdown.interactable = !follow;
        
    }
}
