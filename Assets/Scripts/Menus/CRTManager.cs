using UnityEngine;
using System.Collections.Generic;
public class GameSceneCrtHandler : MonoBehaviour
{
    [Tooltip("Assign all CRT scripts (from CameraFPS, CameraOrbital) here in the Inspector.")]
    [SerializeField] private List<CRT> crtFilterScripts;

    void Awake()
    {
        if (crtFilterScripts == null || crtFilterScripts.Count == 0)
        {
            CRT[] foundCrts = FindObjectsOfType<CRT>();
            if (foundCrts.Length > 0)
            {
                crtFilterScripts = new List<CRT>(foundCrts);
                Debug.Log($"Found {crtFilterScripts.Count} CRT scripts in the scene automatically.");
            }
            else
            {
                Debug.LogError("No CRT scripts found in the scene for GameSceneCrtHandler.", this);
                return;
            }
        }
    }

    void Start()
    {
        ApplyCrtSetting();
    }

    // Call this method whenever you need to update the CRT filter state
    public void ApplyCrtSetting()
    {
        if (crtFilterScripts == null || crtFilterScripts.Count == 0)
        {
            Debug.LogWarning("No CRT scripts assigned or found to apply settings to.", this);
            return;
        }

        // Default to ON (1) if no preference is saved
        int crtState = PlayerPrefs.GetInt("CrtOn", 1);
        bool enableCrt = (crtState == 1);

        foreach (CRT crtScript in crtFilterScripts)
        {
            if (crtScript != null)
            {
                crtScript.enabled = enableCrt;
            }
            else
            {
                Debug.LogWarning("One of the assigned CRT scripts is null in the list.", this);
            }
        }
        Debug.Log($"CRT Filter in Game Scene: {(enableCrt ? "On" : "Off")} for {crtFilterScripts.Count} scripts.");
    }
}