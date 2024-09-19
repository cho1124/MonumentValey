using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;
using System;

[Serializable]
public class Boundary : MonoBehaviour
{
    public Node parentNode;
    public Boundary linkedBoundary;
    public bool isTeleport = false;
    public bool isCirculation = false;


}
