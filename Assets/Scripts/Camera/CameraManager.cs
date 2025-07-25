using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static bool isAssemblyMode = false;

    [SerializeField] Camera fpsCam;
    [SerializeField] Camera orbitalCam;
    [SerializeField] Canvas optionsCanvas;
    [SerializeField] Canvas dmgCanvas;
    [SerializeField] Canvas infoDump;
    [SerializeField] Canvas infoDumpFilter;
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] Canvas background;
    [SerializeField] Canvas playerUI;
    [SerializeField] GameObject gigaGun;
    [SerializeField] GameObject player;

    [SerializeField] private Vector3 assemblyLocation = new Vector3(-500, -500, -500);
    [SerializeField] private Transform gigagunFPStransform;

    private bool weaponchoice = false;
    private int turnedOn = 0; // 0 = Revolver, 1 = PlasmaGun, 2 = Shotgun, 3 = RPG, 4 = Flamethrower

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isAssemblyMode = false;
        weaponchoice = false;
        gigaGun.transform.parent = fpsCam.transform;
        fpsCam.gameObject.SetActive(true);
        orbitalCam.gameObject.SetActive(false);
        optionsCanvas.gameObject.SetActive(false);
        infoDump.gameObject.SetActive(false);
        filterShutdown();
        background.gameObject.SetActive(false);
        dmgCanvas.gameObject.SetActive(false);
        playerUI.gameObject.SetActive(true);
        pauseCanvas.worldCamera = fpsCam;

        orbitalCam.transform.position = assemblyLocation;
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
                SetGun(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetGun(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetGun(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetGun(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetGun(4);
            }
        }

        // Logging player actions for data collection
        // if (!PauseManager.isGamePaused)
        // {
        //     if (isAssemblyMode) JsonFileWriter.sampleData.timeSpentAssembly += Time.deltaTime;
        //     else JsonFileWriter.sampleData.timeSpentFPS += Time.deltaTime;
        // }
    }
    void FixedUpdate()
    {
        // keeps up with player position
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

        gigaGun.transform.position = gigagunFPStransform.position;
        gigaGun.transform.rotation = gigagunFPStransform.rotation;

        fpsCam.gameObject.SetActive(true);
        orbitalCam.gameObject.SetActive(false);
        optionsCanvas.gameObject.SetActive(false);
        infoDump.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        playerUI.gameObject.SetActive(true);
        pauseCanvas.worldCamera = fpsCam;

        // Logging player actions for data collection
        // FindObjectOfType<JsonFileWriter>().WriteToJson();
    }

    private void changeToOrbital()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        player.SendMessage("paused");
        isAssemblyMode = true;
        weaponchoice = true;
        infoDump.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        //default pick
        SetGun(0);


        gigagunFPStransform.position = gigaGun.transform.position;
        gigagunFPStransform.rotation = gigaGun.transform.rotation;

        gigaGun.transform.parent = GameObject.FindGameObjectWithTag("PlayerGroup").transform;

        //dumbest shit ever
        orbitalCam.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        orbitalCam.GetComponent<OrbitalCamera>().rotation = new Vector2(0f, 0f);

        gigaGun.transform.position = assemblyLocation;
        gigaGun.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

        gigaGun.gameObject.SendMessage("enableConnectionPoints");
        fpsCam.gameObject.SetActive(false);
        orbitalCam.gameObject.SetActive(true);
        pauseCanvas.worldCamera = orbitalCam;

        // Logging player actions for data collection
        // FindObjectOfType<JsonFileWriter>().WriteToJson();
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
    public void SetGun(int num)
    {
        infoDumpFilter.transform.GetChild(turnedOn).gameObject.SetActive(false);
        infoDumpFilter.transform.GetChild(num).gameObject.SetActive(true);
        turnedOn = num;
        GameObject newgun = null;
        if (num == 0)
        {
            newgun = Resources.Load<GameObject>("Prefabs/Guns/Revolver");
        }
        else if (num == 1)
        {
            newgun = Resources.Load<GameObject>("Prefabs/Guns/PlasmaGun");
        }
        else if (num == 2)
        {
            newgun = Resources.Load<GameObject>("Prefabs/Guns/Shotgun");
        }
        else if (num == 3)
        {
            newgun = Resources.Load<GameObject>("Prefabs/Guns/RPG");
        }
        else if (num == 4)
        {
            newgun = Resources.Load<GameObject>("Prefabs/Guns/Flamethrower");
        }
        gigaGun.SendMessage("setPickedGun", newgun);
    }

    public void levelUp()
    {

        isAssemblyMode = !isAssemblyMode;
        if (!isAssemblyMode) changeToFPS();
        else
        {
            StartCoroutine(captureFrameTimely());
        }
    }

    public void flashRed()
    {
        dmgCanvas.gameObject.SetActive(true);
        Invoke("flashReset", 0.1f);
    }
    private void flashReset()
    {
        dmgCanvas.gameObject.SetActive(false);
    }
    private IEnumerator captureFrameTimely()
    {
        playerUI.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        background.GetComponentInChildren<RawImage>().texture = texture;
        background.GetComponentInChildren<RawImage>().SetNativeSize();
        changeToOrbital();
    }

    private void filterShutdown()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject child = infoDumpFilter.transform.GetChild(i).gameObject;
            child.SetActive(false);
        }
    }
}


// ADD SMALL GUN ANIMATIONS
// 