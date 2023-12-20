using System.Collections;
using UnityEngine;
using Cinemachine;

public class SpeedEffects : MonoBehaviour
{
    public CinemachineVirtualCamera vCamera;

    public float zoomingFOV = 80, normalFOV = 60;
    public float zoomSpeed = 1;

    bool zooming, shaking;

    Coroutine _zoomingC, _shakingC;

    public void ZoomSpeed()
    {
        if (!zooming)
        {
            zooming = true;

            _zoomingC = StartCoroutine(ChangeFOV(vCamera, zoomingFOV, zoomSpeed));
        }
    }

    IEnumerator ChangeFOV(CinemachineVirtualCamera cam, float endFOV, float duration)
    {
        float startFOV = cam.m_Lens.FieldOfView;
        float time = 0;
        while (time < duration)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, endFOV, time / duration);
            yield return null;
            time += Time.deltaTime;
        }

        zooming = false;
    }
}
