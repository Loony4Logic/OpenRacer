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
    float t = 0.005f;

    public bool _carSetupReady = false;

    public async void Setup(Vector3 startpoint, Vector3 direction)
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
        if (currentCar >= batchSize) return;
        if (followLeader) setCurrentCar(carLeader);
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
