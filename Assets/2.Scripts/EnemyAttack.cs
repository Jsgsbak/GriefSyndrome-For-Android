﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
  public  Collider2D collider2D;
    public int Damage = 20;

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        gameObject.tag = "Attack";
        gameObject.layer = 11;
        collider2D.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player1"))
        {
            MountGSS.gameScoreSettings.Player1Hurt.Invoke(Damage);
        }
        else if (collision.gameObject.CompareTag("Player2"))
        {
            MountGSS.gameScoreSettings.Player2Hurt.Invoke(Damage);
        }
        else if (collision.gameObject.CompareTag("Player3"))
        {
            MountGSS.gameScoreSettings.Player3Hurt.Invoke(Damage);
        }


    }

}
