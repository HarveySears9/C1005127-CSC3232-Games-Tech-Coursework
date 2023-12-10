using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ZombieBat : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;
    public Transform player;
    public float health = 5;
    public BatProjectile BatProjectilePrefab;

    // needed for pathfinding
    private Transform target;
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    Path path;
    private int currentWaypoint = 0;
    private bool reachedEnd = false;

    public Transform[] wanderingDesitinations;

    Seeker seeker;

    public ItemSpawner itemSpawner;

    public enum State { Moving, Shooting, Dead };
    State state = State.Moving;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        seeker = GetComponent<Seeker>();

        InvokeRepeating("UpdateWanderingDestination", 0f, Random.Range(5f, 15f));
        InvokeRepeating("UpdateSprite", 0f, 0.25f);
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case State.Moving:
                Move();
                break;
        }
    }

    // Stops movement and starts shooting at the player when entering a certain range
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            state = State.Shooting;
            InvokeRepeating("Shoot", 3f, Random.Range(3f, 5f));
            Debug.Log("Shooting");
        }
    }

    // Stops shooting and continues wandering between set destination
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            state = State.Moving;
            CancelInvoke("Shoot");
            Debug.Log("Stopped Shooting");
        }
    }

    void Shoot()
    {
        // Find player location
        var direction = player.position - this.transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Convert angle to Vector2 direction
        Vector2 shootDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        // Instantiate the projectile with the calculated direction
        BatProjectile goo = Instantiate(this.BatProjectilePrefab, this.transform.position + new Vector3(0f, 0.075f, 0f), Quaternion.Euler(0f, 0f, angle+180f));

        goo.Project(shootDirection);
    }

    // Damages Zombie bat
    public void ZombieBatHit(string tag)
    {
        // Less damage delt by flamethrower
        if(tag == "Flame")
        {
            health -= 0.01f;
        }
        else
        {
            health--;
        }

        // Checks if zombie bat is dead
        if (health <= 0)
        {
            ZombieBatDead();
        }
    }

    // Handles enemy death
    void ZombieBatDead()
    {
        spriteRenderer.sortingLayerID = SortingLayer.NameToID("EnemyDead");
        itemSpawner.DropItem(this.transform.position);

        // Disables colliders
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        // Sets state to dead to stop shooting and moving
        state = State.Dead;

        // Cancel InvokeRepeatings
        CancelInvoke("Shoot");
        CancelInvoke("UpdateSprite");

        // Flip Sprite upsidedown
        spriteRenderer.flipY = true;

        // Start Death animation
        StartCoroutine("DeathAnimation");

        // Increases amount of kills the player has gotten to use for the results screen
        int killCount = PlayerPrefs.GetInt("zombieKills");
        killCount++;
        PlayerPrefs.SetInt("zombieKills", killCount);
        PlayerPrefs.Save();
    }

    // Death animation that flings enemy in the air and sets gravity scale to bring them back down
    IEnumerator DeathAnimation()
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;
        rigidbody.gravityScale = 1f;
        rigidbody.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
        rigidbody.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
        rigidbody.AddTorque(10f, ForceMode2D.Impulse);
        rigidbody.drag = 1f;
        yield return new WaitForSeconds(1f);
        rigidbody.drag = 15f;
        rigidbody.gravityScale = 0f;
    }

    // toggles between two sprites to have a wing flapping animation
    void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            if(spriteRenderer.sprite == spriteArray[0])
            {
                spriteRenderer.sprite = spriteArray[1];
            }
            else { spriteRenderer.sprite = spriteArray[0]; }
        }
    }

    // Sets the wandering destination
    void UpdateWanderingDestination()
    {
        if (state == State.Moving && wanderingDesitinations.Length > 0)
        {
            int randomIndex = Random.Range(0, wanderingDesitinations.Length);
            target = wanderingDesitinations[randomIndex];
        }
    }

    // Moves along a path
    void Move()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEnd = true;
            return;
        }
        else
        {
            reachedEnd = false;
        }


        // Calculates and applies force in the direction of the current waypoint
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)GetComponent<Rigidbody2D>().position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        GetComponent<Rigidbody2D>().AddForce(force);

        float distance = Vector2.Distance(GetComponent<Rigidbody2D>().position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Keeps the enemy facing forwards
        if (force.x >= 0.01f)
        {
            spriteRenderer.flipX = true;
        }
        else if (force.x <= -0.01f)
        {
            spriteRenderer.flipX = false;
        }
    }
    
    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(GetComponent<Rigidbody2D>().position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
