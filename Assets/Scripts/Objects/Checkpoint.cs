using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Checkpoint Reached");

        if (collision.tag.Equals("Player"))
            PlayerHandler.SaveCheckpoint(transform.position);
    }
}
