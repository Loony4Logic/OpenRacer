using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    public ServerConnector serverConnector;
    public StateProcessor stateProcessor;

    // Start is called before the first frame update
    void Start()
    {
        serverConnector = new ServerConnector();
        serverConnector.Start();

        stateProcessor = new StateProcessor(serverConnector);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
