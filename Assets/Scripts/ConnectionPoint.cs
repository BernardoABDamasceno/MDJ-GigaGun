using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint
{
    private static int idCounter = 0;
    //this is unique
    private int id;
    private Gun parent;
    private Transform position;

    public ConnectionPoint(Gun parent, Transform position)
    {
        this.parent = parent;
        this.position = position;
        id = idCounter;
        idCounter++;
    }
    public int getId()
    {
        return id;
    }
    public Transform getPosition()
    {
        return position;
    }
    public Gun getParent()
    {
        return parent;
    }
}
