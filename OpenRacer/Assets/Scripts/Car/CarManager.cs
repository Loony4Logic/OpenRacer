/*
 * This handles all the cars in the scene. Cars should be generated through this script only. 
 * also handles all the external interaction regarding car action and car state.
 */

using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [SerializeField]
    public int batchSize = 1;
    [SerializeField]
    public bool training = false;
    [SerializeField]
    public bool ManualDrive = false;

    [SerializeField]
    Material transparentMaterial;
    [SerializeField]
    Material regularMaterial;

    public GameObject carPrefab;
    public InteractionManager interactionManager;
    public List<GameObject> cars;
    int currentCar = 0;
    public List<Vector3> centerLine;
    public int carLeader = 0;


    public int currentStep = 0;
    public int respondedStep = 0;
    public bool followLeader = false;
    public bool isEpochActive = true;
    public int activeEpochNumber = 0;

    //Camera settings
    [SerializeField]
    GameObject _camera;
    
    [SerializeField]
    float offset = 5f;

    [SerializeField]
    float elevation = 2f;

    [SerializeField]
    float cameraSmoothFactor = 0.005f;

    public bool _carSetupReady = false;
    public bool testing = false;

    float time = 0.0f;

    public bool debug = false;
    public GameObject dot;
    public List<GameObject> dots;

    public async void Setup(Vector3 startpoint, Vector3 direction)
    {
        for(int i = 0; i < batchSize; i++)
        {
            GameObject car = Instantiate(carPrefab, startpoint, Quaternion.LookRotation(direction));
            cars.Add(car);
            car.GetComponent<CarControl>().carManager = this;
            car.GetComponent<CarControl>().startLap(time);
            if(debug)
                dots.Add(Instantiate(dot, startpoint, Quaternion.LookRotation(direction)));
        }
        _carSetupReady=true;
    }

    public void restart()
    {
        carLeader = 0;
        currentCar = 0;
        setMaterialForAllCars(regularMaterial);
        _carSetupReady = false;


        Vector3 startPoint = centerLine[0];
        Vector3 nextPoint = centerLine[1];
        for(int i = 0;i < batchSize;i++) Destroy(cars[i]);
        cars.Clear();
        Setup(startPoint + new Vector3(0, 2f, 0), nextPoint - startPoint);
        updateCamera();
        _carSetupReady = true;
    }

    public async Task<string> startEpoch(int epoch)
    {
        string ack = await interactionManager.sendEpochEnd(epoch);
        restart();
        isEpochActive = true;
        return ack;
    }

    public void end()
    {
        _carSetupReady = false;
        for(int i = 0; i < batchSize; i++)
        {
            Destroy(cars[i]);
        }
        batchSize =0;
    }

    // Update is called once per frame
    async void FixedUpdate()
    {
        time += Time.deltaTime;
        if(currentStep != respondedStep || ManualDrive || interactionManager == null || !_carSetupReady)
            return;
        if (!isEpochActive)
        {
            await startEpoch(activeEpochNumber);
            return;
        }
        
        int maxCheckpoint = 0;
        List<RawState> rawStates = new List<RawState>();
        List<Action> actions;
        currentStep++;
        for (int i = 0;i < batchSize; i++)
        {
            CarControl carControl = cars[i].GetComponent<CarControl>();
            RawState rawState = carControl.getRawState();
            rawStates.Add(rawState);

            // If car completed a lap
            if(carControl.progess == 100) carControl.endLap(time);
            
            // If car goes ahead of leader.
            if (rawState.closest_waypoints[0] > maxCheckpoint)
            {
                maxCheckpoint = rawState.closest_waypoints[0];
                carLeader = i;
            }
            if (debug) dots[i].GetComponent<Transform>().position = centerLine[(rawState.closest_waypoints[0] + 2)%centerLine.Count];
            carControl.GetComponent<Rigidbody>().Sleep(); // put car to sleep
        }
        
        if (testing)
            actions = await interactionManager.sendForTest(rawStates);
        else
            actions = await interactionManager.sendBatch(rawStates);

        respondedStep++;
        for(int i = 0; i<batchSize; i++)
        {
            cars[i].GetComponent<CarControl>().move(actions[i].y, actions[i].x);
            cars[i].GetComponent<Rigidbody>().WakeUp(); // wake car and make action
        }
        updateCamera();
    }

    void updateCamera()
    {
        if (currentCar >= batchSize) return;
        if (followLeader) setCurrentCar(carLeader);
        Transform carTransform = cars[currentCar].transform;
        Vector3 target = carTransform.position + carTransform.forward * - offset + carTransform.up * elevation;
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, target, cameraSmoothFactor);
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

    public int getCurrentCar()
    {
        return currentCar;
    }

    public void setCurrentCar(int carIndex)
    {
        currentCar = carIndex;
        setMaterialForAllCars(transparentMaterial);
        cars[carIndex].GetComponentInChildren<MeshRenderer>().sharedMaterial = regularMaterial;
    }

    void setMaterialForAllCars(Material _material) 
    {
        for(int i = 0;i<cars.Count;i++) 
        {
            cars[i].GetComponentInChildren<MeshRenderer>().sharedMaterial = _material;
        }

    }

}
