using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testMove : MonoBehaviour
{
    public float moveSpeed = 5f;  // 이동 속도

    void Update()
    {
        // 키보드 입력 (좌우 방향키 또는 A, D 키)
        float horizontalInput = Input.GetAxis("Horizontal");

        // X 축을 기준으로 부모 객체 이동
        Vector3 newPosition = transform.position + new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0f, 0f);

        transform.position = newPosition;
    }
}
