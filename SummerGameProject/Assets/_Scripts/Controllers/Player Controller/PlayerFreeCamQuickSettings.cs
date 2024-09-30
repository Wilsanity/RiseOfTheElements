using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeCamQuickSettings : MonoBehaviour
{
    // editor only script for quick modifications of the camera values, will scale this up to player accessible settings in the future
    [Header("Plug in Fields")]
    [SerializeField] CinemachineFreeLook _camera;
    [Header("Default Camera Settings")]
    [SerializeField] private float _freeLookCamDefaultXSensitibity = 1f;
    [SerializeField] private float _freeLookCamDefaultYSensitibity = 0.1f;
    [Header("'Player' Settings")]
    [SerializeField, Range(0.1f,3)] private float _sensibilityModifier = 1;
    [SerializeField] private bool _invertXAxis = false;
    [SerializeField] private bool _invertYAxis = true;
    private void Start()
    {
        SetCameraValues();
    }
    private void OnValidate()
    {
        SetCameraValues();
    }
    private void SetCameraValues()
    {
        _camera.m_XAxis.m_MaxSpeed = _freeLookCamDefaultXSensitibity * _sensibilityModifier;
        _camera.m_YAxis.m_MaxSpeed = _freeLookCamDefaultYSensitibity * _sensibilityModifier;
        
        _camera.m_XAxis.m_InvertInput = _invertXAxis;
        _camera.m_YAxis.m_InvertInput = _invertYAxis;
    }
}
