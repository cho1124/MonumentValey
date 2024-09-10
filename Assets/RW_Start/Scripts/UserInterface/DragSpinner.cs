using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using DG.Tweening;

namespace RW.MonumentValley
{
    public enum TransformationMode
    {
        Rotation,
        Position,
        Totem
    }

    public enum SpinAxis
    {
        X,
        Y,
        Z
    }
    [Serializable]
    public class AimedObject
    {
        public Transform target;
        public Transform startTr;
        public Transform endTr;

        public void MoveByRatio(float ratio)
        {
            target.transform.position = Vector3.Lerp(startTr.position, endTr.position, ratio);
            target.transform.rotation = Quaternion.Lerp(startTr.rotation, endTr.rotation, ratio);
        }

    }

    [Serializable]
    public class SpinnerSettings
    {
        [Header("이동시킬 타겟")]
        public Transform fakeTarget;
        public Transform target; // 변형시킬 Transform
        [Header("목표 타겟")]
        public AimedObject[] aimedObjects;
        //public Transform aimedTarget;//TODO: 이동 타겟과 목표 타겟에 대한 최대 이동 거리와 최소 이동 거리에 대한 처리
        public SpinAxis spinAxis = SpinAxis.X; // 회전 축 설정
        public float rotationSpeed = 1.0f; // 회전 속도
        public float moveSpeed = 1.0f; // 위치 변경 속도
        public Tween rotatorTweener;
        

        public Transform pivot; // 피벗 포인트
        public int minDragDist = 10; // 최소 드래그 거리

        public TransformationMode transformationMode = TransformationMode.Rotation; // 변형 모드 설정
        public UnityEvent snapEvent; // 스냅 이벤트

        // 상태 관리 변수들
        [HideInInspector] public bool isSpinning = false;
        public bool isSpinningDo = false;
        [HideInInspector] public float previousAngleToMouse;
        [HideInInspector] public bool isActive = true;

        
        [Header("position 이동 상태에서의 최소 최대 거리")]
        [SerializeField] private Vector3 minVector;
        [SerializeField] private Vector3 maxVector;

        [Header("토템이 이동 가능한 노드")]
        [SerializeField] private Node testNode;

        //private CinemachineVirtualCamera CinemachineVirtualCamera;
        private Camera mainCamera;
        private float ratio = 0f;
        [SerializeField] private bool testDebugger = false;
        //[SerializeField] private Quaternion targetRot;

        public void Initialize()
        {
            mainCamera = Camera.main;
            //targetRot = target.transform.rotation;
        }

        public void BeginDrag(Vector2 mousePosition)
        {
            isSpinning = true;
            isSpinningDo = true;
            
            if (transformationMode == TransformationMode.Rotation)
            {
                Vector3 directionToMouse = mousePosition - (Vector2)Camera.main.WorldToScreenPoint(pivot.position);
                previousAngleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            }
        }

        public void Drag(Vector2 mousePosition)
        {
            if (isSpinning && isActive)
            {
                switch(transformationMode)
                {
                    case TransformationMode.Rotation:
                        RotateTarget(mousePosition);
                        break;
                    case TransformationMode.Position:
                        MoveTarget(mousePosition);
                        break;
                    case TransformationMode.Totem:
                        MoveTotem(mousePosition);
                        
                        break;
                    default:
                        Debug.LogError("트랜스폼 모드가 할당 안됨");
                        break;
                }

            }
        }

        private void MoveTotem(Vector2 mousePosition)
        {
            Vector3 directionToMouse = mousePosition - (Vector2)mainCamera.WorldToScreenPoint(pivot.position);
            
            Vector3 newDir = new Vector3(directionToMouse.x,directionToMouse.y,directionToMouse.z);

            target.position += newDir * moveSpeed * Time.deltaTime;
        }

        private void RotateTarget(Vector2 mousePosition)
        {
            Vector3 directionToMouse = mousePosition - (Vector2)mainCamera.WorldToScreenPoint(pivot.position);
            float angleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;



            if(!testDebugger)
            {
                if (directionToMouse.magnitude > minDragDist)
                {
                    Vector3 axisDirection = GetAxisDirection();
                    Vector3 newRotationVector = (previousAngleToMouse - angleToMouse) * axisDirection * rotationSpeed;
                    target.Rotate(newRotationVector);

                    float rotationDelta = Mathf.Abs(previousAngleToMouse - angleToMouse);
                    float rotationRatio = rotationDelta / 360f; // 전체 회전 각도에 대한 비율
                                                                //Debug.Log("rotationRatio : " + rotationRatio);

                    // 다른 오브젝트의 이동/회전 처리
                    if (aimedObjects.Length != 0)
                    {
                        for (int i = 0; i < aimedObjects.Length; i++)
                        {
                            // 회전 비율에 따라 이동시키거나 회전시키는 로직 추가
                            //
                            aimedObjects[i].MoveByRatio(rotationRatio);

                            // 만약 회전시키려면:
                            // aimedObjects[i].RotateByRatio(rotationRatio);
                        }
                    }

                    previousAngleToMouse = angleToMouse;
                }
            }
            else
            {
                if (directionToMouse.magnitude > minDragDist)
                {
                   
                    Vector3 axisDirection = GetAxisDirection();
                    //1. 방향 구하기
                    //2. 방향만큼 흠 구하기
                    
                    
                    Vector3 newRotationVector = (previousAngleToMouse - angleToMouse) * axisDirection * rotationSpeed; //y축 고정
                    
                    
                    newRotationVector = -newRotationVector;

                    
                    Debug.Log("newRotationVector" + newRotationVector);


                    Rigidbody rb = fakeTarget.GetComponent<Rigidbody>();
                    
                    rb.AddTorque(newRotationVector); //y축으로 AddTorque를 쏘는데 왜
                    //target.rotation = fakeTarget.rotation; //이 부분 update로 동기화
                    

                }

            }


        }

        

        private void MoveTarget(Vector2 mousePosition)
        {
            //TODO: Clamp 개선
            //TODO: ratio 비율에 따라 다른 오브젝트도 그에 맞게 이동?

           

            Vector3 directionToMouse = mousePosition - (Vector2)mainCamera.WorldToScreenPoint(pivot.position);
            Vector3 axisDirection = GetAxisDirection();
            Vector3 newDir = new Vector3(axisDirection.x * directionToMouse.x, axisDirection.y * directionToMouse.y, axisDirection.z * directionToMouse.z);

            target.position += newDir * moveSpeed * Time.deltaTime; // 직접적인 위치값 제한 혹은

            target.position = new Vector3(Mathf.Clamp(target.position.x, minVector.x, maxVector.x),
                Mathf.Clamp(target.position.y, minVector.y, maxVector.y),
                Mathf.Clamp(target.position.z, minVector.z, maxVector.z));

            ratio = (target.position - minVector).magnitude / (maxVector - minVector).magnitude;

           
            if(aimedObjects.Length != 0)
            {
                for(int i = 0; i < aimedObjects.Length; i++)
                {
                    aimedObjects[i].MoveByRatio(ratio);
                }
            }


            //Debug.Log("ratio : " + ratio);

        }

        private Vector3 GetAxisDirection()
        {
            switch (spinAxis)
            {
                case SpinAxis.X:
                    return Vector3.right;
                case SpinAxis.Y:
                    return Vector3.up;
                case SpinAxis.Z:
                    return Vector3.forward;
                default:
                    return Vector3.up;
            }
        }

        public void DoSnap()
        {
            
            if (!testDebugger) return;

            if (transformationMode == TransformationMode.Rotation)
            {
                Vector3 eulerAngles = fakeTarget.eulerAngles;
                Vector3 newAngles = Vector3.zero;
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

                //Debug.Log(newAngles);

                if(!isSpinningDo && rb.angularVelocity.magnitude < 1f)
                {
                    fakeTarget.DORotate(newAngles, 0.5f);
                    //rb.angularVelocity = Vector3.zero;
                    
                    return;
                }


                //target.DORotate(newAngles, 0.1f);
   
            }
        }

        public void Snap()
        {
            isSpinning = false;
            isSpinningDo = false;
            if (transformationMode == TransformationMode.Rotation)
            {
                Vector3 eulerAngles = target.eulerAngles;
                Vector3 newAngles = Vector3.zero;
                float roundedXAngle = Mathf.Round(eulerAngles.x / 90f) * 90f;
                float roundedYAngle = Mathf.Round(eulerAngles.y / 90f) * 90f;
                float roundedZAngle = Mathf.Round(eulerAngles.z / 90f) * 90f;

                if (testDebugger)
                {
                    

                }
                else
                {
                    

                    switch (spinAxis)
                    {
                        case SpinAxis.X:
                            target.eulerAngles = new Vector3(roundedXAngle, eulerAngles.y, eulerAngles.z);
                            break;
                        case SpinAxis.Y:
                            target.eulerAngles = new Vector3(eulerAngles.x, roundedYAngle, eulerAngles.z);
                            break;
                        case SpinAxis.Z:
                            target.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, roundedZAngle);
                            break;
                    }
                }
                
            }
            else if (transformationMode == TransformationMode.Position)
            {
                // 위치 스냅 로직을 추가하고 싶다면 여기에 작성
            }

            snapEvent?.Invoke();
        }
    }



    // allows a target Transform to be rotated based on mouse click and drag
    [RequireComponent(typeof(Collider))]
    public class DragSpinner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
    {
        [SerializeField] private SpinnerSettings settings;
        [SerializeField] private Quaternion quaternion;
       
        void Start()
        {
            quaternion = transform.rotation;
            settings.Initialize();
            EnableSpinner(true);
            
        }

        private void FixedUpdate()
        {
            if(settings.fakeTarget != null)
            {
                settings.target.rotation = settings.fakeTarget.rotation;
            }
            settings.DoSnap();
        }

        public void OnInitializePotentialDrag(PointerEventData data)
        {
            Debug.Log("potential Debugger");
            if (settings.isActive)
            {
                settings.BeginDrag(data.position);
            }
        }

        public void OnBeginDrag(PointerEventData data)
        {
            
            
        }

        public void OnDrag(PointerEventData data)
        {
            settings.Drag(data.position);
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (settings.isActive)
            {
                settings.Snap();
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

}