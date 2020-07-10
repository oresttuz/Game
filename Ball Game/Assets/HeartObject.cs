using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartObject : MonoBehaviour
{
    public SpriteRenderer renderer;
    public GameObject heartObject;

    public HeartObject(SpriteRenderer sr, GameObject ho)
    {
        renderer = sr;
        heartObject = ho;
    }
}
