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
        // 오브젝트의 초기 위치 저장
        initialObjectPos = transform.position;
    }

    private void OnMouseDown()
    {
        // 마우스 클릭 시 오브젝트의 월드 좌표 구하기
        Vector3 StartDragPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
        StartDragWorldPos = Camera.main.ScreenToWorldPoint(StartDragPos);
    }

    private void OnMouseDrag()
    {
        // 마우스 드래그 중 월드 좌표 구하기
        Vector3 CurrentDragPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
        CurrentDragWorldPos = Camera.main.ScreenToWorldPoint(CurrentDragPos);



        Debug.Log(CurrentDragWorldPos);
        // 마우스 움직임에 따른 차이 계산




        Vector3 newVec = new Vector3(0, 0, CurrentDragWorldPos.z);

        // 카메라의 우측 벡터를 사용하여 X축으로 이동
        //Vector3 cameraRight = Camera.main.transform.forward;
        //Vector3 newPos = initialObjectPos + cameraRight * PosDiff.x;

        // 오브젝트 위치 업데이트
        transform.position = newVec;
    }
}
