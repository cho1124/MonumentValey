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
       

        // 대상 오브젝트의 새로운 위치 계산 (Z축 반전)
        transform.position = new Vector3(playerPosition.x, playerPosition.y, originPlayerZ + FakeZPos);
        // Mirror rotation logic
        Quaternion playerRotation = player.transform.rotation;

        // 오일러 각도로 변환하여 Y축 각도를 확인
        Vector3 playerEulerAngles = playerRotation.eulerAngles;

        // Y축이 특정 각도 범위에 있을 때만 반전
        float mirroredY = playerEulerAngles.y;

        // 조건 1: 90도에서 0도 또는 180도로 회전할 때

        // 다시 쿼터니언으로 변환하여 회전 적용
        Quaternion mirroredRotation = Quaternion.Euler(playerEulerAngles.x, mirroredY, playerEulerAngles.z);


        
        // 결과를 오브젝트에 적용
        transform.rotation = mirroredRotation;
    }

}
