using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;




namespace RW.MonumentValley
{
    public class TotemSettings
    {
        [Header("토템세팅")]
        public Transform target;
        public float moveSpeed = 1f;
        public float pivot;
        public int minDragDist = 10;
        public UnityEvent snapEvent; //이건 사용할 지는 모르겠네

        private DirectorControlPlayable asd;

        [HideInInspector] public bool isSpinning = false;
        [HideInInspector] public float previousAngleToMouse;
        [HideInInspector] public bool isActive = true;

        /*해야 될 것
         1. 맨 처음에 토템과 가장 근접한 노드를 찾아 위에 놓기
         2. 추가로 Node에 isTotemMove를 추가하여 isTotemMove 상태에 있는 가장 가까운 노드
         3. 마우스를 드래그해서 드래그한 위치에 있는 가장 가까운 노드를 찾은 후
         4. 플레이어 컨트롤에 있는  FindBestPath 등을 활용해서 이동시키기
         5. 토템의 특정 노드가 이동 가능한 거리로 닿아 있을 때 활성화할 수 있도록 Event
         6. 토템에 대한 추가적인 로직 세팅은.....
          
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

