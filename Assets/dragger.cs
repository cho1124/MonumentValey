using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RW.MonumentValley;

public class dragger : MonoBehaviour
{

   
    public static dragger instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            Debug.Log("asdzxc");
            instance = this;
        }
        else
        {
            Debug.Log("asd");
            Destroy(gameObject);
        }

        

    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            SceneManager.LoadScene(0);
        }
    }

}
