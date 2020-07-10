using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    public Sprite[] healthSprites;
    public List<HeartObject> health;
    public int totalHealth;
    public int currHealth;

    public HeartObject pfHeart;

    private void Start()
    {
        health = new List<HeartObject>();
        for (int i = 0; i < totalHealth; i++)
        {
            health.Add(Instantiate(pfHeart));
        }
    }

    public void AddHealth(int numHealth)
    {
        if (numHealth < 1)
        {
            return;
        }
        int spriteIndex = 4;
        if (health[health.Count - 1].renderer.sprite.name != healthSprites[4].name)
        {
            spriteIndex = SpriteIndex(health[health.Count - 1].renderer.sprite);
        }
        health[health.Count - 1].renderer.sprite = healthSprites[4];
        for (int i = 1; i < numHealth; i++)
        {
            health.Add(Instantiate(pfHeart));
        }
        totalHealth += numHealth;
        if (spriteIndex != 4 && spriteIndex != -1)
        {
            health[health.Count - 1].renderer.sprite = healthSprites[spriteIndex];
        }
    }

    public void RemoveHealth(int numHealth)
    {
        if (numHealth < 1 || totalHealth < 1)
        {
            return;
        }
        if (numHealth > totalHealth)
        {
            numHealth = totalHealth;
        }
        int spriteIndex = 4;
        if (health[health.Count - 1].renderer.sprite.name != healthSprites[4].name && numHealth != totalHealth)
        {
            spriteIndex = SpriteIndex(health[health.Count - 1].renderer.sprite);
        }
        for (int i = totalHealth - 1; i >= totalHealth - numHealth; i++)
        {
            Destroy(health[i]);
            health.RemoveAt(i);
        }
        totalHealth -= numHealth;
        if (spriteIndex != 4 && spriteIndex != -1 && totalHealth > 0)
        {
            health[health.Count - 1].renderer.sprite = healthSprites[spriteIndex];
        }
    }

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

}
