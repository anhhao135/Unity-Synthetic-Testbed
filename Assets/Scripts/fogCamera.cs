// This script lets you enable/disable fog per camera.
// by enabling or disabling the script in the title of the Inspector
// you can turn fog on or off per camera.

using UnityEngine;
using System.Collections;

public class fogCamera : MonoBehaviour
{
    private bool revertFogState = false;

    void OnPreRender()
    {
        revertFogState = RenderSettings.fog;
        RenderSettings.fog = enabled;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 10f;
        RenderSettings.fogEndDistance = 1000f;
    }

    void OnPostRender()
    {
        RenderSettings.fog = revertFogState;
    }
}