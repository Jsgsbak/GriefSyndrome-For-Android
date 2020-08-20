using System.Collections;
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
        collider2D.isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player1"))
        {
           StageCtrl.Player1Hurt.Invoke(Damage);
        }
        else if (collision.gameObject.CompareTag("Player2"))
        {
            StageCtrl.Player2Hurt.Invoke(Damage);
        }
        else if (collision.gameObject.CompareTag("Player3"))
        {
            StageCtrl.Player3Hurt.Invoke(Damage);
        }

    }
}
