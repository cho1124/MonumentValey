using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using RW.MonumentValley;


public class BoxSpinner : DragSpinner
{
    // Start is called before the first frame update
    [SerializeField] private GameObject activateObj;
    private BoxContainer boxContainer;

    private void Start()
    {
        boxContainer = GetComponentInParent<BoxContainer>();
       
        settings.Initialize();

        if(activateObj != null)
        {
            activateObj.SetActive(false);   

        }
        //settings.target = boxManager.canActiveObj[?];

        //settings.Initialize();
        //EnableSpinner(true);
    }

    public override void OnBeginDrag(PointerEventData data)
    {
        if (Vector3.Distance(settings.target.transform.position, settings.MinTransform.position) < 0.01f)
        {
            //�̺�Ʈ �߻�!!!!!!!!!!!!!!!!!!!!
            //�ش��ϴ� ���ǳʸ� ������ ������ ���ǳʵ� isactive ��������
            boxContainer.SelectedSpinnerActive(this, false);
            if (activateObj != null)
            {
                activateObj.SetActive(true);
            }
            
        }

        base.OnBeginDrag(data);

    }

    public override void OnDrag(PointerEventData data)
    {
        base.OnDrag(data);
    }

    public override void OnEndDrag(PointerEventData data)
    {
        if (Vector3.Distance(settings.target.transform.position, settings.MinTransform.position) < 0.01f)
        {
            boxContainer.SelectedSpinnerActive(this, true);
            activateObj.SetActive(false);
        }

        base.OnEndDrag(data);
    }

}
