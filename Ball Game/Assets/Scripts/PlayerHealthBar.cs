using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    public Sprite[] healthSprites;
    public List<HeartObject> health;

    public Transform HealthBarTransform;

    public int totalHealth;
    public float currHealth;

    private Vector3 endPosition;
    public float padding;

    public HeartObject pfHeart;

    private void Start()
    {
        endPosition = new Vector3(35f, -30f, 0f);

        health = new List<HeartObject>();
        for (int i = 0; i < totalHealth; i++)
        {
            health.Add(MakeHeart());
        }

        currHealth = (totalHealth * 1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Heal(1.25f);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Damage(1.5f);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddHealth(1);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            RemoveHealth(1);
        }
    }

    public void AddHealth(int numHealth)
    {
        if (numHealth < 1)
        {
            return;
        }
        for (int i = 0; i < numHealth; i++)
        {
            health.Add(MakeHeart());
        }
        totalHealth += numHealth;
    }

    public void RemoveHealth(int numHealth)
    {
        if (numHealth < 1 || totalHealth < 0)
        {
            return;
        }
        if (numHealth > totalHealth)
        {
            numHealth = totalHealth;
        }
        float totalPaddding = 0f;
        for (int i = 0; i < numHealth; i++)
        {
            Destroy(health[i]);
            health.RemoveAt(i);
            totalPaddding -= padding;
        }
        totalHealth -= numHealth;
        endPosition.x += totalPaddding;
        foreach (HeartObject h in health)
        { 
            h.transform.localPosition = new Vector3(h.transform.localPosition.x + totalPaddding, endPosition.y, endPosition.z);
        }
    }

    public void Heal(float amount)
    {
        if (amount < 0.25f)
        {
            return;
        }
        if (currHealth == (totalHealth * 1.0f))
        {
            return;
        }
        float newHealth = currHealth + amount;
        if (newHealth > totalHealth)
        {
            newHealth = totalHealth;
            amount = totalHealth - currHealth;
        }
        int flooredNewHealth = Mathf.FloorToInt(newHealth);
        for (int i = Mathf.FloorToInt(currHealth); i <= flooredNewHealth; i++)
        {
            if (amount < 1f && amount > 0f)
            {
                int newSpriteIndex = health[i].SpriteIndex + (int)(amount * 4);
                if (newSpriteIndex > 4)
                {
                    newSpriteIndex = 4;
                    amount -= ((4 - health[i].SpriteIndex) / 4f);
                }
                else
                {
                    amount = 0f;
                }
                health[i].SpriteIndex = newSpriteIndex;
                health[i].renderer.sprite = healthSprites[newSpriteIndex];
            }
            else
            {
                amount -= (1f - (health[i].SpriteIndex / 4f));
                health[i].SpriteIndex = 4;
                health[i].renderer.sprite = healthSprites[4];
            }
        }
        currHealth = newHealth;
    }
    public void Damage(float amount)
    {
        Debug.Log(currHealth);
        Debug.Log(amount);
        Debug.Log(health.Count);
        if (amount < 0.25f) 
        {
            return;
        }
        if (currHealth == 0f) 
        {
            return;
        }
        float newHealth = currHealth - amount; 
        if (newHealth < 0.25f) 
        {
            newHealth = 0f;
            amount = currHealth;
        }
        int flooredNewHealth = Mathf.FloorToInt(newHealth);
        for (int i = Mathf.FloorToInt(currHealth) - 1; i >= flooredNewHealth; i--) 
        {
            if (amount < 1f && amount > 0f) 
            {
                int newSpriteIndex = health[i].SpriteIndex - (int)(amount * 4);
                if (newSpriteIndex < 0)
                {
                    newSpriteIndex = 0;
                    amount -= (health[i].SpriteIndex / 4f);
                }
                else
                {
                    amount = 0f;
                }
                health[i].SpriteIndex = newSpriteIndex;
                health[i].renderer.sprite = healthSprites[newSpriteIndex];
            }
            else 
            {
                amount -= (health[i].SpriteIndex / 4f);
                health[i].SpriteIndex = 0;
                health[i].renderer.sprite = healthSprites[0];
            }
        }
        currHealth = newHealth;
    }

    /*
    public int SpriteIndex(Sprite s)
    {
        for (int currIndex = 0; currIndex < healthSprites.Length; currIndex++)
        {
            if (healthSprites[currIndex].name == s.name)
            {
                return currIndex;
            }
        }
        return -1;
    }
    */

    public HeartObject MakeHeart()
    {
        HeartObject cloneHeartObject = Instantiate(pfHeart, new Vector3(0f, 0f, 0f), Quaternion.identity, HealthBarTransform);
        cloneHeartObject.transform.localPosition = endPosition;
        endPosition.x += (padding);
        return cloneHeartObject;
    }
}
