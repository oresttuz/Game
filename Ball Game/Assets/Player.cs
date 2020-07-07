using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator PlayerAnimator;

    public int health;
    public bool isAlive, takingDmg;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    // Start is called before the first frame update
    void Start()
    {
        health = 5;
        isAlive = true;
        takingDmg = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack();
            PlayerAnimator.SetBool("Attack", true);
        }
        else
        {
            PlayerAnimator.SetBool("Attack", false);
        }
        
    }

    void Attack()
    {

        Collider2D[] hitEnemies =  Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hah, Gotem: " + enemy.tag);
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

    private void FixedUpdate()
    {
        if (isAlive)
        {
            if (takingDmg)
            {
                health--;
            }
            if (health <= 0)
            {
                isAlive = false;
                Debug.Log("Goodbye Cruel World");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Here");
        if (collision.collider.tag == "Enemy")
        {
            //Debug.Log("Is an enemy");
            takingDmg = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log("Bye");
        if (collision.collider.tag == "Enemy")
        {
            //Debug.Log("Mr.Enemy");
            takingDmg = false;
        }
    }

}
