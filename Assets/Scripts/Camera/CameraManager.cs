using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static bool isAssemblyMode = false;

    [SerializeField] Camera fpsCam;
    [SerializeField] Camera orbitalCam;
    [SerializeField] Canvas optionsCanvas;
    [SerializeField] GameObject gigaGun;
    [SerializeField] GameObject player;

    private bool fpsMode = true;
    private bool weaponchoice = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        fpsCam.gameObject.SetActive(fpsMode);
        orbitalCam.gameObject.SetActive(!fpsMode);
        optionsCanvas.gameObject.SetActive(weaponchoice);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            levelUp();
        }
        if (weaponchoice)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                print("1");
                SetGun("Prefabs/Guns/Gun");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                print("2");
                SetGun("Prefabs/Guns/SMG");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                print("3");
                SetGun("Prefabs/Guns/Shotgun");
            }
        }
    }
    void FixedUpdate()
    {
        if (!isAssemblyMode)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1.0f, player.transform.position.z);
        }
    }

    private void changeToFPS()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isAssemblyMode = false;
        weaponchoice = false;
        player.SendMessage("unpaused");
        if (GigaGun.insertingGun != null)
            gigaGun.SendMessage("cancelInsertGun");
        gigaGun.gameObject.SendMessage("disableConnectionPoints");
        gigaGun.transform.parent = fpsCam.transform;
        fpsCam.gameObject.SetActive(true);
        orbitalCam.gameObject.SetActive(false);
        optionsCanvas.gameObject.SetActive(false);
    }

    private void changeToOrbital()
    {
        gigaGun.transform.parent = transform;
        gigaGun.gameObject.SendMessage("enableConnectionPoints");
        fpsCam.gameObject.SetActive(false);
        orbitalCam.gameObject.SetActive(true);
    }

    private void weaponPick()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        player.SendMessage("paused");
        isAssemblyMode = true;
        optionsCanvas.gameObject.SetActive(true);
        changeToOrbital();
        weaponchoice = true;
    }
    private void SetGun(string gunPrefabPath)
    {
        GameObject newgun = Resources.Load<GameObject>(gunPrefabPath);
        print("Setting gun: " + newgun.layer);
        gigaGun.transform.parent = transform;
        gigaGun.SendMessage("setPickedGun", newgun);
        optionsCanvas.gameObject.SetActive(false);
        weaponchoice = false;
    }
    
    public void levelUp()
    {
        fpsMode = !fpsMode;
        if (fpsMode) changeToFPS();
        else weaponPick();
    }
}
