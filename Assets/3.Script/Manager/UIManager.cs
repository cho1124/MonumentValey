using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

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
#if UNITY_EDITOR
        // 에디터에서는 플레이 모드를 종료
        EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서는 게임을 종료
        Application.Quit();
#endif
    }

    // Update is called once per frame

}
