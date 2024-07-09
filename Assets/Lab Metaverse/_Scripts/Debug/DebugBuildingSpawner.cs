using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class DebugBuildingSpawner : MonoBehaviour
{
    public GameObject buildingPrefab;
    public int buildingCount = 100;
    public float buildingSpawnRadius = 100f;
    public GameObject buildingParent;

    // Start is called before the first frame update
    void Start()
    {
        // get Buildings gameObject parent
        
        // spawn buildings in random position
        for (int i = 0; i < buildingCount; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-buildingSpawnRadius, buildingSpawnRadius), 0, Random.Range(-buildingSpawnRadius, buildingSpawnRadius));
            // check if too close to player
            if (Vector3.Distance(randomPosition, Vector3.zero) < 25)
            {
                i--;
                continue;
            }
            Instantiate(buildingPrefab, buildingParent.transform);
            buildingPrefab.transform.position = randomPosition;
            // random size
            buildingPrefab.transform.localScale = new Vector3(Random.Range(5, 20), Random.Range(5, 15), Random.Range(5, 20));
            // reposition y
            buildingPrefab.transform.position = new Vector3(buildingPrefab.transform.position.x, buildingPrefab.transform.localScale.y / 2 - 0.5f, buildingPrefab.transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif
