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
    ServerConnector serverConnector;

    // Start is called before the first frame update
    async void Start()
    {
        serverConnector = new ServerConnector();
        await serverConnector.Start();

        interactionManager = new InteractionManager(serverConnector);
        
        carManager = gameObject.GetComponent<CarManager>();
        carManager.interactionManager = interactionManager;

        track.GetComponent<TrackGenerator>().setInteractionManager(interactionManager);
        track.GetComponent<TrackGenerator>().generate();
    }

    private void OnDestroy()
    {
        serverConnector.OnDestroy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
