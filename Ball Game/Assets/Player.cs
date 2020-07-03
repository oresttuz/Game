using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health;
    public bool isAlive, takingDmg;

    // Start is called before the first frame update
    void Start()
    {
        health = 5;
        isAlive = true;
        takingDmg = false;
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
        Debug.Log("Here");
        if (collision.collider.tag == "Enemy")
        {
            Debug.Log("Is an enemy");
            takingDmg = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Bye");
        if (collision.collider.tag == "Enemy")
        {
            Debug.Log("Mr.Enemy");
            takingDmg = false;
        }
    }

}
