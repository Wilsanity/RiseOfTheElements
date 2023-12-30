using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{
    [SerializeField]
    Text inputPrompt;
    [SerializeField]
    CanvasGroup inputAlpha;

    IEnumerator DisplayPrompt()
    {
        do
        {
            inputAlpha.alpha += 0.1f;
            yield return new WaitForSeconds(0.1f);
        } while(inputAlpha.alpha < 1);
        yield return new WaitForSeconds(3);
        do
        {
            inputAlpha.alpha -= 0.2f;
            yield return new WaitForSeconds(0.1f);
        } while (inputAlpha.alpha > 0);
    }

    public void promptPlayer(string textPrompt)
    {
        inputPrompt.text = textPrompt;
        StartCoroutine(DisplayPrompt());
    }

}
