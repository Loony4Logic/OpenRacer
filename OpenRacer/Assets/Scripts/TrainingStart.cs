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
    GameObject startModal;
    [SerializeField]
    TMP_InputField trackNameInput;
    [SerializeField]
    TMP_InputField batchSizeInput;
    [SerializeField]
    TMP_InputField epochInput;
    [SerializeField]
    TMP_InputField sessionTimeInput;

    [SerializeField]
    GameObject TrainingMonitorUI;

    // Start is called before the first frame update
    async void Start()
    {
        startModal.SetActive(true);
        TrainingMonitorUI.SetActive(false);
        //TODO: add a loading screen while it connects to server
        //TODO: display server connection status
        serverConnector = new ServerConnector();
        await serverConnector.Start();

        interactionManager = new InteractionManager(serverConnector);

        TrackGenerator trackGenerator = track.GetComponent<TrackGenerator>();

        carManager = gameObject.GetComponent<CarManager>();
        carManager.interactionManager = interactionManager;

        trainingMonitor = GetComponent<TrainingMonitor>();
        trainingMonitor.carManager = carManager;
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

        Vector3 startPoint = trackGenerator.centerLine[trackGenerator.centerLine.Count - 2];
        Vector3 nextPoint = trackGenerator.centerLine[trackGenerator.centerLine.Count - 1];
        carManager.batchSize =  batchSize;
        carManager.Setup(startPoint + new Vector3(0, 2f, 0), nextPoint - startPoint);
        carManager.centerLine = trackGenerator.centerLine;

        startModal.SetActive(false);
        TrainingMonitorUI.SetActive(true);
    }
}
