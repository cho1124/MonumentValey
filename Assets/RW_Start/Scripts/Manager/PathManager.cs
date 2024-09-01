using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;


public class PathManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static PathManager instance = null;


    public Clickable[] clickables;
    public Node[] newNode;

    private void Awake()
    {

        if(instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }

        clickables = FindObjectsOfType<Clickable>();
        newNode = FindObjectsOfType<Node>();
        


    }
    private void Start()
    {
        //newNode = new Node[clickables.Length];

        //Debug.Log(clickables.Length);



    }


}
