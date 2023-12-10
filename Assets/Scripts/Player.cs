using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    public HandgunBullet handgunBulletPrefab;
    public GameObject weapon;
    public GameObject interactBubble;
    private SpriteRenderer spriteRenderer;

    // Sprites for weapons
    public Sprite[] weaponSpriteArray;

    // Sprites for the players health states
    public Sprite[] playerSpriteArray;

    public Image hudWeaponSprite;

    public int health = 3;
    public int maxHealth = 3;

    // Ammo variables
    public int handgunAmmoStockPile = 12;
    public int handgunAmmoCapacity = 12;
    public int currentHandgunAmmo = 12;

    public int shotgunAmmoStockPile = 5;
    public int shotgunAmmoCapacity = 5;
    public int currentShotgunAmmo = 5;

    public int flameStockPile = 50;
    public int flameCapacity = 50;
    public float currentFlame = 50;
    public float fuelConsumptionRate = 10f;
    public ParticleSystem flamethrowerParticles;
    public UnityEngine.Rendering.Universal.Light2D flamethrowerLight;
    public float fadeDuration = 0.25f;
    private bool isFlamethrowerFiring = false;


    // Medkit variables
    public int currentMedkits = 0;
    public int maxMedkits = 5;

    // booleans for if player has inventory items
    public bool hasRedKeycard = false;
    public bool hasBlueKeycard = false;
    public bool hasShotgun = false;
    public bool hasFlameThrower = false;

    private new Rigidbody2D rigidbody;
    public float speed = 10.0f;
    private bool facingRight = true;
    private bool reloading = false;

    private IInteractable interactableInstance;

    // enum that used to handle which weapon the player has equipped
    private enum WeaponEquipped { Handgun, Shotgun, FlameThrower };
    WeaponEquipped weaponEquipped = WeaponEquipped.Handgun;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = playerSpriteArray[0];
    }

    private void Update()
    {
        if (!InventoryScreen.isPaused)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePosition.x > transform.position.x && !facingRight)
            {
                flip();
            }
            else if (mousePosition.x < transform.position.x && facingRight)
            {
                flip();
            }

            // Shoots when pressing left mouse
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!reloading && weaponEquipped != WeaponEquipped.FlameThrower)
                {
                    Shoot();
                }
                else if (!reloading && weaponEquipped == WeaponEquipped.FlameThrower && !isFlamethrowerFiring)
                {
                    // Needed to simulate holding mouse 0 for continious firing of the flamethrower
                    isFlamethrowerFiring = true;
                    StartCoroutine(ShootFlameThrower());
                }
            }

            // Reloads when pressing R
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!reloading)
                {
                    if (weaponEquipped == WeaponEquipped.Handgun && currentHandgunAmmo != handgunAmmoCapacity && handgunAmmoStockPile > 0)
                    {
                        StartCoroutine(Reload());
                    }
                    else if (weaponEquipped == WeaponEquipped.Shotgun && currentShotgunAmmo != shotgunAmmoCapacity && shotgunAmmoStockPile > 0)
                    {
                        StartCoroutine(Reload());
                    }
                    else if (weaponEquipped == WeaponEquipped.FlameThrower && currentFlame != flameCapacity && flameStockPile > 0)
                    {
                        isFlamethrowerFiring = false;
                        StartCoroutine(Reload());
                    }
                }
            }

            // Interact when pressing Space bar
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Interact();
            }

            // Switch weapons when pressing F
            if (Input.GetKeyDown(KeyCode.F))
            {
                if(!reloading)
                {
                    isFlamethrowerFiring = false;
                    SwitchWeapon();
                }
            }
        }
    }

    // called at fixed interval not dependent on framerate
    private void FixedUpdate()
    {
        // Logic for movement using Arrow keys or W, S, A, D
        if (!InventoryScreen.isPaused)
        {
            if (facingRight)
            {
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    rigidbody.AddForce(this.transform.right * this.speed);
                }
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    rigidbody.AddForce(-this.transform.right * this.speed);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    rigidbody.AddForce(-this.transform.right * this.speed);
                }
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    rigidbody.AddForce(this.transform.right * this.speed);
                }
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                rigidbody.AddForce(this.transform.up * this.speed);
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                rigidbody.AddForce(-this.transform.up * this.speed);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ||
                Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ||
                Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                // Lowers drag when moving
                rigidbody.drag = 5;
            }
            else
            {
                // Increases drag when no movement key is pressed
                rigidbody.drag = 25;
            }
        }
    }

    // Flips the player object
    void flip()
    {
        facingRight = !facingRight;
        this.transform.Rotate(0f, 180f, 0f);
        weapon.transform.Rotate(180f, 0f, 0f);
        interactBubble.transform.Rotate(0f, 180f, 0f);
    }

    // Sets hasShotgun to true so player can equip the shotgun
    public void PickupShotgun()
    {
        hasShotgun = true;
        SwitchToShotgun();
        UpdateAmmoCounter();
    }

    // Sets hasFlameThrower to true so player can equip the flame thrower
    public void PickupFlameThrower()
    {
        hasFlameThrower = true;
        SwitchToFlameThrower();
        UpdateAmmoCounter();
    }

    // Toggles between shotgun and handgun and the flamethrower
    void SwitchWeapon()
    {
        if(hasShotgun && hasFlameThrower)
        {
            if(weaponEquipped == WeaponEquipped.Handgun)
            {
                SwitchToShotgun();
            }
            else if(weaponEquipped == WeaponEquipped.Shotgun)
            {
                SwitchToFlameThrower();
            }
            else if(weaponEquipped == WeaponEquipped.FlameThrower)
            {
                SwitchToHandgun();
            }
        }
        else if(hasShotgun)
        {
            if (weaponEquipped == WeaponEquipped.Handgun)
            {
                SwitchToShotgun();
            }
            else if (weaponEquipped == WeaponEquipped.Shotgun)
            {
                SwitchToHandgun();
            }
        }
        else if(hasFlameThrower)
        {
            if (weaponEquipped == WeaponEquipped.Handgun)
            {
                SwitchToFlameThrower();
            }
            else if (weaponEquipped == WeaponEquipped.FlameThrower)
            {
                SwitchToHandgun();
            }
        }

        // Updates ammo counter to display current weapons ammo
        UpdateAmmoCounter();
    }

    // Sets current weapon to handgun
    void SwitchToHandgun()
    {
        weapon.GetComponent<SpriteRenderer>().sprite = weaponSpriteArray[0];

        hudWeaponSprite.sprite = weaponSpriteArray[0];

        weaponEquipped = WeaponEquipped.Handgun;
    }

    // Sets current weapon to shotgun
    void SwitchToShotgun()
    {
        weapon.GetComponent<SpriteRenderer>().sprite = weaponSpriteArray[1];

        hudWeaponSprite.sprite = weaponSpriteArray[1];

        weaponEquipped = WeaponEquipped.Shotgun;
    }

    // Sets current weapon to flamethrower
    void SwitchToFlameThrower()
    {
        weapon.GetComponent<SpriteRenderer>().sprite = weaponSpriteArray[2];

        hudWeaponSprite.sprite = weaponSpriteArray[2];

        weaponEquipped = WeaponEquipped.FlameThrower;
    }

    void Shoot()
    {
        if (weaponEquipped == WeaponEquipped.Handgun)
        {
            if (currentHandgunAmmo > 0)
            {
                // Shoots one bullet for the handgun by instantiating the bullet prefab
                HandgunBullet bullet = Instantiate(this.handgunBulletPrefab, weapon.transform.position + new Vector3(0f, 0.075f, 0f), weapon.transform.rotation);
                bullet.Project(weapon.transform.right);

                currentHandgunAmmo--;

                UpdateAmmoCounter();
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
        else if (weaponEquipped == WeaponEquipped.Shotgun)
        {
            if (currentShotgunAmmo > 0)
            {
                // Shoots 5 pellets for the shotgun by instantiating the bullet prefab
                for (int i = 0; i < 5; i++)
                {
                    float pelletAngle = Random.Range(-15f, 15f);
                    HandgunBullet bullet = Instantiate(this.handgunBulletPrefab, weapon.transform.position + new Vector3(0f, 0.075f, 0f), weapon.transform.rotation * Quaternion.Euler(0f, 0f, pelletAngle));

                    Vector3 spreadDirection = Quaternion.Euler(0f, 0f, pelletAngle) * weapon.transform.right;

                    bullet.Project(spreadDirection);
                }

                currentShotgunAmmo--;

                UpdateAmmoCounter();
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
    }

    IEnumerator ShootFlameThrower()
    {
        while (isFlamethrowerFiring && currentFlame > 0)
        {
            // If the particle effect is not playing then it plays
            // This means it will only be played once
            // The particle system is set to loop so will play until code stops it
            if (!flamethrowerParticles.isPlaying)
            {
                flamethrowerParticles.Play();
                StartCoroutine(FadeLight(true));
            }

            // Decrease fuel over time
            currentFlame -= fuelConsumptionRate * Time.deltaTime;
            currentFlame = Mathf.Clamp(currentFlame, 0f, float.MaxValue);



            UpdateAmmoCounter();

            // When Mouse0 is released the while loop will end
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                isFlamethrowerFiring = false;
            }

            yield return null; // Wait for the next frame
        }

        // Stop flamethrower particles
        StartCoroutine(FadeLight(false));
        flamethrowerParticles.Stop();

        // If fuel is depleted, initiate reload
        if (currentFlame <= 0)
        {
            StartCoroutine(Reload());
        }

        // Set the firing flag to false
        // Stops continued firing when reloading occurs to break the while loop
        isFlamethrowerFiring = false;
    }


    IEnumerator FadeLight(bool fadeIn)
    {
        float elapsedTime = 0f;
        float startIntensity = flamethrowerLight.intensity;
        float targetIntensity;
        // Set the target intensity
        if (fadeIn)
        { 
            targetIntensity = 2.0f; 
        }
        else
        {
            targetIntensity = 0f;
        }

        while (elapsedTime < fadeDuration)
        {
            // Calculate the new intensity between start and target intensity
            flamethrowerLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / fadeDuration);

            // Increment time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // set intensity to target intesity
        flamethrowerLight.intensity = targetIntensity;
    }

    // Reloads gun after a time delay
    IEnumerator Reload()
    {
        ammoText.text = "RELOADING...";
        reloading = true;
        speed = speed / 2.0f;

        yield return new WaitForSeconds(3);

        if(weaponEquipped == WeaponEquipped.Handgun)
        {
            if (handgunAmmoStockPile >= handgunAmmoCapacity)
            {
                int temp = handgunAmmoCapacity - currentHandgunAmmo;
                handgunAmmoStockPile -= temp;

                currentHandgunAmmo = handgunAmmoCapacity;
            }
            else
            {
                currentHandgunAmmo = handgunAmmoStockPile;
                handgunAmmoStockPile = 0;
            }
        }
        if (weaponEquipped == WeaponEquipped.Shotgun)
        {
            if (shotgunAmmoStockPile >= shotgunAmmoCapacity)
            {
                int temp = shotgunAmmoCapacity - currentShotgunAmmo;
                shotgunAmmoStockPile -= temp;

                currentShotgunAmmo = shotgunAmmoCapacity;
            }
            else
            {
                currentShotgunAmmo = shotgunAmmoStockPile;
                shotgunAmmoStockPile = 0;
            }
        }
        if (weaponEquipped == WeaponEquipped.FlameThrower)
        {
            if (flameStockPile >= flameCapacity)
            {
                float temp = flameCapacity - currentFlame;
                flameStockPile -= (int)temp;

                currentFlame = flameCapacity;
            }
            else
            {
                currentFlame = flameStockPile;
                flameStockPile = 0;
            }
        }

        UpdateAmmoCounter();

        speed = speed * 2.0f;
        reloading = false; 
    }

    // Updates text to display the current weapons ammo in the gun and stockpile
    public void UpdateAmmoCounter()
    {
        if(weaponEquipped == WeaponEquipped.Handgun)
        {
            ammoText.text = currentHandgunAmmo + "/" + handgunAmmoStockPile;
        }else if(weaponEquipped == WeaponEquipped.Shotgun)
        {
            ammoText.text = currentShotgunAmmo + "/" + shotgunAmmoStockPile;
        }
        else if (weaponEquipped == WeaponEquipped.FlameThrower)
        {
            float temp = Mathf.RoundToInt(currentFlame);
            ammoText.text = temp + "/" + flameStockPile;
        }
    }

    // interacts with interactable object
    void Interact()
    {
        if(interactableInstance != null)
        {
            interactableInstance.InteractLogic();
        }
    }

    public void SetIInstance(IInteractable interactable)
    {
        interactableInstance = interactable;
    }
    
    public void ClearIInstance()
    {
        interactableInstance = null;
    }

    // Decrements the amount of medkits the player has and increases health by 1
    public void UseMedkit()
    {
        if(currentMedkits > 0 && health < maxHealth)
        {
            currentMedkits--;
            health++;
        }

        UpdateSprite();
    }

    // Lowers health of the player by 1 and checks to see if player has died
    public void TakeDamage()
    {
        health--;

        if(health < 1)
        {
            StartCoroutine(PlayerDeath());
        }

        UpdateSprite();
    }

    // Updates the sprite based on the players health
    private void UpdateSprite()
    {
        if(health == 3)
        {
            spriteRenderer.sprite = playerSpriteArray[0];
        } else if (health == 2)
        {
            spriteRenderer.sprite = playerSpriteArray[1];
        } else if(health == 1)
        {
            spriteRenderer.sprite = playerSpriteArray[2];
        }
    }

    // Changes the rotation of the player
    // Disables player controls by pausing the game
    // Waits for 5 seconds then calls GameManager.Lose()
    IEnumerator PlayerDeath()
    {
        this.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        rigidbody.drag = 25;
        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<Collider2D>().enabled = false;
        InventoryScreen.isPaused = true;
        yield return new WaitForSeconds(5f);
        InventoryScreen.isPaused = false;
        FindObjectOfType<GameManager>().Lose();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Win"))
        {
            // Calls GameManager.Win() when player hits the "Win" tagged collider
            FindObjectOfType<GameManager>().Win();
        }
        if (collision.CompareTag("Item"))
        {
            // Enables interact bubble when player collides with an item
            interactBubble.SetActive(true);
        }
    }

    // Disables interact bubble when player is not near an interactable object
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            interactBubble.SetActive(false);
        }
    }
}
