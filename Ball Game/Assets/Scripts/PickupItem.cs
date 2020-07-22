using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string type;
    public float value;

    public PickupItem(string t, float v)
    {
        type = t;
        value = v;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player p = collision.collider.GetComponent<Player>();

        Debug.Log(p);

        if (p != null)
        {
            Debug.Log("hoiahoishdoi");

            if (type == "ADD_HEALTH")
            {
                p.health++;
                p.HealthBar.AddHealth();
            }
            else
            {
                p.inventory.Add(this);
            }
            Destroy(this.gameObject);
        }
    }
}
