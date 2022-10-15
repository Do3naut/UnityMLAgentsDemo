using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRManager : MonoBehaviour
{
    private int numAlive;

    [SerializeField] BRAgent enemy0;
    [SerializeField] BRAgent enemy1;

    Vector3 enemy0Start;
    Vector3 enemy1Start;


    public int enemyCount
    {
        get
        {
            // Return number of alive enemies, so -1 because one is always yourself.
            return numAlive - 1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        enemy0Start = enemy0.transform.position;
        enemy1Start = enemy1.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Register what agent hit which other agent 
    public void RegisterHit(BRAgent src, BRAgent targ)
    {
        // Right now for 2 player duel: Reward winning agent and end episode
        src.Win();
        targ.EndEpisode();
    }

    public void EndAllAgents()
    {
        enemy0.EndEpisode();
        enemy1.EndEpisode();
    }

    public void ResetStage()
    {
        enemy0.transform.position = enemy0Start;
        enemy1.transform.position = enemy1Start;
    }
}
