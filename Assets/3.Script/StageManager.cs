using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public void SceneLoad(int a)
    {
        SceneManager.LoadScene(a);

    }
    public void SceneLoad(string s)
    {
        SceneManager.LoadScene(s);
    }
    private void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, LayerMask.NameToLayer("Stage")))
        {
            Debug.Log("hitted num : " + hit.transform.GetComponent<Stage>().stageNum);
            //SceneLoad(hit.transform.GetComponent<Stage>().stageNum);
        }

    }
}
