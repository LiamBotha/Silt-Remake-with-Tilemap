using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Level End Reached");

        if (collision.tag.Equals("Player"))
            GameObject.FindObjectOfType<UIManager>().EndGame();
    }
}
