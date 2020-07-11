using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartObject : MonoBehaviour
{
    public SpriteRenderer renderer;
    public RectTransform rectTransform;

    public int SpriteIndex;

    public HeartObject(SpriteRenderer sr, RectTransform rt, int si)
    {
        renderer = sr;
        rectTransform = rt;
        SpriteIndex = si;
    }
}
