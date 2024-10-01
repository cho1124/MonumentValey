using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testCo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveLocalPositionX());
    }

    

    private IEnumerator MoveLocalPositionX()
    {
        float moveTime = 2f; // 이동에 걸리는 시간
        


        while(true)
        {
            Vector3 startPosition = transform.localPosition;
            Vector3 endPosition = new Vector3(5f, transform.localPosition.y, transform.localPosition.z);
            float elapsedTime = 0f;
            while (elapsedTime < moveTime)
            {
                elapsedTime += Time.deltaTime;
                float lerpValue = Mathf.Clamp01(elapsedTime / moveTime);

                // X 좌표 0에서 5까지 Lerp로 이동
                transform.localPosition = Vector3.Lerp(startPosition, endPosition, lerpValue);

                yield return null; // 프레임마다 대기
            }

            // X 좌표 5부터 0까지 이동
            elapsedTime = 0f;
            startPosition = transform.localPosition;
            endPosition = new Vector3(0f, transform.localPosition.y, transform.localPosition.z);

            while (elapsedTime < moveTime)
            {
                elapsedTime += Time.deltaTime;
                float lerpValue = Mathf.Clamp01(elapsedTime / moveTime);

                // X 좌표 5에서 0까지 Lerp로 이동
                transform.localPosition = Vector3.Lerp(startPosition, endPosition, lerpValue);

                yield return null; // 프레임마다 대기
            }

            yield return null;
        }

        // X 좌표 0부터 5까지 이동
        
    }
}
