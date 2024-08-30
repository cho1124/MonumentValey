using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testcollision : MonoBehaviour
{
    public List<GameObject> collidedList;
    public Transform map;
    public List<Transform> newList;
    public Rigidbody rb;
    // Start is called before the first frame update

    private void Start()
    {
        collidedList = new List<GameObject>();
        rb = GetComponent<Rigidbody>();



    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    map = collision.transform.root;
    //    newList.Clear();
    //    Debug.Log(collision.transform.localPosition.x);
    //
    //    for(int i = 0; i < map.childCount; i++)
    //    {
    //        if(collision.transform.localPosition.z == map.GetChild(i).transform.localPosition.z)
    //        {
    //            newList.Add(map.GetChild(i).transform);
    //        }
    //    }
    //
    //    collidedList.Add(collision.gameObject);
    //}


    private void OnCollisionStay(Collision collision)
    {
        map = collision.transform.root;
        newList.Clear();
        Debug.Log(collision.transform.localPosition.x);

        for (int i = 0; i < map.childCount; i++)
        {
            if (collision.transform.localPosition.z == map.GetChild(i).transform.localPosition.z)
            {
                newList.Add(map.GetChild(i).transform);
            }
        }

        collidedList.Add(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        

        if (collidedList.Contains(collision.gameObject))
        {
            collidedList.Remove(collision.gameObject);
        }
    }


    private void Update()
    {
        Debug.Log($"posx : {rb.position.x}, posy : {rb.position.y}, posz : {rb.position.z}");
    }


}
