using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;
public class FakeCrow : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerController player;
    private Transform newTr;
    float FakeZPos;
    private float previousPlayerZ;
    private float originPlayerZ;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();

        gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        previousPlayerZ = player.transform.position.z + 4;
        originPlayerZ = player.transform.position.z;
        Debug.Log(player.transform.position.z);
        Debug.Log(previousPlayerZ);
    }

    // Update is called once per frame
    void Update()
    {
        // Mirror position: Make sure it's offset relative to the player

        Vector3 playerPosition = player.transform.position;

        //Debug.Log("playerz : " + (playerPosition.z + 4));
        FakeZPos = previousPlayerZ - playerPosition.z;
       

        // ��� ������Ʈ�� ���ο� ��ġ ��� (Z�� ����)
        transform.position = new Vector3(playerPosition.x, playerPosition.y, originPlayerZ + FakeZPos);
        // Mirror rotation logic
        Quaternion playerRotation = player.transform.rotation;

        // ���Ϸ� ������ ��ȯ�Ͽ� Y�� ������ Ȯ��
        Vector3 playerEulerAngles = playerRotation.eulerAngles;

        // Y���� Ư�� ���� ������ ���� ���� ����
        float mirroredY = playerEulerAngles.y;

        // ���� 1: 90������ 0�� �Ǵ� 180���� ȸ���� ��

        // �ٽ� ���ʹϾ����� ��ȯ�Ͽ� ȸ�� ����
        Quaternion mirroredRotation = Quaternion.Euler(playerEulerAngles.x, mirroredY, playerEulerAngles.z);


        
        // ����� ������Ʈ�� ����
        transform.rotation = mirroredRotation;
    }

}
