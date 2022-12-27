using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] GameObject start, end;
    [SerializeField] List<GameObject> obstacles;
    [SerializeField] GameObject agent;

    List<GameObject> entities;

    [SerializeField] float minBound = -9f, maxBound = 9f;
    // Start is called before the first frame update
    void Start()
    {
        entities = new List<GameObject>();
        //setup();
    }

    public void setup()
    {
        cleanUp();
        GameObject currStart = Instantiate(start, transform, false);
        currStart.transform.localPosition = new Vector3(Random.Range(minBound, maxBound), transform.localPosition.y, Random.Range(minBound, maxBound));
        GameObject currEnd = Instantiate(end, transform, false);
        currEnd.transform.localPosition = new Vector3(Random.Range(minBound, maxBound), transform.localPosition.y, Random.Range(minBound, maxBound));
        entities.Add(currStart);
        entities.Add(currEnd);
        currStart.SetActive(true);
        currEnd.SetActive(true);
        while (currEnd.GetComponent<Collider>().bounds.Intersects(currStart.GetComponent<Collider>().bounds))
        {
            entities.Remove(currEnd);
            Destroy(currEnd);
            currEnd = Instantiate(end, transform, false);
            entities.Add(currEnd);
            currEnd.transform.localPosition = new Vector3(Random.Range(minBound, maxBound), transform.localPosition.y, Random.Range(minBound, maxBound));
        }
        foreach (GameObject obstacle in obstacles)
        {
            GameObject o = Instantiate(obstacle, transform, false);
            entities.Add(o);
            o.transform.localPosition = new Vector3(Random.Range(minBound, maxBound), transform.localPosition.y, Random.Range(minBound, maxBound));
            o.transform.rotation = Quaternion.Euler(0, Random.Range(-1f, 1f) * 90, 0);
            while (o.GetComponentInChildren<Collider>().bounds.Intersects(currStart.GetComponent<Collider>().bounds) || o.GetComponentInChildren<Collider>().bounds.Intersects(currEnd.GetComponent<Collider>().bounds))
            {
                entities.Remove(o);
                Destroy(o);
                o = Instantiate(obstacle, transform, false);
                entities.Add(o);
                o.transform.localPosition = new Vector3(Random.Range(minBound, maxBound), transform.localPosition.y, Random.Range(minBound, maxBound));
                o.transform.rotation = Quaternion.Euler(0, Random.Range(-1f, 1f) * 90, 0);
            }
        }
        agent.transform.position = currStart.transform.position + new Vector3(0, 0.5f, 0);
        agent.transform.localRotation = Quaternion.identity;
    }

    private void cleanUp()
    {
        foreach(GameObject g in entities)
        {
            Destroy(g);
        }
        entities = new List<GameObject>();
    }
}
