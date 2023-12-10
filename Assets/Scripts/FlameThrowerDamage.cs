using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerDamage : MonoBehaviour
{
    void OnParticleCollision(GameObject collisionObject)
    {
        if (collisionObject.tag == "Enemy" || collisionObject.tag == "EnemyHead")
        {
            // Call the Hit function for the enemy type the particles collided with

            if(collisionObject.GetComponent<ZombieController>() != null)
            {
                collisionObject.GetComponent<ZombieController>().ZombieHit(tag);
            }
            else if (collisionObject.GetComponent<ZombieBat>() != null)
            {
                collisionObject.GetComponent<ZombieBat>().ZombieBatHit(tag);
            }
        }
    }
}
