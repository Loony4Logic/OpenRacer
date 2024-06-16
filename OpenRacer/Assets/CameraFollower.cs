using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField]
    GameObject car = null;

    [SerializeField]
    float offset = 5f;

    [SerializeField]
    float elevation = 2f;
    
    [SerializeField]
    float t = 0.005f;

    // Start is called before the first frame update
    void Start()
    {
            car = GameObject.Find("car(Clone)");
        Debug.Log(car);
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 target = car.transform.position + car.transform.forward * -offset + car.transform.up * elevation;
        //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target, t);
        //gameObject.transform.LookAt(car.transform.position);
        
    }
}
