using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using DG.Tweening;
using System.Linq;


namespace RW.MonumentValley
{
    public enum TransformationMode
    {
        Rotation,
        Position
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
        [Header("MultiplierValue")]
        public float multiplyValue = 1f;

        [Header("Mover")]
        public Transform target;
        public Transform startTr;
        public Transform endTr;
        public bool isMover = true;
        private float accumulateValue = 0f;
        [SerializeField] private int direction = 1;
        private bool checkRatio = false;
        [SerializeField] private bool isToggle = false;

        [Header("Rotator")]
        public SpinAxis spinAxis = SpinAxis.X;


        


        public void MoveByRatio(float ratio, float multiplier)
        {
            // ratio 값을 multiplier에 따라 변화시키도록 계산
            //ratio = Mathf.Clamp01(ratio * multiplier);  // Clamp로 값이 0~1 사이에 있게 유지

            // 위치 이동
            target.transform.localPosition = Vector3.Lerp(startTr.localPosition, endTr.localPosition, ratio);

            // 회전 이동
            target.transform.localRotation = Quaternion.Slerp(startTr.localRotation, endTr.localRotation, ratio);

            // 디버깅용 로그 출력
            //Debug.Log($"MoveByRatio: Ratio = {ratio}, Multiplier = {multiplier}");
        }

        public void MoveByRotationRatio(float ratio, float multiplier)
        {

            if(!checkRatio)
            {
                float totalDistance = Vector3.Distance(startTr.localPosition, endTr.localPosition); // 시작과 끝 사이의 전체 거리
                float currentDistance = Vector3.Distance(startTr.localPosition, target.transform.localPosition); // 시작과 현재 사이의 거리

                // 현재 위치가 startTr와 endTr 사이에서 어느 비율에 있는지 계산 (0은 start, 1은 end)
                float currentRatio = Mathf.Clamp01(currentDistance / totalDistance);
                accumulateValue = currentRatio;
                checkRatio = true;
            }


            //Debug.Log("ratio is " + ratio);
            accumulateValue += direction * ratio * multiplier;

            // 누적된 값이 0보다 작거나 1보다 클 때 방향을 반전
            if (accumulateValue > 1f)
            {
                if(ratio > 0f)
                {
                    direction = -1;        // 방향을 반전하여 감소하도록
                }
                else
                {
                    direction = 1;
                }

                accumulateValue = 1f;  // 값을 1로 고정
                
            }
            else if (accumulateValue < 0f)
            {
                if(ratio > 0f)
                {
                    direction = 1;         // 방향을 반전하여 증가하도록
                }
                else
                {
                    direction = -1;
                }

                accumulateValue = 0f;  // 값을 0으로 고정
                
            }

            
            // 위치 이동
            target.transform.localPosition = Vector3.Lerp(startTr.localPosition, endTr.localPosition, accumulateValue);

            // 회전 이동
            target.transform.localRotation = Quaternion.Slerp(startTr.localRotation, endTr.localRotation, accumulateValue);

            // 디버깅용 로그 출력
            //Debug.Log($"MoveByRatio: Ratio = {ratio}, Multiplier = {multiplier}");
        }

        public void SnapAccumulateValue()
        {
            float[] snapValues;

            if (isToggle)
            {
                snapValues = new float[] { 0f, 1f };
            }
            else
            {
                snapValues = new float[] { 0f, 0.5f, 1f };
            }

            // 0, 0.5, 1 중에서 accumulateValue와 가장 가까운 값을 찾기


            // 현재 accumulateValue와 각 스냅 값의 차이를 계산하여 가장 작은 차이를 가진 값을 찾음
            float closestValue = snapValues.OrderBy(v => Mathf.Abs(accumulateValue - v)).First();

            // accumulateValue를 가장 가까운 값으로 스냅
            accumulateValue = closestValue;

            target.transform.localPosition = Vector3.Lerp(startTr.localPosition, endTr.localPosition, accumulateValue);

            // 회전 이동
            target.transform.localRotation = Quaternion.Slerp(startTr.localRotation, endTr.localRotation, accumulateValue);


            // 디버그 로그로 확인
            //Debug.Log("Snapped accumulateValue: " + accumulateValue);
        }


        public void RotateByRatio(float rotationRatio, float value)
        {
            // rotationRatio를 실제 각도로 변환
            float rotationAmount = 360f * rotationRatio * value;  // 전체 회전 비율을 각도로 변환
            Vector3 rotationAxis; // 회전할 축

            // 회전 축 선택
            switch (spinAxis)
            {
                case SpinAxis.X:
                    rotationAxis = Vector3.right;
                    break;

                case SpinAxis.Y:
                    rotationAxis = Vector3.up;
                    break;

                case SpinAxis.Z:
                    rotationAxis = Vector3.forward;
                    break;

                default:
                    rotationAxis = Vector3.up;
                    break;
            }

            // 회전 적용 (Quaternion 사용)
            Quaternion rotation = Quaternion.AngleAxis(rotationAmount, rotationAxis * direction);
            target.transform.rotation = rotation * target.transform.rotation;
        }

        
        public void SnapByRatio(Vector3 originVec)
        {
            target.transform.rotation = Quaternion.Euler(originVec);
        }

    }

    [Serializable]
    public class SpinnerSettings
    {
        [Header("이동시킬 타겟")]
        public Transform target; // 변형시킬 Transform
        [Header("목표 타겟")]
        public AimedObject[] aimedObjects;
        public AimedObject[] chainedObjects;

        [Header("회전 축")]
        public SpinAxis spinAxis = SpinAxis.X; // 회전 축 설정



        public float rotationSpeed = 1.0f; // 회전 속도
        public float moveSpeed = 1.0f; // 위치 변경 속도
        
        public Transform pivot; // 피벗 포인트
        public int minDragDist = 10; // 최소 드래그 거리

        [Header("이동 타입")]
        public TransformationMode transformationMode = TransformationMode.Rotation; // 변형 모드 설정
        public UnityEvent snapEvent; // 스냅 이벤트

        // 상태 관리 변수들
        [HideInInspector] public bool isSpinning = false;
        [HideInInspector] public bool isActive = true;
        [HideInInspector] public float previousAngleToMouse;

        [Header("position 이동 상태에서의 최소 최대 거리")]
        [SerializeField] private Transform minTransform;
        [SerializeField] private Transform maxTransform;

        public Transform MinTransform => minTransform;
        public Transform MaxTransform => maxTransform;


        [Header("뒤틀린 축 방지")]
        [SerializeField] private bool isCrossed = false;

        protected Camera mainCamera;
        
        private float ratio = 0f; //mover를 움직이기 위한 비율

        public void Initialize()
        {
            mainCamera = Camera.main;
            //targetRot = target.transform.rotation;
        }

        public virtual void BeginDrag(Vector2 mousePosition)
        {
            isSpinning = true;
            //Debug.Log(isSpinning);
            
            
            if (transformationMode == TransformationMode.Rotation)
            {
                Vector3 directionToMouse = mousePosition - (Vector2)Camera.main.WorldToScreenPoint(pivot.position);
                //Debug.Log("rotate Mode");
                previousAngleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            }
        }

        public virtual void Drag(Vector2 mousePosition)
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
                        //MoveTargetFake(mousePosition);
                        break;
                    default:
                        Debug.LogError("트랜스폼 모드가 할당 안됨");
                        break;
                }

            }
        }

        protected virtual void RotateTarget(Vector2 mousePosition)
        {
            Vector3 directionToMouse = mousePosition - (Vector2)mainCamera.WorldToScreenPoint(pivot.position);



            float angleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

            if (directionToMouse.magnitude > minDragDist)
            {
                Vector3 axisDirection = GetAxisDirection();

                float rotationDelta = previousAngleToMouse - angleToMouse;

                // 각도를 0 ~ 360도로 래핑(Wrapping)
                rotationDelta = (rotationDelta + 360f) % 360f;

                if (rotationDelta > 180f)
                {
                    rotationDelta -= 360f; // 시계 방향으로 회전할 때
                }
                else if (rotationDelta < -180f)
                {
                    rotationDelta += 360f; // 반시계 방향으로 회전할 때
                }


                Vector3 newRotationVector = rotationDelta * axisDirection * rotationSpeed;


                if(isCrossed)
                {
                    newRotationVector = -newRotationVector;
                }

                target.Rotate(newRotationVector, Space.World);
                
                float rotationRatio = rotationDelta / 360f; // 전체 회전 각도에 대한 비율
                                                            //Debug.Log("rotationRatio : " + rotationRatio);

                

                //Debug.Log("rotateDelta : " + rotationDelta);

                // 다른 오브젝트의 이동/회전 처리
                if (aimedObjects.Length != 0)
                {
                    for (int i = 0; i < aimedObjects.Length; i++)
                    {
                        // 회전 비율에 따라 이동시키거나 회전시키는 로직 추가
                        //

                        if(aimedObjects[i].isMover)
                        {
                            aimedObjects[i].MoveByRotationRatio(rotationRatio, aimedObjects[i].multiplyValue);

                        }
                        else
                        {
                            aimedObjects[i].RotateByRatio(rotationRatio, aimedObjects[i].multiplyValue);
                            
                        }

                    }
                }
                previousAngleToMouse = angleToMouse;
            }
        }

        private void MoveTarget(Vector2 mousePosition)
        {

           
            Vector3 CurrentDragPos = new Vector3(mousePosition.x, mousePosition.y, Camera.main.WorldToScreenPoint(pivot.position).z);
            Vector3 CurrentDragWorldPos = Camera.main.ScreenToWorldPoint(CurrentDragPos);

            Vector3 axisDirection = GetAxisDirection(); // z축 (0, 0, z)

            Vector3 newDir = new Vector3(axisDirection.x * CurrentDragWorldPos.x,
                                         axisDirection.y * CurrentDragWorldPos.y,
                                         axisDirection.z * CurrentDragWorldPos.z);

            target.position = newDir;

            target.position = new Vector3(Mathf.Clamp(target.position.x, minTransform.position.x, maxTransform.position.x),
                Mathf.Clamp(target.position.y, minTransform.position.y, maxTransform.position.y),
                Mathf.Clamp(target.position.z, minTransform.position.z, maxTransform.position.z));


            ratio = (target.position - minTransform.position).magnitude / (maxTransform.position - minTransform.position).magnitude;
            
            
            if(aimedObjects.Length != 0)
            {
                for(int i = 0; i < aimedObjects.Length; i++)
                {
                    
                    aimedObjects[i].MoveByRatio(ratio, aimedObjects[i].multiplyValue);
                }
            }

        }

        protected Vector3 GetAxisDirection()
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

        public void CompareAndSwap()
        {
            switch(spinAxis)
            {
                case SpinAxis.X:
                    Swap(minTransform.position.x, maxTransform.position.x);
                    return;
                case SpinAxis.Y:
                    Swap(minTransform.position.y, maxTransform.position.y);
                    return;
                case SpinAxis.Z:
                    Swap(minTransform.position.z, maxTransform.position.z);
                    return;
                default:
                    return;
            }

        }

        private void Swap(float minPosition, float maxPosition)
        {
            if(minPosition > maxPosition)
            {
                Transform temp = minTransform;
                minTransform = maxTransform;
                maxTransform = temp;
            }
        }

        public virtual void Snap()             
        {
            isSpinning = false;
            
            if (transformationMode == TransformationMode.Rotation)
            {
                Snapping(target, spinAxis);

                foreach(AimedObject a in aimedObjects)
                {
                    if(!a.isMover)
                    {
                        Snapping(a.target, a.spinAxis);
                    }
                    else
                    {
                        a.SnapAccumulateValue();
                        //SnappingByRatio(a.target, a.accumulateValue);
                    }
                    
                    
                }
            }
            else if (transformationMode == TransformationMode.Position)
            {

                
                

                // 위치 스냅 로직을 추가하고 싶다면 여기에 작성
            }

            snapEvent?.Invoke();
        }

        private void Snapping(Transform tr, SpinAxis spinAxis)
        {
            Quaternion currentRotation = tr.rotation;

            // Quaternion을 Euler 각도로 변환
            Vector3 eulerAngles = currentRotation.eulerAngles;
            // 90도 단위로 각도 스냅
            float roundedXAngle = Mathf.Round(eulerAngles.x / 90f) * 90f;
            float roundedYAngle = Mathf.Round(eulerAngles.y / 90f) * 90f;
            float roundedZAngle = Mathf.Round(eulerAngles.z / 90f) * 90f;

            Quaternion newRotation;

            switch (spinAxis)
            {
                case SpinAxis.X:
                    // X축을 기준으로 회전, 기존 회전을 유지하면서 X축만 스냅
                    newRotation = Quaternion.AngleAxis(roundedXAngle, Vector3.right);
                    tr.rotation = newRotation * Quaternion.Euler(0, eulerAngles.y, eulerAngles.z);
                    break;

                case SpinAxis.Y:
                    // Y축을 기준으로 회전, 기존 회전을 유지하면서 Y축만 스냅
                    newRotation = Quaternion.AngleAxis(roundedYAngle, Vector3.up);
                    tr.rotation = newRotation * Quaternion.Euler(eulerAngles.x, 0, eulerAngles.z);
                    break;

                case SpinAxis.Z:
                    // Z축을 기준으로 회전, 기존 회전을 유지하면서 Z축만 스냅
                    newRotation = Quaternion.AngleAxis(roundedZAngle, Vector3.forward);
                    tr.rotation = newRotation * Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0);
                    break;
            }
        }

        
    }
    
    public class DragSpinner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] public SpinnerSettings settings;

        void Start()
        {            
            settings.Initialize();
            EnableSpinner(true);
            
        }

        public virtual void OnBeginDrag(PointerEventData data)
        {
            //Debug.Log("Drag Beginning");
            if (settings.isActive)
            {
                settings.BeginDrag(data.position);
            }
        }

        public virtual void OnDrag(PointerEventData data)
        {
            settings.Drag(data.position);
        }

        public virtual void OnEndDrag(PointerEventData data)
        {
            //Debug.Log("EndDrag");
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