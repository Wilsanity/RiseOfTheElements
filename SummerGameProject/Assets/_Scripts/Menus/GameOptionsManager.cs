using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameOptionsManager : MonoBehaviour
{
    //variables needed for rebinding actions
    #region
    [SerializeField]
    public PlayerInput gameActions = null;

    enum action
    {
        Sprint,
        Jump,
        Dodge,
        Interact,
        Pause,
    }

    //Texts & action refrences for rebinding
    #region
    [SerializeField] Button[] rebindButtons;
    [SerializeField] InputActionReference[] actionReferences;
    #endregion

    InputActionRebindingExtensions.RebindingOperation keyRebind;
    #endregion

    //shell functions for rebinding. These are called OnClick by the respective buttons and then call the changeBinging function with the specific information
    //One is required for each key action.
    #region
    public void sprintShell()
    {
        changeBinding(actionReferences[(int)action.Sprint], rebindButtons[(int)action.Sprint].GetComponentInChildren<Text>());
    }
    public void jumpShell()
    {
        changeBinding(actionReferences[(int)action.Jump], rebindButtons[(int)action.Jump].GetComponentInChildren<Text>());
    }
    public void dodgeShell()
    {
        changeBinding(actionReferences[(int)action.Dodge], rebindButtons[(int)action.Dodge].GetComponentInChildren<Text>());
    }
    public void interactShell()
    {
        changeBinding(actionReferences[(int)action.Interact], rebindButtons[(int)action.Interact].GetComponentInChildren<Text>());
    }
    public void pauseShell()
    {
        changeBinding(actionReferences[(int)action.Pause], rebindButtons[(int)action.Pause].GetComponentInChildren<Text>());
    }
    #endregion
    private void changeBinding(InputActionReference actionToBeChanged, Text keyText)
    {
        gameActions.SwitchCurrentActionMap("temp");
        disableButtons();

        keyText.text = "";
        keyRebind = actionToBeChanged.action.PerformInteractiveRebinding().
            WithControlsExcluding("Mouse").
            OnMatchWaitForAnother(0.1f).
            OnComplete(operation => finishChange(actionToBeChanged, keyText)).
            Start();
    }
    private void finishChange(InputActionReference newKey, Text updatedKeyText)
    {
        int bindingIndex = newKey.action.GetBindingIndexForControl(newKey.action.controls[0]);

        keyRebind.Dispose();
        updatedKeyText.text = InputControlPath.
            ToHumanReadableString(newKey.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        gameActions.SwitchCurrentActionMap("Player");
        enableButtons();
    }
    private void disableButtons()
    {
        foreach (Button button in rebindButtons)
        {
            button.interactable = false;
        }
    }
    private void enableButtons()
    {
        foreach (Button button in rebindButtons)
        {
            button.interactable = true;
        }
    }
}