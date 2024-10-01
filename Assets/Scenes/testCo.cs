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
        float moveTime = 2f; // �̵��� �ɸ��� �ð�
        


        while(true)
        {
            Vector3 startPosition = transform.localPosition;
            Vector3 endPosition = new Vector3(5f, transform.localPosition.y, transform.localPosition.z);
            float elapsedTime = 0f;
            while (elapsedTime < moveTime)
            {
                elapsedTime += Time.deltaTime;
                float lerpValue = Mathf.Clamp01(elapsedTime / moveTime);

                // X ��ǥ 0���� 5���� Lerp�� �̵�
                transform.localPosition = Vector3.Lerp(startPosition, endPosition, lerpValue);

                yield return null; // �����Ӹ��� ���
            }

            // X ��ǥ 5���� 0���� �̵�
            elapsedTime = 0f;
            startPosition = transform.localPosition;
            endPosition = new Vector3(0f, transform.localPosition.y, transform.localPosition.z);

            while (elapsedTime < moveTime)
            {
                elapsedTime += Time.deltaTime;
                float lerpValue = Mathf.Clamp01(elapsedTime / moveTime);

                // X ��ǥ 5���� 0���� Lerp�� �̵�
                transform.localPosition = Vector3.Lerp(startPosition, endPosition, lerpValue);

                yield return null; // �����Ӹ��� ���
            }

            yield return null;
        }

        // X ��ǥ 0���� 5���� �̵�
        
    }
}
