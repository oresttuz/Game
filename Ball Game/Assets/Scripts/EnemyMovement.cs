using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask playerLayers;

    public float moveSpeed = 2f;
    public Rigidbody2D rb;

    public Animator animator;

    Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = new Vector2(1f, 0f);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        animator.SetFloat("Speed", movement.magnitude);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Wall")
        {
            movement.x *= -1;
        }

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayers);

        if (hitPlayers.Length > 0)
        {
            animator.SetBool("Attacking", true);
            foreach (Collider2D player in hitPlayers)
            {
                player.GetComponentInParent<Player>().HealthBar.Damage(1f);
            }
        }
        else
        {
            animator.SetBool("Attacking", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
