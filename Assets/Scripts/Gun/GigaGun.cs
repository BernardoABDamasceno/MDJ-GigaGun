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
    [SerializeField] GameObject recoilManager;
    public static GameObject insertingGun = null;
    private GameObject insertingCP = null;
    private bool insertingCPActive = true;
    private List<ConnectionPoint> insertingGunCP = new List<ConnectionPoint>();

    [Header("Initial Gun Settings")]
    [SerializeField] GameObject initialGun;

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
            if (Input.GetKeyDown(KeyCode.Space))
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
            if (Input.GetButton("Fire1") && !CameraManager.isAssemblyMode) // Default Fire key is Left Mouse Button
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
    }

    public void disableConnectionPoints()
    {
        foreach (ConnectionPoint item in freeConnectionPoints) item.gameObject.SetActive(false);
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
            initialGun,
            insertingCP.transform.position,
            insertingCP.transform.rotation,
            transform
        );

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
            if (point.gameObject.transform.position == insertingGun.gameObject.transform.position)
            {
                Destroy(point);
            }
            else
            {
                point.SetInteractable(true);
                freeConnectionPoints.Add(point);
            }
        }
        insertingGunCP.Clear();

        guns.Add(insertingGun);
        insertingGun = null;

        freeConnectionPoints.Remove(insertingCP.GetComponent<ConnectionPoint>());
        Destroy(insertingCP);
        insertingCP = null;

        orbitalCam.SendMessage("resetAssemblyMode");
    }
    
    private void finishCooldown()
    {
        cooldownRot = false;
    }
}
