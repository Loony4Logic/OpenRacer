using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public List<Vector3> centerLine;
    public int carLeader = 0;


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

    public bool _carSetupReady = false;

    public void Setup(Vector3 startpoint, Vector3 direction)
    {
        for(int i = 0; i < batchSize; i++)
        {
            cars.Add(Instantiate(carPrefab, startpoint, Quaternion.LookRotation(direction)));
            cars.Last().GetComponent<CarControl>().carManager = this;
        }
        _carSetupReady=true;
    }

    public void restart()
    {
        carLeader = 0;
        currentCar = 0;
        _carSetupReady = false;
        Vector3 startPosition = centerLine[0] + new Vector3(0,2,0);
        Quaternion rotation = Quaternion.LookRotation(centerLine[1] - centerLine[0]);
        for(int i = 0;i < batchSize; i++)
        {
            GameObject car = cars[i];
            car.GetComponent<CarControl>().all_wheels_on_track = true;
            car.GetComponent<CarControl>().LastCheckpoint = 0;
            Rigidbody carRigidbody = car.GetComponent<Rigidbody>();
            carRigidbody.Sleep();
            carRigidbody.velocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
            carRigidbody.position = startPosition;
            carRigidbody.rotation = rotation;
            carRigidbody.WakeUp();
        }
        updateCamera();
        _carSetupReady = true;
    }

    // Update is called once per frame
    async void FixedUpdate()
    {
        if(currentStep != respondedStep || ManualDrive || interactionManager == null || !_carSetupReady)
            return;
        int maxCheckpoint = 0;
        List<RawState> rawStates = new List<RawState>();
        List<Action> actions;
        currentStep++;
        for (int i = 0;i < batchSize; i++)
        {
            CarControl carControl = cars[i].GetComponent<CarControl>();
            RawState rawState = carControl.getRawState();
            rawStates.Add(rawState);

            if (rawState.closest_waypoints[0] > maxCheckpoint)
            {
                maxCheckpoint = rawState.closest_waypoints[0];
                carLeader = i;
            }
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

    public int getClosestWaypoint(Vector3 position)
    {
        int closestPoint = 0;
        float closestDist = Mathf.Infinity;
        for (int i = 0; i < centerLine.Count; i++)
        {
            float currentDist = Vector3.Distance(centerLine[i], position);
            if (currentDist < closestDist)
            {
                closestDist = currentDist;
                closestPoint = i;
            }
        }
        return closestPoint;
    }
}
