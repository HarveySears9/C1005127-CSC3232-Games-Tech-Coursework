using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandgunBullet : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    public float bulletSpeed = 500f;
    public float maxLifeSpan = 10f;
    public ParticleSystem BulletHit;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Method to project a bullet in a specified direction
    public void Project(Vector2 direction)
    {
        // Add force to the bullet in the specified direction
        rigidbody.AddForce(direction * this.bulletSpeed);

        // Destroy the bullet after the specified lifespan
        Destroy(this.gameObject, this.maxLifeSpan);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the collider in collision
        Collider2D collider = collision.contacts[0].collider;

        string tag = collider.gameObject.tag;

        // Check if the collided GameObject is an enemy or enemy head
        if (tag == "Enemy" || tag == "EnemyHead")
        {
            // Inform the GameManager about the bullet hitting an enemy
            FindObjectOfType<GameManager>().BulletHitEnemy(this);

            // Call the Hit function for the enemy type the particles collided with

            if (collision.gameObject.GetComponent<ZombieController>() != null)
            {
                collision.gameObject.GetComponent<ZombieController>().ZombieHit(tag);
            }
            else if (collision.gameObject.GetComponent<ZombieBat>() != null)
            {
                collision.gameObject.GetComponent<ZombieBat>().ZombieBatHit(tag);
            }
        }

        // Destroy the bullet after the collision
        Destroy(this.gameObject);
    }
}
