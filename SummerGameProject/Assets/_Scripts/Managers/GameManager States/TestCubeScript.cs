using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// THIS SCRIPT IS JUST TO TEST THE GAME STATE MANANGER
/// </summary>
public class TestCubeScript : MonoBehaviour, IStateInterface
{
    public MeshRenderer meshRenderer;
   

    private IEnumerator ChangeColorOverTime() 
    {
        while (true)
        {
            Debug.Log("COLOR CHANGING!");
            meshRenderer.material.color = Random.ColorHSV();

            yield return new WaitForSeconds(2f);
        }
    }


    public void Initialize() 
    {
        StartCoroutine(ChangeColorOverTime());
    }
    public void Pause()
    {

    }
    public void Play() { }

    public void End() 
    
    {
        StopCoroutine(ChangeColorOverTime());
        Destroy(this.gameObject);
    }
}
