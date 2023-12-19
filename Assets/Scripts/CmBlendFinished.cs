/* CmBlendFinished.cs - HSSC-2
 * 
 * Creation Date: 19/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [19/12/2023] - Initial implementation (C137)
 */
using UnityEngine;
using Cinemachine;
using System;

public class CmBlendFinished : MonoBehaviour
{
    CinemachineVirtualCameraBase vcamBase;

    [Serializable] public delegate void BlendFinished(CinemachineVirtualCameraBase cam);
    public event BlendFinished onBlendFinished;

    void Start()
    {
        vcamBase = GetComponent<CinemachineVirtualCameraBase>();
        ConnectToVcam(true);
        enabled = false;
    }

    void ConnectToVcam(bool connect)
    {
        var vcam = vcamBase as CinemachineVirtualCamera;
        if (vcam != null)
        {
            vcam.m_Transitions.m_OnCameraLive.RemoveListener(OnCameraLive);
            if (connect)
                vcam.m_Transitions.m_OnCameraLive.AddListener(OnCameraLive);
        }
        var freeLook = vcamBase as CinemachineFreeLook;
        if (freeLook != null)
        {
            freeLook.m_Transitions.m_OnCameraLive.RemoveListener(OnCameraLive);
            if (connect)
                freeLook.m_Transitions.m_OnCameraLive.AddListener(OnCameraLive);
        }
    }

    void OnCameraLive(ICinemachineCamera vcamIn, ICinemachineCamera vcamOut)
    {
        enabled = true;
    }

    void Update()
    {
        var brain = CinemachineCore.Instance.FindPotentialTargetBrain(vcamBase);
        if (brain == null)
            enabled = false;
        else if (!brain.IsBlending)
        {
            if (brain.IsLive(vcamBase))
                onBlendFinished?.Invoke(vcamBase);
            enabled = false;
        }
    }
}