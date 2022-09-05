using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class EnvManager : MonoBehaviour
{
    [SerializeField] HeroAgent agent;
    [SerializeField] GameObject enemy;

    List<GameObject> enemies;
    int numEnemies;

    // Start is called before the first frame update
    void Start()
    {
        enemies = new List<GameObject>();
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemies()
    {
        ClearEnemies();
        agent.gameObject.transform.position = transform.position;
        int num = 4;
        numEnemies = num;
        for (int i = 0; i < num; i++)
        {
            GameObject obj = Instantiate(enemy, transform.position + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0), Quaternion.identity);
            obj.GetComponent<EnemyController>().Setup(this, agent);
            enemies.Add(obj);
        }
    }

    public void EnemyDestroyed()
    {
        numEnemies--;
        if (numEnemies == 0)
        {
            agent.Win();
        }
    }

    public void ClearEnemies()
    {
        foreach(GameObject obj in enemies)
        {
            Destroy(obj);
        }
        enemies.Clear();
    }
}
