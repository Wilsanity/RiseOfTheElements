using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenuManager : MonoBehaviour
{
    [SerializeField] GameObject saveDataFormPrefab;
    [SerializeField] GameObject contentWindow;
    private PlayerSaveFileData[] saveFiles;

    [SerializeField] MainMenuManager mainMenuManager;

    public void fill_Window_With_Data()
    {
        float totalOffset = 0;
        float hightOffset = saveDataFormPrefab.transform.localScale.y +1 ;
        Vector3 offsetPosiotion = new Vector3(0, 0, 0);
        foreach (PlayerSaveFileData currentFile in saveFiles)
        {
            GameObject newForm = Instantiate(saveDataFormPrefab, contentWindow.transform);
            newForm.transform.position = offsetPosiotion;
            totalOffset += hightOffset;
            offsetPosiotion.y -= hightOffset;

            //fill prefab
            newForm.GetComponentInChildren<RawImage>().texture = currentFile.saveScreenshot; // find way to do this with an image like yo can in the editor
            newForm.GetComponentInChildren<Slider>().value = currentFile.gameProgressPercentage;
            newForm.GetComponent<Button>().onClick.AddListener(() => mainMenuManager.load_Game(currentFile));
            //fill multiple text fields.
        }
    }
}
public class PlayerSaveFileData
{
    public string playerName;
    public string saveDate;
    public string saveTime;
    public Texture saveScreenshot;
    public int gameProgressPercentage;
    public Object lastLocationScene;
}
