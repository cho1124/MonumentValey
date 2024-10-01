using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testMove : MonoBehaviour
{
    public float moveSpeed = 5f;  // �̵� �ӵ�

    void Update()
    {
        // Ű���� �Է� (�¿� ����Ű �Ǵ� A, D Ű)
        float horizontalInput = Input.GetAxis("Horizontal");

        // X ���� �������� �θ� ��ü �̵�
        Vector3 newPosition = transform.position + new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0f, 0f);

        transform.position = newPosition;
    }
}
