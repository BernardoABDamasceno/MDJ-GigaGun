using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static bool isAssemblyMode = false;

    [SerializeField] Camera fpsCam;
    [SerializeField] Camera orbitalCam;
    [SerializeField] GameObject gigaGun;
    [SerializeField] GameObject player;

    private bool fpsMode = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        fpsCam.gameObject.SetActive(fpsMode);
        orbitalCam.gameObject.SetActive(!fpsMode);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            fpsMode = !fpsMode;

            if (fpsMode) changeToFPS();
            else changeToOrbital();

            fpsCam.gameObject.SetActive(fpsMode);
            orbitalCam.gameObject.SetActive(!fpsMode);
        }
    }

    private void changeToFPS()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isAssemblyMode = false;
        player.SendMessage("unpaused");
        gigaGun.SendMessage("cancelInsertGun");
        gigaGun.SendMessage("disableConnectionPoints");
        gigaGun.transform.parent = fpsCam.transform;
    }

    private void changeToOrbital()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        isAssemblyMode = true;
        player.SendMessage("paused");
        gigaGun.gameObject.SendMessage("enableConnectionPoints");
        gigaGun.transform.parent = transform;
    }
}
