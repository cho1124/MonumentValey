using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using RW.MonumentValley;
using DG.Tweening;

[Serializable]
public class Stage
{
    public int stageNum;
    public string stageName;
    public GameObject stageObj;
    public bool isOpened;

}

[Serializable]
public class LevelRotator : SpinnerSettings
{
    public Vector3 newAngles = Vector3.zero;

    protected override void RotateTarget(Vector2 mousePosition)
    {

        Vector3 directionToMouse = mousePosition - (Vector2)mainCamera.WorldToScreenPoint(pivot.position);
        //Debug.Log(directionToMouse);
        float angleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        if (directionToMouse.magnitude > minDragDist)
        {

            Vector3 axisDirection = GetAxisDirection();
            //1. 방향 구하기
            //2. 방향만큼 흠 구하기


            Vector3 newRotationVector = (previousAngleToMouse - angleToMouse) * axisDirection * rotationSpeed; //y축 고정

            newRotationVector = -newRotationVector;
            newRotationVector = newRotationVector * smoothDamp;


            Rigidbody rb = fakeTarget.GetComponent<Rigidbody>();

            rb.AddTorque(newRotationVector, ForceMode.Force);

        }

    }

    public override void Drag(Vector2 mousePosition)
    {
        if (isActive && isSpinning)
        {
            //Debug.Log("Mouse Pos : " + mousePosition);
            RotateTarget(mousePosition);
        }
    }

    public override void Snap()
    {
        isSpinning = false;
        isSpinningDo = false;

    }
    public void DOSnap()
    {
        if (isSpinningDo) return;

        if (transformationMode == TransformationMode.Rotation)
        {
            Vector3 eulerAngles = fakeTarget.eulerAngles;
            
            float roundedXAngle = Mathf.Round(eulerAngles.x / 90f) * 90f;
            float roundedYAngle = Mathf.Round(eulerAngles.y / 90f) * 90f;
            float roundedZAngle = Mathf.Round(eulerAngles.z / 90f) * 90f;
            Rigidbody rb = fakeTarget.GetComponent<Rigidbody>();

            switch (spinAxis)
            {
                case SpinAxis.X:
                    newAngles = new Vector3(roundedXAngle, eulerAngles.y, eulerAngles.z);
                    break;
                case SpinAxis.Y:
                    newAngles = new Vector3(eulerAngles.x, roundedYAngle, eulerAngles.z);
                    break;
                case SpinAxis.Z:
                    newAngles = new Vector3(eulerAngles.x, eulerAngles.y, roundedZAngle);
                    break;
            }

            if (rb.angularVelocity.magnitude < 1f)
            {
                fakeTarget.DORotate(newAngles, 0.5f);
            }
        }
    }
}
public class LevelSelectionContainer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //TODO: level class를 담?는 자료구조 만들기 >> 클래스 만들고 연결 부드럽게 하면 될듯, 그리고 정면에서 레이를 쏘는 것 혹은 1스테이지부터 각도 계속 중첩계산해서 해당하는 각도에 맞게 나오든가 혹은 레이를 쏘든가 알아서 하면 되긴 할듯

    [SerializeField] private LevelRotator settings;
    [SerializeField] private List<GameObject> stageObj;
    [SerializeField] private float totalRotation; // 누적된 회전값
    [SerializeField] private Transform stageTr;
    public List<Stage> stages;
    private int count = 0;
    private Quaternion previousRotation; // 이전 프레임의 회전값
    private int totalRevolutions; // 총 회전한 바퀴 수


    private void Awake()
    {
        foreach (Stage stage in stages)
        {
            GameObject gameObject = Instantiate(stage.stageObj, stageTr);
            stageObj.Add(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        previousRotation = transform.rotation;
        totalRotation = 0f;
        totalRevolutions = 0;
        settings.Initialize();
        EnableSpinner(true);
    }

    private void FixedUpdate()
    {
        if (settings.fakeTarget != null)
        {
            settings.target.rotation = settings.fakeTarget.rotation;
        }
        settings.DOSnap();

        Checker();
        TestQ();

    }

    //private void OnMouseDown()
    //{
    //
    //    if (settings.isActive)
    //    {
    //        settings.BeginDrag(Input.mousePosition);
    //    }
    //}
    //
    //private void OnMouseDrag()
    //{
    //    settings.Drag(Input.mousePosition);
    //    //SetQ();
    //}
    //
    //private void OnMouseUp()
    //{
    //    if (settings.isActive)
    //    {
    //        settings.Snap();
    //    }
    //
    //}

    public void OnBeginDrag(PointerEventData data)
    {
        //Debug.Log("Drag Beginning");
        if (settings.isActive)
        {
            settings.BeginDrag(data.position);
        }

    }

    public void OnDrag(PointerEventData data)
    {
        settings.Drag(data.position);
    }

    public void OnEndDrag(PointerEventData data)
    {
        //Debug.Log("EndDrag");
        if (settings.isActive)
        {
            settings.Snap();
        }
    }

    private void TestQ()
    {
        Quaternion currentRotation = settings.target.rotation;

        // 이전 회전값과 현재 회전값의 차이를 구함 (Quaternion 간의 각도 차이)
        float angleDelta = Mathf.Sign(currentRotation.eulerAngles.magnitude - previousRotation.eulerAngles.magnitude) * Quaternion.Angle(previousRotation, currentRotation);

        // 누적 회전 각도에 추가
        totalRotation += angleDelta;

        previousRotation = currentRotation;
        
    }

    private void Checker()
    {
        int stageNum = Mathf.RoundToInt(totalRotation / 90f);
        
        if(stageNum == 0)
        {
            stageObj[stageNum].SetActive(true);
            stageObj[stageNum + 1].SetActive(true);
            return;
        }

        ActiveStageObj(stageNum);
    }


    private void ActiveStageObj(int num)
    {
        for(int i = 0; i < stageObj.Count; i++)
        {
            if(i == num || i == num - 1 || i == num + 1)
            {
                stageObj[i].SetActive(true);
            }
            else
            {
                stageObj[i].SetActive(false);
            }
        }

    }

    public void EnableSpinner(bool state)
    {
        settings.isActive = state;

        if (!settings.isActive)
        {
            settings.Snap();
        }
    }



}

