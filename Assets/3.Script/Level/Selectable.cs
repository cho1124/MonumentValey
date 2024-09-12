using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;



public class Selectable : MonoBehaviour
{

    // Start is called before the first frame update
    public Stage stage;

    public Animation ani;

    public UnityEvent<StageState> stageState;
    


    private void Start()
    {
        ani = GetComponentInChildren<Animation>();

        stageState.AddListener(PlayAni);
        
    }


    private void PlayAni(StageState state)
    {
    }
   
    private void OnMouseDown()
    {
        string layerName = LayerMask.LayerToName(gameObject.layer);

        if(layerName == "Stage")
        {
            

        }

    }


}
