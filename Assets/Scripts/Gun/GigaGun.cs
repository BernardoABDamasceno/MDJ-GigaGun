using System.Collections.Generic;
using UnityEngine;

public class GigaGun : MonoBehaviour
{
    private List<ConnectionPoint> freeConnectionPoints = new List<ConnectionPoint>();
    private List<GameObject> guns = new List<GameObject>();

    [Header("Required Components")]
    [SerializeField] Camera orbitalCam;
    [SerializeField] Camera fpsCam;
    [SerializeField] GameObject player;
    public static GameObject insertingGun = null;
    private GameObject insertingCP = null;
    private bool insertingCPActive = true;
    private List<ConnectionPoint> insertingGunCP = new List<ConnectionPoint>();

    [Header("Initial Gun Settings")]
    [SerializeField] GameObject initialGun;
    private GameObject pickedGun;

    [Header("Gun Rotation Settings")]
    [SerializeField] float insertingGunRotSpeed = 15.0f;
    [SerializeField] float cooldownRotTime = 0.25f;
    private bool cooldownRot = false;

    void Start()
    {
        //this gets fps camera due to the prefab setup, which is wrong
        // orbitalCam = GetComponentInParent<Camera>().GetComponentInParent<Camera>();

        guns.Add(Instantiate(initialGun, transform));
        //adicionar os pontos a lista de pontos livres
        foreach (ConnectionPoint point in guns[0].GetComponentsInChildren<ConnectionPoint>())
        {
            freeConnectionPoints.Add(point);
            point.SetInteractable(true);
            point.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (insertingGun != null)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                insertingCPActive = !insertingCPActive;
                foreach (ConnectionPoint point in insertingGunCP)
                {
                    point.gameObject.SetActive(!point.gameObject.activeSelf);
                }
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                confirmInsertGun();
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (!cooldownRot)
                {
                    if (insertingCP.transform.localPosition.y != 0)
                    {
                        insertingGun.transform.Rotate(Vector3.up, -insertingGunRotSpeed);
                        cooldownRot = true;
                        Invoke("finishCooldown", cooldownRotTime);
                    }
                    else if (insertingCP.transform.localPosition.x != 0)
                    {
                        insertingGun.transform.Rotate(Vector3.right, -insertingGunRotSpeed);
                        cooldownRot = true;
                        Invoke("finishCooldown", cooldownRotTime);
                    }
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (!cooldownRot)
                {
                    if (insertingCP.transform.localPosition.y != 0)
                    {
                        insertingGun.transform.Rotate(Vector3.up, insertingGunRotSpeed);
                        cooldownRot = true;
                        Invoke("finishCooldown", cooldownRotTime);
                    }
                    else if (insertingCP.transform.localPosition.x != 0)
                    {
                        insertingGun.transform.Rotate(Vector3.right, insertingGunRotSpeed);
                        cooldownRot = true;
                        Invoke("finishCooldown", cooldownRotTime);
                    }
                }
            }
        }
        else
        {
            if (Input.GetButton("Fire1") && !CameraManager.isAssemblyMode && !PauseManager.isGamePaused) // Default Fire key is Left Mouse Button
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        foreach (GameObject gun in guns) gun.SendMessage("shoot");
    }

    public void enableConnectionPoints()
    {
        foreach (ConnectionPoint item in freeConnectionPoints) item.gameObject.SetActive(true);
        foreach (ConnectionPoint item in freeConnectionPoints) item.confirmCollisions();
    }

    public void disableConnectionPoints()
    {
        foreach (ConnectionPoint item in freeConnectionPoints) item.gameObject.SetActive(false);
    }

    public void setPickedGun(GameObject newgun)
    {
        pickedGun = newgun;
    }

    public void insertGun(ConnectionPoint connectionPoint)
    {
        if (insertingGun != null)
        {
            cancelInsertGun();
        }
        // instaciar a nova arma
        //referencia da nova arma
        // referencia do ponto
        // referencia da arma a qual o ponto pertence para fazer a arma nova ser filha
        //adiciona-la a lista de armas

        insertingCP = connectionPoint.gameObject;
        insertingGun = Instantiate(
            pickedGun,
            insertingCP.transform.position,
            insertingCP.transform.rotation,
            transform
        );


        // who programmed this? me? why?
        foreach (ConnectionPoint point in insertingGun.GetComponentsInChildren<ConnectionPoint>())
        {
            bool found = false;
            foreach (ConnectionPoint point2 in freeConnectionPoints)
            {
                if (point.transform.position == point2.transform.position)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                foreach (GameObject gun in guns)
                {
                    if (point.transform.position == gun.transform.position)
                    {
                        found = true;
                        break;
                    }
                }
            }
            point.SetInteractable(false);

            //this shit makes no sense wtf is this
            if (found) point.gameObject.SetActive(false);
            else point.gameObject.SetActive(insertingCPActive);

            insertingGunCP.Add(point);
        }

        //disable o ponto
        insertingCP.SetActive(false);
    }
    //these names are terrible
    public void cancelInsertGun()
    {
        //destroy a arma que estava sendo instanciada
        Destroy(insertingGun);
        insertingGun = null;
        insertingCP.SetActive(true);
        insertingGunCP.Clear();
    }

    public void confirmInsertGun()
    {
        foreach (ConnectionPoint point in insertingGunCP)
        {
            //THIS WILL NEED TO BE CHANGED, SINCE THE STATEMENT IS NOT GARANTEED LATER
            /* if (point.gameObject.transform.position == insertingGun.gameObject.transform.position)
            {
                Destroy(point);
            }
            else
            {
                point.SetInteractable(true);
                freeConnectionPoints.Add(point);
            } */

            if (point != null)
            {
                point.SetInteractable(true);
                freeConnectionPoints.Add(point);
            }
        }
        insertingGunCP.Clear();

        guns.Add(insertingGun);
        // Logging player actions for data collection
        switch (insertingGun.name.Split('(')[0]) // Get the gun type from the name
        // This assumes the gun name is formatted like "Revolver(Clone)" or "Shotgun(Clone)"
        {
            case "Revolver":
                JsonFileWriter.sampleData.revolvers++;
                break;
            case "Shotgun":
                JsonFileWriter.sampleData.shotguns++;
                break;
            case "PlasmaGun":
                JsonFileWriter.sampleData.plasmaRifles++;
                break;
            case "Flamethrower":
                JsonFileWriter.sampleData.flamethrowers++;
                break;
            case "RPG":
                JsonFileWriter.sampleData.rpgs++;
                break;
            default:
                Debug.LogWarning("Unknown gun type: " + insertingGun.name);
                break;
        }
        FindObjectOfType<JsonFileWriter>().WriteToJson();
        // End of logging
        insertingGun = null;

        freeConnectionPoints.Remove(insertingCP.GetComponent<ConnectionPoint>());
        Destroy(insertingCP);
        insertingCP = null;

        //check overlaps to stop too extreme shenanigans
        foreach (ConnectionPoint point in freeConnectionPoints)
        {
            point.confirmCollisions();
        }

        // NEEDS TO BE REVIEWD LATER THIS SHOULD NOT BE A CONCERN OF LEVEL UP  METHOD;
        //orbitalCam.SendMessage("resetAssemblyMode");
        //GetComponentInParent<CameraManager>().SendMessage("levelUp");
    }
    
    private void finishCooldown()
    {
        cooldownRot = false;
    }
}
