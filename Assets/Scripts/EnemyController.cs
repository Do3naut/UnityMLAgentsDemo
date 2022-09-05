using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class EnemyController : MonoBehaviour
{
    EnvManager manager;
    HeroAgent hero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Setup(EnvManager man, HeroAgent agent)
    {
        manager = man;
        hero = agent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("player"))
        {
            Debug.Log("Collided");
            manager.EnemyDestroyed();
            hero.EnemyDestroyed();
            Destroy(gameObject);
        }
    }
}
