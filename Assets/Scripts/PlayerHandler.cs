using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerHandler
{
    public static PlayerController Player { get; set; }

    public static int deathCounter { get; private set; } = 0;

    static Vector2 spawnPoint = Vector2.zero;

    public static void SaveCheckpoint(Vector2 checkpoint)
    {
        spawnPoint = checkpoint;
    }

    public static void PlayerDeath()
    {
        if (Player != null)
        {
            Player.transform.position = spawnPoint;
            deathCounter++;
        }
    }

    internal static void Reset()
    {
        deathCounter = 0;
    }
}
