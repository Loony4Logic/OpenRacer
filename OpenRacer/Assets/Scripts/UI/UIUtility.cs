using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIUtility : MonoBehaviour
{
    [SerializeField]
    GameObject UIContainer;

    [SerializeField]
    TMP_Text InfoText;
    public enum UINames { LoadingScreen, StartModal, TrainingData, TrainingCompleted, InfoModal };

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

    public void Alert(string message)
    {
        InfoText.text = message;
        setUI(UINames.InfoModal);
    }

    public void goToMenu()
    {
        SceneManager.LoadSceneAsync("Scenes/Start");
    }
}
