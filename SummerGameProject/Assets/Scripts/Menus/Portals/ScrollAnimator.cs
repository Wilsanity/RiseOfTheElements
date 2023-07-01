using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollAnimator : MonoBehaviour
{
    RectTransform main;
    [SerializeField] RectTransform top, bottom;

    Vector3 posVel;
    float scrollVel;

    [SerializeField] float scrollMaxSpeed;
    [SerializeField] float time;

    private void Awake()
    {
        main = GetComponent<RectTransform>();
        StartCoroutine(SetPosition(new Vector3(10, 10, 0), Quaternion.Euler(0, 0, 45), 0, time));
    }

    private void Update()
    {
        top.localPosition = new Vector3(0, (main.rect.height / 2), 0);
        bottom.localPosition = new Vector3(0, -(main.rect.height / 2), 0);
    }
    
    public IEnumerator SetPosition(Vector3 pos, Quaternion rot, float deltaY, float time)
    {
        yield return new WaitForSeconds(1f);

        if (main.sizeDelta.y > deltaY)
            while (main.sizeDelta.y.ToString("0.0") != deltaY.ToString("0.0"))
            {
                main.sizeDelta = new Vector2(main.sizeDelta.x, Mathf.SmoothDamp(main.sizeDelta.y, deltaY, ref scrollVel, time));
                yield return new WaitForEndOfFrame();
            }

        while (main.position.ToString("0.0") != pos.ToString("0.0") && main.rotation.ToString("0.0") != rot.ToString("0.0"))
        {
            main.position = Vector3.SmoothDamp(main.position, pos, ref posVel, time);
            main.rotation = Quaternion.Lerp(main.rotation, rot, Time.deltaTime * Mathf.Pow(time, -1));
            yield return new WaitForEndOfFrame();
        }
    }
}
