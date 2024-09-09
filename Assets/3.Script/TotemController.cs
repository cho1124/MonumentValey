using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using UnityEngine.UI;

namespace RW.MonumentValley
{
    [Serializable]
    public class TotemSettings
    {
        [Header("토템세팅")]
        public Transform target;
        public float moveSpeed = 1f;
        public Transform pivot;
        
        
        public int minDragDist = 10;
        public UnityEvent snapEvent; //이건 사용할 지는 모르겠네
        public Text DebugText;

        
        [HideInInspector] public float previousAngleToMouse;
        [HideInInspector] public bool isActive = true;


        //이 부분은 부모 객체에서 수행해야 할 부분
        private Camera mainCamera;
       
        [SerializeField] public Pathfinder pathfinder;
        private Graph graph;
        [SerializeField] private Node[] Nodes;
        public Node currentNode;

        public Node nextNode;
        public float moveTime = 1f; // 이동 시간
        
        public float speed;

        private List<Node> pathNodes = new List<Node>(); // 이동할 경로
        
        private int currentPathIndex = 0;                // 현재 경로에서 몇 번째 노드로 가는지 추적
        private bool isMoving = false;                   // 캐릭터가 이동 중인지 여부
        
        private float elapsedTime = 0f;                  // 이동 중 경과 시간
        


        //1. 따라서 토템컨트롤러는 부모객체에 달아야 함
        //2. 그에 따라서 pivot들을 체계적으로 관리해야함

        /*해야 될 것
         1. 맨 처음에 토템과 가장 근접한 노드를 찾아 위에 놓기
         2. 추가로 Node에 isTotemMove를 추가하여 isTotemMove 상태에 있는 가장 가까운 노드
         3. 마우스를 드래그해서 드래그한 위치에 있는 가장 가까운 노드를 찾은 후
         4. 플레이어 컨트롤에 있는  FindBestPath 등을 활용해서 이동시키기
         5. 토템의 특정 노드가 이동 가능한 거리로 닿아 있을 때 활성화할 수 있도록 Event
         6. 토템에 대한 추가적인 로직 세팅은.....
         7. 진짜 조영준은 개쓰레기멍청이바보
         */

        public void UpdateDebugText()
        {
            if(DebugText != null)
            {
                DebugText.text = $"currentNodes po : {currentNode.transform.position} \n currentPos : {target.transform.position}";
            }
        }

        public void Initialize()
        {
            mainCamera = Camera.main;

            if (pathfinder != null)
            {
                graph = pathfinder.GetComponent<Graph>();
            }
            
        }

        public List<Node> FindPathStart(Node node)
        {
            return pathfinder.FindBestPathForTotem(node, Nodes);
        }

        public void BeginDrag(Vector2 mousePosition)
        {
            //isSpinning = true;
            //pivot들 중 가장 가까운 pivot을 찾기?
            
            
        }

        public void Drag(Vector2 mousePosition)
        {
            if (isActive)
            {
                MoveTotem(mousePosition);
            }
            
        }

        public void EndDrag(Vector2 mousePosition)
        {

        }

        private Node HasNeighbor(Node currentNode, Vector3 mouseDir)
        {
            float maxDot = -1.0f; // 내적의 최대값을 저장 (가장 유사한 방향을 찾기 위해)
            Edge closestEdge = null;
            Node nextNode = null;
            Vector3 normalizedMouseDir = mouseDir.normalized;

            foreach (Edge e in currentNode.Edges)
            {
                if (e.neighbor != null)
                {
                    
                    // 현재 노드와 이웃 노드 사이의 방향 벡터를 구하고 정규화
                    Vector3 testDir = (e.neighbor.transform.position - currentNode.transform.position).normalized;

                    // 두 벡터의 내적을 구하여 유사도를 계산
                    float dot = Vector3.Dot(testDir, normalizedMouseDir);


                    if (dot > maxDot && dot > 0.7f) // 0.7f는 유사성을 판단하는 임계값
                    {
                        maxDot = dot;
                        nextNode = e.neighbor;
                    }


                    //return true;
                }
            }

            return nextNode;
            

            //return false;
        }

        private void MoveTotem(Vector2 mousePosition)
        {
            
            Vector3 directionToMouse = mousePosition - (Vector2)mainCamera.WorldToScreenPoint(pivot.position);
            //마우스와 타겟 사이의 방향벡터
            //그러면 현재 노드와 이웃한 노드들 사이에서 가장 마우스와 타겟 사이의 방향벡터와 가까운 방향을 찾아서 결정한다?

            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            // Ray와 충돌한 객체 정보를 저장할 변수
            RaycastHit hit;

            // Ray가 어떤 객체와 충돌했는지 확인
            if (Physics.Raycast(ray, out hit))
            {
                // 충돌한 객체의 월드 좌표
                Vector3 hitPoint = hit.point;

                // 충돌 위치 출력
                nextNode = graph?.FindClosestNode(hitPoint, true);
                Debug.Log("Hit Points Node : " + nextNode);

            }
            
            //아랫부분은 controller 부분에서 진행할듯
            List<Node> testNodes = pathfinder.FindPath(currentNode, nextNode);

            Debug.Log("in movetototem's testNodes Count : " + testNodes.Count);

            //
            //
            //
            //if (testNodes.Count != 0)
            //{
            //    Debug.Log("nodes Count : " + testNodes.Count);
            //
            //   
            //
            //
            //    foreach(Node node in testNodes)
            //    {
            //       Debug.Log("nodes name : " + node.name);
            //
            //    }
            //    
            //}
            //
            //if (testNodes.Count > 1)
            //{
            //    //StartCoroutine(FollowPathRoutine(newPath));
            //    //StartMoveAlongPath(testNodes);
            //    //이 부분을 코루틴 없이 한번 해보자
            //}
            //
            //MoveAlongPath(target.transform);
            //
            //
            //Vector3 newDir = (nextNode.transform.position - target.position).normalized;
            //target.position = Vector3.MoveTowards(target.position, nextNode.transform.position, moveSpeed  * 5f * Time.deltaTime);
            //
            //currentNode = nextNode;

            //여기서 destNode가 현재 노드의 이웃 노드가 아니라면 리턴을 쌔려버리면 끝일거 같은데
            //TODO: 추가적인 버튼과 timeline 공부하면서 문법 공부 다시하기
            //레벨 셀렉션 시작하기
            //맵 2단계까지 가능하면 끝내기
            //단순히 이동하는 것이 아닌, directionToMouse와 가장 가까운 Node를 찾기
            //애당초 드래그로는 너무 불안정한 노드찾기\
            //해결방안을 계속 생각해보니 그냥 노드를 찾는게 아닌 그냥 처음부터 토템이 이동 가능한 길을 만들고 거기서만 이동할 수 있도록 한다?
        }

        public void StartMoveAlongPath(List<Node> path)
        {
            if (path == null || path.Count <= 1)
            {
                Debug.Log("PLAYERCONTROLLER StartMoveAlongPath: invalid path");
                return;
            }

            pathNodes = path;
            currentPathIndex = 0;
            nextNode = pathNodes[currentPathIndex];
            //isMoving = true;
            elapsedTime = 0f;
            //MoveAlongPath(currentNode.transform);



        }

        // 이동 로직을 처리하는 메서드
        public void MoveAlongPath(Transform currentTr)
        {
            if (nextNode == null) return;

            elapsedTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp(elapsedTime / moveTime, 0f, 1f);

            // 현재 위치에서 목표 노드까지 Lerp로 이동
            Vector3 targetPos = nextNode.transform.position;
            currentTr.position = Vector3.Lerp(currentTr.position, targetPos, lerpValue);

            // 목표 노드에 도달한 경우
            if (lerpValue >= 1f)
            {
                // 현재 노드 갱신 및 다음 노드 설정
                currentTr.parent = nextNode.transform;
                currentNode = nextNode;

                // 해당 노드의 이벤트 실행
                nextNode.gameEvent.Invoke();

                // 다음 노드로 이동 준비
                currentPathIndex++;
                if (currentPathIndex < pathNodes.Count)
                {
                    nextNode = pathNodes[currentPathIndex];
                    elapsedTime = 0f;
                }
                else
                {
                    // 경로 끝에 도달
                    isMoving = false;
                    
                }
            }
        }

        public Transform SnapToNearestNode(Transform _transform, bool isSmoothMove)
        {
            Node nearestNode = graph?.FindClosestNode(_transform.position, true);
            
            if(isSmoothMove)
            {
                if (nearestNode != null && nearestNode.canTotemMove)
                {
                    currentNode = nearestNode;
                    
                    _transform.DOMove(currentNode.transform.position, 0.2f);
                    //_transform.position = nearestNode.transform.position;
                }
            }
            else
            {
                if (nearestNode != null && nearestNode.canTotemMove)
                {
                    currentNode = nearestNode;
                    //_transform.DOMove(currentNode.transform.position, 1f);
                    _transform.position = nearestNode.transform.position;
                }
            }
            return _transform;
            
        }

        //토템 이동 이벤트에서 사용할 것 >>> 이 부분은 아이다도 사용할 가능성이 높다.
        //TODO: PlayerController 스크립트에서도 추가하기, 이 스크립트들에 대한 설계를 다시 해야할 수도

        public void SnapToNode(Transform _transform ,Node node)
        {
            currentNode = node;
            _transform.position = node.transform.position;
        }



    }

    public class TotemController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Range(0.25f, 2f)]
        [SerializeField] private float moveTime = 0.5f;

       
        [Header("토템 세팅")]
        [SerializeField] private TotemSettings totemSettings;
        [SerializeField] private Node startNode;
        [SerializeField] private Node destNode;
        [SerializeField] private List<Node> canMoveNodes;
        

        [Header("컨트롤 제어권")]
        public bool isControlEnabled = false;
        public bool isMoving = false;


        private void Awake()
        {
            totemSettings.Initialize();
            //isMoving = false;
            isControlEnabled = true;
        }

        private void Start()
        {

            totemSettings.SnapToNearestNode(transform, false);
            canMoveNodes = totemSettings.FindPathStart(startNode);
            

            //Vector3.Slerp()
        }

        private void Update()
        {
            totemSettings.UpdateDebugText();
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            
            if (totemSettings.isActive)
            {
                totemSettings.BeginDrag(eventData.position);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            //드래그를 통해 해당하는 노드들만 이동하기 >> 노드가 있는 위치가 아니어도 가장 가깝고 이동 가능한 노드를 찾아서 그 방향으로 토템 이동하는 알고리즘
            if (totemSettings.isActive)
            {
                totemSettings.Drag(eventData.position);

                //Debug.Log("totemSettings nextNode : " + totemSettings.nextNode);

                List<Node> testNodes = totemSettings.pathfinder.FindPath(totemSettings.currentNode, totemSettings.nextNode);

                


                //Debug.Log("currentNode : " + totemSettings.currentNode);

                Debug.Log("testNodes Count : " + testNodes.Count);


                if(testNodes.Count > 1 && Vector3.Distance(transform.position, totemSettings.currentNode.transform.position) < 0.1f)
                {
                    StartCoroutine(FollowPathRoutine(testNodes));
                }
                else
                {
                    isMoving = false;
                }

                
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            
            //나중에 쓸지도 아닐지도 쓸 가능성이 높을듯 >>> 아닌것같기도   
        }
        public IEnumerator FollowPathRoutine(List<Node> path)
        {
            //isMoving = true;

            if (path == null || path.Count <= 1)
            {
                Debug.Log("PLAYERCONTROLLER FollowPathRoutine: invalid path");
            }
            else
            {
                

                // loop through all Nodes
                for (int i = 0; i < path.Count; i++)
                {
                    // use the current Node as the next waypoint
                    totemSettings.nextNode = path[i];

                    


                    // move to the next Node
                    yield return StartCoroutine(MoveToNodeRoutine(transform.position, totemSettings.nextNode));
                }
            }

            

        }

        //  lerp to another Node from current position
        private IEnumerator MoveToNodeRoutine(Vector3 startPosition, Node targetNode)
        {

            float elapsedTime = 0;

            // validate move time
            moveTime = Mathf.Clamp(moveTime, 0.1f, 5f);

            while (elapsedTime < moveTime && targetNode != null && !HasReachedNode(targetNode))
            {

                elapsedTime += Time.deltaTime;
                float lerpValue = Mathf.Clamp(elapsedTime / moveTime, 0f, 1f);

                Vector3 targetPos = targetNode.transform.position;
                transform.position = Vector3.Lerp(startPosition, targetPos, lerpValue);

                // if over halfway, change parent to next node
                if (lerpValue > 0.51f)
                {
                    transform.parent = targetNode.transform;
                    totemSettings.currentNode = targetNode;

                    
                    // invoke UnityEvent associated with next Node
                    targetNode.gameEvent.Invoke();
                    //Debug.Log("invoked GameEvent from targetNode: " + targetNode.name);
                }

                // wait one frame
                yield return null;
            }
        }
        public bool HasReachedNode(Node node)
        {
            if (totemSettings.pathfinder == null || node == null)
            {
                return false;
            }

            float distanceSqr = (node.transform.position - transform.position).sqrMagnitude;

            return (distanceSqr < 0.01f);
        }


    }
}


