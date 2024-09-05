using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;




namespace RW.MonumentValley
{
    public class TotemSettings
    {
        [Header("���ۼ���")]
        public Transform target;
        public float moveSpeed = 1f;
        public float pivot;
        public int minDragDist = 10;
        public UnityEvent snapEvent; //�̰� ����� ���� �𸣰ڳ�

        private DirectorControlPlayable asd;

        [HideInInspector] public bool isSpinning = false;
        [HideInInspector] public float previousAngleToMouse;
        [HideInInspector] public bool isActive = true;

        /*�ؾ� �� ��
         1. �� ó���� ���۰� ���� ������ ��带 ã�� ���� ����
         2. �߰��� Node�� isTotemMove�� �߰��Ͽ� isTotemMove ���¿� �ִ� ���� ����� ���
         3. ���콺�� �巡���ؼ� �巡���� ��ġ�� �ִ� ���� ����� ��带 ã�� ��
         4. �÷��̾� ��Ʈ�ѿ� �ִ�  FindBestPath ���� Ȱ���ؼ� �̵���Ű��
         5. ������ Ư�� ��尡 �̵� ������ �Ÿ��� ��� ���� �� Ȱ��ȭ�� �� �ֵ��� Event
         6. ���ۿ� ���� �߰����� ���� ������.....
          
         */
        
        


    }

    public class TotemController : MonoBehaviour
    {
        // Start is called before the first frame update

        private void Start()
        {
            //Vector3.Slerp()
        }


    }
}

