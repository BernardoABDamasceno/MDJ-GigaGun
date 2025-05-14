using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun
{
    [SerializeField] private GameObject gunModel;
    //TODO: add all relevant stats

    public Gun(GameObject gunModel, Transform position)
    {
        this.gunModel = gunModel;
        GameObject.Instantiate(gunModel, position.position, position.rotation);
    }
}
