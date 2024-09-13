using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

    
    // Start is called before the first frame update
    private void Start()
    {
        
        for(int i = 0; i < levelUI.images.Count; i++)
        {
            levelUI.images[i].sprite = SpriteLoader.instance.GetSprite(levelUI.names[i]);
        }

    }

    // Update is called once per frame

}
