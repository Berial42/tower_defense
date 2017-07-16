using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour {

    // Use this for initialization

    // owner of the projectile
    public enum ESide
    {
        EPlayer,
        EEnemy
    }
    public ESide Side;

    private void OnEnable()
    {
        // calls inactive after certain amount of time
        Invoke("DisableProjectile", 2f);
    }

    private void OnDisable()
    {
        // cancels all invoke
        CancelInvoke();
    }

    // disables projectile
    private void DisableProjectile()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // checks ownership of the  projectile and acts accordingly based on the collided object
        if (collision.tag == "Enemy")
        {
            if (Side == ESide.EPlayer)
            {
                collision.GetComponentInParent<EnemyControllerScript>().GotHit();
                DisableProjectile();
            }
        }
        else if (collision.tag == "Player")
        {
            if (Side == ESide.EEnemy)
            {
                collision.GetComponentInParent<PlayerControllerScript>().playerHealth -= 1;
                DisableProjectile();
            }
        }
        else if (collision.tag == "Turret")
        {}
        else
        {
            DisableProjectile();
        }
    }
}
