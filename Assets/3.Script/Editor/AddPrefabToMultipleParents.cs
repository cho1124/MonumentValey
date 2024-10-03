using UnityEngine;
using UnityEditor;

public class AddPrefabToMultipleParents : MonoBehaviour
{
    [MenuItem("Tools/Add Prefab to Selected")]
    static void AddPrefabToSelected()
    {
        // �ְ��� �ϴ� �������� �����մϴ�.
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/2.Model/Prefabs/ClickableNodes.prefab");

        // ���õ� �θ� ������Ʈ ��������
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject parent in selectedObjects)
        {
            // �������� �θ�� ������ ������Ʈ�� �ڽ����� �߰�
            GameObject newChild = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent.transform);
            newChild.transform.localPosition = Vector3.zero; // �θ��� ���� ��ġ �������� ��ġ ���� ����
        }
    }
}
