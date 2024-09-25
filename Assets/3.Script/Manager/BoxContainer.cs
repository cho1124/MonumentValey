using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoxContainer : MonoBehaviour
{
    [SerializeField] private BoxSpinner[] canActiveObj;

    private void Start()
    {
        canActiveObj = GetComponentsInChildren<BoxSpinner>();
    }

    public void SelectedSpinnerActive(BoxSpinner boxSpinner, bool isActive)
    {

        for(int i = 0; i < canActiveObj.Length; i++)
        {
            if(canActiveObj[i] != boxSpinner)
            {
                canActiveObj[i].settings.isActive = isActive;
            }
            


        }
    }


}
