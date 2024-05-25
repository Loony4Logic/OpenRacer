using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    public InteractionManager interactionManager;
    ServerConnector serverConnector;

    // Start is called before the first frame update
    async void Start()
    {
        serverConnector = new ServerConnector();
        await serverConnector.Start();

        interactionManager = new InteractionManager(serverConnector, GetComponent<CarControl>());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
