using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    public bool PlayerOpened;

    public GameObject Left, Right;

    private void FixedUpdate()
    {
        if (PlayerOpened)
        {
            Left.GetComponent<Animator>().SetBool("Open", true);
            Right.GetComponent<Animator>().SetBool("Open", true);
        }
    }
}
