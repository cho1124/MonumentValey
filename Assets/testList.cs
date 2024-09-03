using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class testList : MonoBehaviour
{
    public List<Transform> TooManyObj;

    private void Awake()
    {
        TooManyObj = new List<Transform>();

        Transform[] test = gameObject.GetComponentsInChildren<Transform>();

        for(int i = 0; i < transform.childCount; i++)
        {
            TooManyObj.Add(test[i]);
        }



    }

    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        
        foreach(Transform tr in TooManyObj)
        {
            tr.Translate(tr.forward * Time.deltaTime);
        }


    }
}
