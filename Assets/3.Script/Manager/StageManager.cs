using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageManager : MonoBehaviour
{
    public static List<Stage> stages;

    [SerializeField] private float maxRaycastDistance = 100f;

    private Camera mainCamera;
    private int layerMask;

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

                if(selectable.GetCurrentState() is OpenedState)
                {
                    Debug.Log("씬로드할예정");
                    SceneLoad(selectable.stage.stageNum);
                    return;
                }


            }
        }        
    }

}