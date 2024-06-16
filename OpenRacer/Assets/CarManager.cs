using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class CarManager : MonoBehaviour
{
    [SerializeField]
    public int batchSize = 1;
    [SerializeField]
    public bool training = false;
    [SerializeField]
    public bool ManualDrive = false;
    
    public GameObject carPrefab;
    public InteractionManager interactionManager;
    public List<GameObject> cars;
    public int currentCar = 0;


    public int currentStep = 0;
    public int respondedStep = 0;

    //Camera settings
    [SerializeField]
    GameObject _camera;
    
    [SerializeField]
    float offset = 5f;

    [SerializeField]
    float elevation = 2f;

    [SerializeField]
    float t = 0.005f;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < batchSize; i++)
        {
            cars.Add(Instantiate(carPrefab, new Vector3(0,2,0), Quaternion.identity));
        }
    }

    // Update is called once per frame
    async void FixedUpdate()
    {
        if(currentStep != respondedStep || ManualDrive || interactionManager == null)
            return;
        List<RawState> rawStates = new List<RawState>();
        List<Action> actions = new List<Action>();
        currentStep++;
        for (int i = 0;i < batchSize; i++)
        {
            CarControl carControl = cars[i].GetComponent<CarControl>();
            RawState rawState = carControl.getRawState();
            rawStates.Add(rawState);
        }
        actions = await interactionManager.sendBatch(rawStates);
        respondedStep++;
        for(int i = 0; i<batchSize; i++)
        {
            cars[i].GetComponent<CarControl>().move(actions[i].y, actions[i].x);
        }
        updateCamera();
    }

    void updateCamera()
    {
        Transform carTransform = cars[currentCar].transform;
        Vector3 target = carTransform.position + carTransform.forward * - offset + carTransform.up * elevation;
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, target, t);
        _camera.transform.LookAt(carTransform.position);
    }

}
