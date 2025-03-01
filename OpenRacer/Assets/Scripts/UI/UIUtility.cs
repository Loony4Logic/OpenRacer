using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIUtility : MonoBehaviour
{
    [SerializeField]
    GameObject UIContainer;
    public enum UINames { LoadingScreen, StartModal, TrainingData, TrainingCompleted };

    List<GameObject> UIs = new List<GameObject>();

    void getChilds(GameObject _gameObject)
    {
        for (int i = 0; i < _gameObject.transform.childCount; i++)
        {
            UIs.Add(_gameObject.transform.GetChild(i).gameObject);
        }
    }

    public void setUI(UINames UI)
    {
        for (int i = 0; i < UIs.Count; i++)
        {
            UIs[i].SetActive(false);
        }
        UIs[(int)UI].SetActive(true);
    }

    void Awake()
    {
        getChilds(UIContainer);
    }

}
