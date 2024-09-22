using UnityEngine;
using UnityEditor;

public class AddPrefabToMultipleParents : MonoBehaviour
{
    [MenuItem("Tools/Add Prefab to Selected")]
    static void AddPrefabToSelected()
    {
        // 넣고자 하는 프리팹을 지정합니다.
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/2.Model/Prefabs/ClickableNodes.prefab");

        // 선택된 부모 오브젝트 가져오기
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject parent in selectedObjects)
        {
            // 프리팹을 부모로 지정된 오브젝트의 자식으로 추가
            GameObject newChild = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent.transform);
            newChild.transform.localPosition = Vector3.zero; // 부모의 로컬 위치 기준으로 위치 조정 가능
        }
    }
}
