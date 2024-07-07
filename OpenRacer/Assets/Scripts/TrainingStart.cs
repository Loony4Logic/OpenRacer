using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using TMPro;

public class TrainingStart : MonoBehaviour
{
    public InteractionManager interactionManager;
    public CarManager carManager;
    public GameObject track;
    public TrainingMonitor trainingMonitor;
    ServerConnector serverConnector;

    [SerializeField]
    TMP_InputField trackNameInput;
    [SerializeField]
    TMP_InputField batchSizeInput;
    [SerializeField]
    TMP_InputField epochInput;
    [SerializeField]
    TMP_InputField sessionTimeInput;
    [SerializeField]
    UIUtility _UIUtility;
    
    // Start is called before the first frame update
    async void Start()
    {
        _UIUtility.setUI(UIUtility.UINames.LoadingScreen);   
        //TODO: display server connection status
        //TODO: if server load fails then display that
        //TODO: keep server connected if last message was a while back just ping the server 
        serverConnector = new ServerConnector();

        interactionManager = new InteractionManager(serverConnector);

        TrackGenerator trackGenerator = track.GetComponent<TrackGenerator>();

        carManager = gameObject.GetComponent<CarManager>();
        carManager.interactionManager = interactionManager;

        trainingMonitor.carManager = carManager;
        trainingMonitor.interactionManager = interactionManager;
        
        _UIUtility.setUI(UIUtility.UINames.StartModal);
    }

    private void OnDestroy()
    {
        serverConnector.OnDestroy();
    }

    public async void startScene()
    {
        string trackName = trackNameInput.text;
        int batchSize = int.Parse(batchSizeInput.text);
        int epoch = int.Parse(epochInput.text);
        int sessionTime = int.Parse(sessionTimeInput.text);
        _UIUtility.setUI(UIUtility.UINames.LoadingScreen);
        await serverConnector.Start();
        Debug.Log($"Track: {trackName}, batchSize: {batchSize}, epoch: {epoch}, sessionTime: {sessionTime}");

        trainingMonitor.setTrainingDetails(trackName, batchSize, epoch, sessionTime);

        if (interactionManager == null)
        {
            Debug.Log("Something Wrong can't connect to server");
            return;
        }

        Track trackVert = await interactionManager.GetTrackVerts(trackName);
        TrackGenerator trackGenerator = track.GetComponent<TrackGenerator>();
        trackGenerator.generate(trackVert.track);

        Vector3 startPoint = trackGenerator.centerLine[0];
        Vector3 nextPoint = trackGenerator.centerLine[1];
        carManager.batchSize =  batchSize;
        carManager.Setup(startPoint + new Vector3(0, 2f, 0), nextPoint - startPoint);
        carManager.centerLine = trackGenerator.centerLine;

        _UIUtility.setUI(UIUtility.UINames.TrainingData);
    }
}
