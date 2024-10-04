using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;

[Serializable]
public class UISettings
{
    public List<Image> images;
    public List<string> names;

}

public class UIManager : MonoBehaviour
{
    [Header("Level Select UI")]
    [SerializeField] private UISettings levelUI;

    public static UIManager instance = null;

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

    // Start is called before the first frame update
    private void Start()
    {
        
        for(int i = 0; i < levelUI.images.Count; i++)
        {
            levelUI.images[i].sprite = SpriteLoader.instance.GetSprite(levelUI.names[i]);
        }

    }

    public void GameExit()
    {

        Debug.Log(SceneManager.GetActiveScene().name);

        if(SceneManager.GetActiveScene().buildIndex != 0)
        {

            
            Debug.Log("asd");
            SceneManager.LoadScene(0);
        }
        else
        {
#if UNITY_EDITOR
            // �����Ϳ����� �÷��� ��带 ����
            EditorApplication.isPlaying = false;
#else
        // ����� ���ӿ����� ������ ����
            Application.Quit();
#endif
        }



    }

    // Update is called once per frame

}
