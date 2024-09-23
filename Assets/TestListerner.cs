using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestListerner : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(test_co(1));
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(test_co(3));
        }
    }

    public void Move(int count)
    {
        if(count <= 0)
        {
            return;
        }
        count--;
        
        Move(count);

    }
    private IEnumerator test_co(int count)
    {
        while(count > 0)
        {
            count--;
            Debug.Log("Moved, RemainedCount : " + count);
            //Move로직 시작(코루틴)

            yield return new WaitForSeconds(1f);
        }
    }
}
