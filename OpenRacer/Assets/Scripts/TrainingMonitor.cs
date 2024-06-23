using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net.NetworkInformation;

public class TrainingMonitor : MonoBehaviour
{
    [SerializeField]
    TMP_Text EpochLabel;
    [SerializeField]
    RawImage SessionElapseTimeImage;
    [SerializeField]
    TMP_Dropdown carDropdown;
    [SerializeField]
    TMP_Text CarLeaderLabel;

    public string trackName;
    public int batchSize;
    public int epoch;
    public int sessionTotalTime;

    public CarManager carManager;

    string EpochString = "Epoch ";
    int currentEpoch = 0;
    float sessionElapseTime = 0;
    string carLeaderString = "Leader is car";

    public void setTrainingDetails(string name, int batchSize, int epoch, int sessionTime)
    {
        this.name = name;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (sessionTotalTime == 0) return;
        sessionElapseTime += Time.deltaTime;
        SessionElapseTimeImage.rectTransform.sizeDelta = new Vector2(sessionElapseTime/ sessionTotalTime * 300f, 15);
        
        if(sessionElapseTime > sessionTotalTime)
        {
            sessionElapseTime = 0;
            currentEpoch++;
            EpochLabel.text = EpochString+$"{currentEpoch} / {epoch}";
            carManager.restart();
            //TODO: add end to the training
        }
        CarLeaderLabel.text = $"{carLeaderString}{carManager.carLeader + 1}";
    }

    public void changeCar(int index)
    {
        carManager.currentCar = index;
    }

    public void followLeader(bool follow)
    {
        if (follow)
            carManager.currentCar = carManager.carLeader;
    }
}
