using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float forceAmount = 0;

    private Rigidbody2D rb2D;
    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb2D.AddForce(Vector2.up * forceAmount);
        }

        if (Input.GetKey(KeyCode.S))
        {
            rb2D.AddForce(Vector2.down * forceAmount);
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb2D.AddForce(Vector2.left * forceAmount);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb2D.AddForce(Vector2.right * forceAmount);
        }

        rb2D.velocity *= .09f;
    }
}
