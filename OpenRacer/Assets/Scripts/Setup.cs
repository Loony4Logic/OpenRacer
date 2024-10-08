using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Setup : MonoBehaviour
{
    public InteractionManager interactionManager;
    public CarManager carManager;
    public GameObject track;
    public UIInteraction uiInteraction;
    ServerConnector serverConnector;

    // Start is called before the first frame update
    async void Start()
    {
        serverConnector = new ServerConnector();
        await serverConnector.Start();

        interactionManager = new InteractionManager(serverConnector);

        TrackGenerator trackGenerator = track.GetComponent<TrackGenerator>();
        
        carManager = gameObject.GetComponent<CarManager>();
        carManager.interactionManager = interactionManager;

        uiInteraction.interactionManager = interactionManager;
        uiInteraction.trackGenerator = trackGenerator;
        uiInteraction.carManager = carManager;
    }

    private void OnDestroy()
    {
        serverConnector.OnDestroy();
    }
}
