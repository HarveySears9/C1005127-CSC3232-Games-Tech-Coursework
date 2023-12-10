using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ZombieController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;
    public float health = 10;
    public float knockbackForce = 15f;

    // needed for pathfinding
    private Transform target;
    public Transform playerTarget;
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    Path path;
    private int currentWaypoint = 0;
    private bool reachedEnd = false;

    public Transform[] wanderingDesitinations;

    Seeker seeker;

    public bool inactive = false;
    private Collider2D chasingRange;

    public ItemSpawner itemSpawner;

    // State of the zombie
    public enum State { Wandering, Chasing, Downed, Inactive, NoHead, Dead };
    // State for if zombie has its head or not
    public enum HeadState { Head, NoHead }
    State state = State.Wandering;
    HeadState headState = HeadState.Head;

    // Start is called before the first frame update
    void Start()
    {
        chasingRange = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteArray[0];

        seeker = GetComponent<Seeker>();

        target = playerTarget;

        InvokeRepeating("UpdatePath", 0f, 0.5f);

        InvokeRepeating("UpdateWanderingDestination", 0f, Random.Range(5f, 15f));

        if (inactive)
        {
            state = State.Inactive;
            SetupInactiveZombie();
        }
    }

    void FixedUpdate()
    {
        // If state is wandering or chasing the zombie will move around
        if(state == State.Wandering || state == State.Chasing)
        {
            Move();
        }
    }

    // When player enters the trigger collider around the zombie, the zombie will start chasing the player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && state == State.Wandering)
        {
            state = State.Chasing;
            target = playerTarget;
            CancelInvoke("UpdateWanderingDestination");
            Debug.Log("CHASING");
        }
    }

    // Zombie wanders between set points on the map when the player exits the trigger collider
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && state == State.Chasing)
        {
            state = State.Wandering;
            InvokeRepeating("UpdateWanderingDestination", 0f, Random.Range(5f, 15f));
            Debug.Log("Wandering");
        }
    }

    // Zombie deals damage to the player when colliding with the player
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerHead"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null)
            {
                // Call the TakeDamage method on the player
                player.TakeDamage();
                player.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position).normalized * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    // Zombie will move along path
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
            spriteRenderer.flipX = false;
        }
        else if (force.x <= -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    // Sets the wandering destination of the zombie
    void UpdateWanderingDestination()
    {
        if (state == State.Wandering && wanderingDesitinations.Length > 0)
        {
            int randomIndex = Random.Range(0, wanderingDesitinations.Length);
            target = wanderingDesitinations[randomIndex];
        }
    }

    // Called when zombie is hit with a bullet by the player
    public void ZombieHit(string tag)
    {
        Debug.Log(tag);
        if (state == State.Wandering || state == State.Chasing)
        {
            if (tag == "EnemyHead")
            {
                health -= 2;

                // 10% chance to shoot off zombies head if zombie shot in the head
                if (Random.Range(0, 10) <= 1)
                {
                    headState = HeadState.NoHead;
                    spriteRenderer.sprite = spriteArray[1];
                    transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            else if (tag == "Enemy")
            {
                health--;
            }
            else if (tag == "Flame")
            {
                health -= 0.05f;
            }
            if (health <= 0)
            {
                // Zombie can only die if it has no head
                if (headState == HeadState.NoHead || tag == "Flame")
                {
                    ZombieDead();
                }
                else
                {
                    // Else zombie is only temporarily downed
                    StartCoroutine(ZombieDowned());
                }
            }
        }
    }

    IEnumerator ZombieDowned()
    {
        // Sets state to downed so zombie stops moving
        state = State.Downed;
        this.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("EnemyDead");
        spriteRenderer.sprite = spriteArray[2];
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);

        // Waits 10 seconds
        yield return new WaitForSeconds(10);

        // Zombie gets back up again
        state = State.Wandering;
        // Makes sure zombies dont follow player across the level
        if(target == playerTarget)
        {
            InvokeRepeating("UpdateWanderingDestination", 0f, Random.Range(5f, 15f));
        }
        spriteRenderer.sprite = spriteArray[0];
        this.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Enemy");
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        health = 10;
    }

    void ZombieDead()
    {
        // Sets state to dead so zombie stops moving
        state = State.Dead;
        spriteRenderer.sprite = spriteArray[3];
        this.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("EnemyDead");
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
        itemSpawner.DropItem(this.transform.position);

        // Increases amount of kills the player has gotten to use for the results screen
        int killCount = PlayerPrefs.GetInt("zombieKills");
        killCount++;
        PlayerPrefs.SetInt("zombieKills", killCount);
        PlayerPrefs.Save();
    }

    void SetupInactiveZombie()
    {
        this.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("EnemyDead");
        spriteRenderer.sprite = spriteArray[2];
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        chasingRange.enabled = false;
    }

    // Called from the game manager when game state changes as players progress
    public void AwakenZombie()
    {
        StartCoroutine(InactiveZombieSetActive());
    }

    IEnumerator InactiveZombieSetActive()
    {
        yield return new WaitForSeconds(3);

        state = State.Wandering;
        spriteRenderer.sprite = spriteArray[0];
        this.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Enemy");
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);

        chasingRange.enabled = true;
        health = 10;
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
