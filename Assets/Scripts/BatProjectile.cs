using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatProjectile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;
    private new Rigidbody2D rigidbody;
    public float projectileSpeed = 205f;
    public float maxLifeSpan = 10f;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        InvokeRepeating("AnimateGoo", 0f, 0.1f);
    }

    // cycles through sprites of projectile for an animation
    void AnimateGoo()
    {
        if (spriteRenderer != null)
        {
            if(spriteRenderer.sprite == spriteArray[0])
            {
                spriteRenderer.sprite = spriteArray[1];
            } else if(spriteRenderer.sprite == spriteArray[1])
            {
                spriteRenderer.sprite = spriteArray[2];
            } else { spriteRenderer.sprite = spriteArray[0]; }
        }
    }

    // Method to project a projectile in a specified direction
    public void Project(Vector2 direction)
    {
        // Add force to the projectile  in the specified direction
        rigidbody.AddForce(direction * this.projectileSpeed);

        // Destroy the projectile  after the specified lifespan
        Destroy(this.gameObject, this.maxLifeSpan);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the collider in collision
        Collider2D collider = collision.contacts[0].collider;

        string tag = collider.gameObject.tag;

        // Check if the collided GameObject is the player or  the players head
        if (tag == "Player" || tag == "PlayerHead")
        {
            // Deals damage to the player
            collision.gameObject.GetComponent<Player>().TakeDamage();
        }

        // Destroy the projectile after the collision
        Destroy(this.gameObject);
    }
}
