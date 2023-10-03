using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementRandom : MonoBehaviour
{

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float speed;
    private Vector3 targetPos;
    
    // Start is called before the first frame update
    void Start()
    {
        MoveObject();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Check if the object has reached the target position
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            // If reached, randomize a new target position
           MoveObject();
        }
    }

    public void MoveObject()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        targetPos = new Vector3(randomX, randomY, transform.position.z);
    }
}
