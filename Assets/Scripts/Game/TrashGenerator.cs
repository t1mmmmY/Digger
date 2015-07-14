using UnityEngine;
using System.Collections;

public class TrashGenerator : MonoBehaviour 
{
    [SerializeField] Transform[] points;
    [SerializeField] Vector2 radius;

    [Range(0.0f, 180.0f)]
    [SerializeField]
    float rotationAngle = 30.0f;

    [Range(0.0f, 1.0f)]
    [SerializeField] float density = 0.5f;

    [Range(0, 10)]
    [SerializeField] int minDistance = 3;

    [SerializeField] GameObject[] trashPrefabs;

    int lastTrashIndex = -1;
    int distance = 0;

    void Start()
    {
        CreateTrash(true);       
    }

    void OnEnable()
    {
        InfiniteMap.OnAddLine += OnAddLine;
    }

    void OnDisable()
    {
        InfiniteMap.OnAddLine -= OnAddLine;
    }

    void OnAddLine()
    {
        foreach(Transform point in points)
        {
            point.Translate(0, -1, 0);
        }

        distance++;
        bool isCreated = false;
        if (distance >= minDistance)
        {
            isCreated = CreateTrash();
        }

        if (isCreated)
        {
            distance = 0;
        }
        
    }

    bool CreateTrash(bool createAlways = false)
    {
        if (!createAlways)
        {
            float randomValue = Random.Range(0.0f, 1.0f);
            if (randomValue > density)
            {
                return false;
            }
        }

        int pointNumber = Random.Range(0, points.Length);
        Vector3 trashPosition = points[pointNumber].transform.localPosition + new Vector3(Random.Range(-radius.x, radius.x), Random.Range(-radius.y, radius.y), 0);

        GameObject trash = GameObject.Instantiate<GameObject>(GetRandomTrash());
        trash.transform.parent = this.transform;
        trash.transform.localPosition = trashPosition;
        trash.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-rotationAngle, rotationAngle));

        return true;
    }

    GameObject GetRandomTrash()
    {
        int trashNumber = -1;
        do
        {
            trashNumber = Random.Range(0, trashPrefabs.Length);

        } while (trashNumber == lastTrashIndex);

        lastTrashIndex = trashNumber;

        return trashPrefabs[trashNumber];
    }
}
