using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net.NetworkInformation;
using Unity.VisualScripting;

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
    [SerializeField]
    UIUtility _UIUtility;

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
    }

    async void LateUpdate()
    {
        if (sessionTotalTime == 0) return;
        sessionElapseTime += Time.deltaTime;
        SessionElapseTimeImage.rectTransform.sizeDelta = new Vector2(sessionElapseTime/ sessionTotalTime * 300f, 15);
        
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

    public void changeCar(int index)
    {
        carManager.followLeader = false;
        carManager.currentCar = index;
    }

    public void setFollowLeader(bool follow)
    {
        carManager.followLeader = follow;
        carDropdown.interactable = !follow;
        
    }
}
