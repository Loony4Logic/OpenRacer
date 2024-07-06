using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoaderAnimation : MonoBehaviour
{
    [SerializeField]
    RawImage block1;
    [SerializeField]
    RawImage block2;
    [SerializeField]
    TMP_Text loadingHint;
    [SerializeField]
    float hintDuration = 1f;

    List<string> Hints = new List<string>{"Make sure your model is not over train on a single track to avoid overfitting.", "Don't make too many tight turns."};
    bool upcycle = true;
    float speed = 0.5f;
    float timeElapsed = 0;

    Vector2 getNextPosition(Vector2 position, bool upcycle)
    {
        if (upcycle) position.y += 1*speed;
        else position.y -= 1*speed;
        return position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 refPosition = block1.rectTransform.localPosition;
        if(refPosition.y <= -50) upcycle = true;
        if(refPosition.y >= 50) upcycle = false;
        block1.rectTransform.localPosition = getNextPosition(block1.rectTransform.localPosition, upcycle);
        block2.rectTransform.localPosition = getNextPosition(block2.rectTransform.localPosition, !upcycle);
        timeElapsed += Time.deltaTime;
        if (timeElapsed > hintDuration)
        {
            timeElapsed = 0;
            loadingHint.text = Hints[Random.Range(0, Hints.Count)];
        }
    }
}
