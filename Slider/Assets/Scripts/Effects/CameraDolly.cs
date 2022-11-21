using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraDolly : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineTrackedDolly dolly;
    public CinemachineSmoothPath path;
    private int numWaypoints;
    
    public AnimationCurve pathMovementCurve;
    public float duration;

    private void Awake() 
    {
        dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        numWaypoints = path.m_Waypoints.Length;
    }

    public void StartTrack()
    {
        virtualCamera.Priority = 15;
        StartCoroutine(Rollercoaster());
    }
    
    private IEnumerator Rollercoaster()
    {
        UIEffects.FadeFromBlack();
        UIManager.canOpenMenus = false;
        Player.SetCanMove(false);

        float t = 0;
        
        // fade out at end
        while (t < duration - 0.25f)
        {
            float x = (t / duration);

            dolly.m_PathPosition = pathMovementCurve.Evaluate(x) * (numWaypoints - 1);

            yield return null;
            t += Time.deltaTime;
        }

        UIEffects.FadeToBlack(
            () => EndTrack()
        );

        while (t < duration)
        {
            float x = (t / duration);

            dolly.m_PathPosition = pathMovementCurve.Evaluate(x) * (numWaypoints - 1);

            yield return null;
            t += Time.deltaTime;
        }

        // EndTrack();
    }

    private void EndTrack()
    {
        UIEffects.FadeFromBlack();
        UIManager.canOpenMenus = true;
        Player.SetCanMove(true);
        virtualCamera.Priority = -15;
    }
}
