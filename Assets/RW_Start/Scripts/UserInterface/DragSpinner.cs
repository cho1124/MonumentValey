using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

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
    public class SpinnerSettings
    {
        public Transform target; // 변형시킬 Transform
        public SpinAxis spinAxis = SpinAxis.X; // 회전 축 설정
        public float rotationSpeed = 1.0f; // 회전 속도
        public float moveSpeed = 1.0f; // 위치 변경 속도

        public Transform pivot; // 피벗 포인트
        public int minDragDist = 10; // 최소 드래그 거리

        public TransformationMode transformationMode = TransformationMode.Rotation; // 변형 모드 설정
        public UnityEvent snapEvent; // 스냅 이벤트

        // 상태 관리 변수들
        [HideInInspector] public bool isSpinning = false;
        [HideInInspector] public float previousAngleToMouse;
        [HideInInspector] public bool isActive = true;

        public void Initialize()
        {
            // 초기화 시 필요한 설정
        }

        public void BeginDrag(Vector2 mousePosition)
        {
            isSpinning = true;
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
                if (transformationMode == TransformationMode.Rotation)
                {
                    RotateTarget(mousePosition);
                }
                else if (transformationMode == TransformationMode.Position)
                {
                    MoveTarget(mousePosition);
                }
            }
        }

        private void RotateTarget(Vector2 mousePosition)
        {
            Vector3 directionToMouse = mousePosition - (Vector2)Camera.main.WorldToScreenPoint(pivot.position);
            float angleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

            if (directionToMouse.magnitude > minDragDist)
            {
                Vector3 axisDirection = GetAxisDirection();
                Vector3 newRotationVector = (previousAngleToMouse - angleToMouse) * axisDirection * rotationSpeed;
                target.Rotate(newRotationVector);
                previousAngleToMouse = angleToMouse;
            }
        }

        private void MoveTarget(Vector2 mousePosition)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.WorldToScreenPoint(target.position).z));
            Vector3 offset = worldPosition - target.position;
            target.position += offset * moveSpeed * Time.deltaTime;
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

        public void Snap()
        {
            isSpinning = false;

            if (transformationMode == TransformationMode.Rotation)
            {
                Vector3 eulerAngles = target.eulerAngles;
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
            //settings.Initialize();
            EnableSpinner(true);
        }

        public void OnBeginDrag(PointerEventData data)
        {
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