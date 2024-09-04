using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Queue<CinemachineVirtualCamera> virtualCameras;
    [SerializeField] private List<CinemachineVirtualCamera> virtualCams;
    public CinemachineVirtualCamera currentVirtualCamera;
    public UnityEvent CameraEvent;

    private void Start()
    {
        virtualCameras = new Queue<CinemachineVirtualCamera>();


        foreach (CinemachineVirtualCamera virtualCamera in virtualCams)
        {
            virtualCameras.Enqueue(virtualCamera);
        }

        InitPrority();
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            UpdateQueue();
        }
    }

    private void InitPrority()
    {
        foreach (CinemachineVirtualCamera cinemachineVirtualCamera in virtualCameras)
        {
            cinemachineVirtualCamera.Priority = 1;
        }

        currentVirtualCamera = virtualCameras.Dequeue();
        currentVirtualCamera.Priority = 10;
    }

    private void UpdateQueue()
    {
        if (currentVirtualCamera == null) return;

        currentVirtualCamera.Priority = 1;
        virtualCameras.Enqueue(currentVirtualCamera);

        currentVirtualCamera = virtualCameras.Dequeue();
        currentVirtualCamera.Priority = 10;
        //currentVirtualCamera
        CameraEvent?.Invoke();
    }


}
