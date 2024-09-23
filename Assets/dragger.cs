using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragger : MonoBehaviour
{
    Vector3 StartDragWorldPos;
    Vector3 CurrentDragWorldPos;
    private Vector3 initialObjectPos;

    private void Start()
    {
        // ������Ʈ�� �ʱ� ��ġ ����
        initialObjectPos = transform.position;
    }

    private void OnMouseDown()
    {
        // ���콺 Ŭ�� �� ������Ʈ�� ���� ��ǥ ���ϱ�
        Vector3 StartDragPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
        StartDragWorldPos = Camera.main.ScreenToWorldPoint(StartDragPos);
    }

    private void OnMouseDrag()
    {
        // ���콺 �巡�� �� ���� ��ǥ ���ϱ�
        Vector3 CurrentDragPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
        CurrentDragWorldPos = Camera.main.ScreenToWorldPoint(CurrentDragPos);



        Debug.Log(CurrentDragWorldPos);
        // ���콺 �����ӿ� ���� ���� ���




        Vector3 newVec = new Vector3(0, 0, CurrentDragWorldPos.z);

        // ī�޶��� ���� ���͸� ����Ͽ� X������ �̵�
        //Vector3 cameraRight = Camera.main.transform.forward;
        //Vector3 newPos = initialObjectPos + cameraRight * PosDiff.x;

        // ������Ʈ ��ġ ������Ʈ
        transform.position = newVec;
    }
}
