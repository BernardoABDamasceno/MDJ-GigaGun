using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class GigaGun : MonoBehaviour
{
    private List<ConnectionPoint> freeConnectionPoints = new List<ConnectionPoint>();
    private List<GameObject> guns = new List<GameObject>();
    
    private GameObject insertingGun = null;
    private GameObject insertingCP = null;
    private bool insertingCPActive = true;
    private List<ConnectionPoint> insertingGunCP = new List<ConnectionPoint>();

    [SerializeField] GameObject initialGun;

    void Start()
    {
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
        }
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
        if(insertingGun != null)
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
            insertingCP.GetComponentInParent<Gun>().transform
        );

        foreach (ConnectionPoint point in insertingGun.GetComponentsInChildren<ConnectionPoint>())
        {
            bool found = false;
            foreach (ConnectionPoint point2 in freeConnectionPoints)
            {
                if(point.transform.position == point2.transform.position)
                {
                    found = true;
                    break;
                }
            }
            if (!found) {
                foreach (GameObject gun in guns)
                {
                    if(point.transform.position == gun.transform.position)
                    {
                        found = true;
                        break;
                    }
                }
            }
            point.SetInteractable(false);
            
            if(found) point.gameObject.SetActive(false);
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

        Camera.main.SendMessage("resetAssemblyMode");
    }
}
