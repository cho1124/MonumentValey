using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageManager : MonoBehaviour
{
    public static List<Stage> stages;

    [SerializeField] private float maxRaycastDistance = 100f; // 최대 레이캐스트 거리

    private Camera mainCamera;      // 카메라 캐싱
    private int layerMask;          // 레이어 마스크 캐싱

    public static void SceneLoad(int a)
    {
        SceneManager.LoadScene(a);

    }

    public static void SceneLoad(string s)
    {
        SceneManager.LoadScene(s);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        layerMask = LayerMask.GetMask("Stage");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray directionToMouse = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit hit;

            if (Physics.Raycast(directionToMouse, out hit, maxRaycastDistance, layerMask))
            {
                Selectable selectable = hit.transform.GetComponentInParent<Selectable>();
                selectable.ChangeState(selectable.GetOpeningState());
            }
        }        
    }

}