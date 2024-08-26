using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collidertets : MonoBehaviour
{
    [SerializeField] private CapsuleCollider col;

    private void Start()
    {
        col = GetComponentInChildren<CapsuleCollider>();
    }

    

}
