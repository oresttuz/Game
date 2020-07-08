using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Rigidbody2D rb;

    Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = new Vector2(1f, 0f);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Wall")
        {
            movement.x *= -1;
        }
    }
}
