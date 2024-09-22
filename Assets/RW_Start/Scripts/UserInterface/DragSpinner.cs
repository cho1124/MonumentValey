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
        public Transform target; // 변형시킬 Transform
        [Header("목표 타겟")]
        public AimedObject[] aimedObjects;
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
        [HideInInspector] public float previousAngleToMouse;
        [HideInInspector] public bool isActive = true;

        [Header("position 이동 상태에서의 최소 최대 거리")]
        [SerializeField] private Transform minTransform;
        [SerializeField] private Transform maxTransform;

        
        protected Camera mainCamera;
        
        private float ratio = 0f; //mover를 움직이기 위한 비율

        public void Initialize()
        {
            mainCamera = Camera.main;
            //targetRot = target.transform.rotation;
        }

        public void BeginDrag(Vector2 mousePosition)
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

       
        private void MoveTarget(Vector2 mousePosition)
        {
            
            Vector3 directionToMouse = mousePosition - (Vector2)mainCamera.WorldToScreenPoint(pivot.position);
            Vector3 axisDirection = GetAxisDirection();
            Vector3 newDir = new Vector3(axisDirection.x * directionToMouse.x, axisDirection.y * directionToMouse.y, axisDirection.z * directionToMouse.z);

            target.position += newDir * moveSpeed * Time.deltaTime; // 직접적인 위치값 제한 혹은

            target.position = new Vector3(Mathf.Clamp(target.position.x, minTransform.position.x, maxTransform.position.x),
                Mathf.Clamp(target.position.y, minTransform.position.y, maxTransform.position.y),
                Mathf.Clamp(target.position.z, minTransform.position.z, maxTransform.position.z));

            ratio = (target.position - minTransform.position).magnitude / (maxTransform.position - minTransform.position).magnitude;
            //

           
            if(aimedObjects.Length != 0)
            {
                for(int i = 0; i < aimedObjects.Length; i++)
                {
                    //개얼탱이슈
                    //
                    aimedObjects[i].MoveByRatio(ratio);
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

        
        public virtual void Snap()
        {
            isSpinning = false;
            
            if (transformationMode == TransformationMode.Rotation)
            {
                Vector3 eulerAngles = target.eulerAngles;
                Vector3 newAngles = Vector3.zero;
                float roundedXAngle = Mathf.Round(eulerAngles.x / 90f) * 90f;
                float roundedYAngle = Mathf.Round(eulerAngles.y / 90f) * 90f;
                float roundedZAngle = Mathf.Round(eulerAngles.z / 90f) * 90f;

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
            else if (transformationMode == TransformationMode.Position)
            {
                // 위치 스냅 로직을 추가하고 싶다면 여기에 작성
            }

            snapEvent?.Invoke();
        }
    }

    // allows a target Transform to be rotated based on mouse click and drag
    [RequireComponent(typeof(Collider))]
    public class DragSpinner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private SpinnerSettings settings;
        
        void Start()
        {
            settings.Initialize();
            EnableSpinner(true);
            
        }

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