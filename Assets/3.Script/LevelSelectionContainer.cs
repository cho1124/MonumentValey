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
            //1. ���� ���ϱ�
            //2. ���⸸ŭ �� ���ϱ�


            Vector3 newRotationVector = (previousAngleToMouse - angleToMouse) * axisDirection * rotationSpeed; //y�� ����

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
    //TODO: level class�� ��?�� �ڷᱸ�� ����� >> Ŭ���� ����� ���� �ε巴�� �ϸ� �ɵ�, �׸��� ���鿡�� ���̸� ��� �� Ȥ�� 1������������ ���� ��� ��ø����ؼ� �ش��ϴ� ������ �°� �����簡 Ȥ�� ���̸� ��簡 �˾Ƽ� �ϸ� �Ǳ� �ҵ�

    [SerializeField] private LevelRotator settings;
    [SerializeField] private List<GameObject> stageObj;
    [SerializeField] private float totalRotation; // ������ ȸ����
    [SerializeField] private Transform stageTr;
    public List<Stage> stages;
    private int count = 0;
    private Quaternion previousRotation; // ���� �������� ȸ����
    private int totalRevolutions; // �� ȸ���� ���� ��


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

        // ���� ȸ������ ���� ȸ������ ���̸� ���� (Quaternion ���� ���� ����)
        float angleDelta = Mathf.Sign(currentRotation.eulerAngles.magnitude - previousRotation.eulerAngles.magnitude) * Quaternion.Angle(previousRotation, currentRotation);

        // ���� ȸ�� ������ �߰�
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

