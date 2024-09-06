using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerTest : MonoBehaviour
{
    // Start is called before the first frame update

    public static UIManagerTest instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SceneLoader(string nextScene)
    {
        SceneManager.LoadScene(nextScene);

    }

    
}
