using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using RW.MonumentValley;
using DG.Tweening;

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


            fakeTarget.rotation = Quaternion.Euler(newRotationVector);

            //rb.AddTorque(newRotationVector, ForceMode.Acceleration);
            //fakeTarget.rotation = Quaternion.Euler(newRotationVector);
            
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
            //Rigidbody rb = fakeTarget.GetComponent<Rigidbody>();

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

            Quaternion targetRotation = Quaternion.Euler(newAngles);

            fakeTarget.rotation = Quaternion.Lerp(fakeTarget.rotation, targetRotation, Time.deltaTime * 5f);

            //fakeTarget.DORotate(newAngles, 0.5f);
        }
    }
}

public class LevelSelectionRotator : MonoBehaviour
{
    
    public List<Stage> stages;

    [SerializeField] private LevelRotator settings;
    [SerializeField] private List<GameObject> stageObj;
    [SerializeField] private float totalRotation; // 누적된 회전값
    [SerializeField] private Transform stageTr;
    [SerializeField] private Vector3 currentVel;
    [SerializeField] private float inertialDamping = 0.95f;
    [SerializeField] private bool isDebug = false;
    [SerializeField] private bool rotating = false;
    [SerializeField] private float rotateSpeed = 30.0f;

    private Quaternion previousRotation; // 이전 프레임의 회전값
    Vector3 mousePos, offset, rotation;
    
    private float snapThreshold = 3f;  // 관성 회전과 스냅 사이의 임계값
    private float stopThreshold = 0.1f;  // 회전이 완전히 멈출 최소 속도
    
    private void Awake()
    {
        foreach (Stage stage in stages)
        {
            //TODO: 이 부분에서 현재 진행 상태를 불러와서 
            GameObject gameObject = Instantiate(stage.stageObj, stageTr);
            gameObject.GetComponentInChildren<Selectable>().stage = stage;
            stageObj.Add(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        previousRotation = transform.localRotation;
        totalRotation = 0f;
        
        settings.Initialize();
        EnableSpinner(true);
        
    }

    private void FixedUpdate()
    {
        ObjRotate();

        if (settings.fakeTarget != null)
        {
            settings.target.localRotation = settings.fakeTarget.rotation;
        }
        Checker();
        //ClampRot();
    }

    private void OnMouseDown()
    {
       
        int layer = gameObject.layer;
        string layerName = LayerMask.LayerToName(layer);
        
        rotating = true;

        mousePos = Input.mousePosition;

        //if (settings.isActive)
        //{
        //    settings.BeginDrag(Input.mousePosition);
        //}
    }
    
    private void OnMouseDrag()
    {
        //settings.Drag(Input.mousePosition);
        //SetQ();
    }
    
    private void OnMouseUp()
    {
        rotating = false;

        //if (settings.isActive)
        //{
        //    settings.Snap();
        //}

    }

    private void ObjRotate()
    {
        ClampRot();

        if (rotating)
        {
            offset = (Input.mousePosition - mousePos);

            rotation.y = -(offset.x + offset.y) * Time.deltaTime * rotateSpeed;
           
            settings.fakeTarget.Rotate(rotation);

            currentVel = rotation;

            mousePos = Input.mousePosition;
        }
        else
        {
            // 관성으로 회전
            settings.fakeTarget.Rotate(currentVel * Time.deltaTime * 3f);

            // 점진적으로 회전 속도 감소
            currentVel *= settings.smoothDamp;

            // currentVel.magnitude가 특정 값 이하일 때 스냅 처리
            if (currentVel.magnitude < snapThreshold)
            {
                // 너무 작은 속도가 되면 스냅 실행
                if (currentVel.magnitude < stopThreshold)
                {
                    settings.DOSnap();  // 회전 멈추고 스냅 동작
                    currentVel = Vector3.zero;  // 속도 초기화
                }
                else
                {
                    // 스냅 동작 없이 최소 회전 속도를 유지
                    currentVel = currentVel.normalized * stopThreshold; // 최소 속도로 보정
                }
            }
        }
        
    }
    private void ClampRot()
    {
        Quaternion currentRotation = settings.target.localRotation;

        // 이전 회전값과 현재 회전값의 차이를 구함 (Quaternion 간의 각도 차이)
        float angleDelta = Mathf.Sign(currentRotation.eulerAngles.magnitude - previousRotation.eulerAngles.magnitude) * Quaternion.Angle(previousRotation, currentRotation);

        // 누적 회전 각도에 추가
        totalRotation += angleDelta;

        if(totalRotation < 0 || totalRotation > (stageObj.Count - 1) * 90f)
        {
            
            if(totalRotation <= 0)
            {
                totalRotation = 0;
                
            }
            
            if(totalRotation > (stageObj.Count - 1) * 90f)
            {
                totalRotation = (stageObj.Count - 1) * 90f;
            }

            //settings.fakeTarget.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            
           

            settings.DOSnap();  // 회전 멈추고 스냅 동작
            currentVel = Vector3.zero;  // 속도 초기화


        }

        previousRotation = currentRotation;
        
    }

    private void Checker()
    {
        //이 부분 조건을 좀 더 정밀화
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

