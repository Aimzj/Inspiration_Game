using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public List<GameObject> EnemyPool;
    public GameObject EnemyObj;

    public Transform[] SpawnPoints;
    public float SpawnIntervals;

    public int NumEnemies;
    public int MaxEnemies;

    public bool IsSpawning;
	// Use this for initialization
	void Start () {
        NumEnemies = 0;
        IsSpawning = true;

        for(int k=0; k<MaxEnemies; k++)
        {
            GameObject temp = (GameObject)Instantiate(EnemyObj, Vector3.zero, Quaternion.identity);
            temp.SetActive(false);
            EnemyPool.Add(temp);
        }

        InvokeRepeating("SpawnEnemy", 0, SpawnIntervals);
	}
	
    void SpawnEnemy()
    {
        if (IsSpawning)
        {
            if (NumEnemies < MaxEnemies)
            {
                int rand = Random.Range(0, SpawnPoints.Length);

                for (int k = 0; k < EnemyPool.Count; k++)
                {
                    if (!EnemyPool[k].activeInHierarchy)
                    {
                        EnemyPool[k].transform.position = SpawnPoints[rand].position;
                        EnemyPool[k].transform.rotation = transform.rotation;
                        EnemyPool[k].SetActive(true);
                        break;
                    }
                }
                NumEnemies++;
            }
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
