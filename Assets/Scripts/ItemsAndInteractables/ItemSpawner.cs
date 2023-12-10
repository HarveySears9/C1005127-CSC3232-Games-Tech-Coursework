using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private Player player;
    public GameObject handgunAmmoPrefab;
    public GameObject medkitPrefab;
    public GameObject shotgunAmmoPrefab;
    public GameObject flameFuelPrefab;

    // This vaule is lowered from the Game Manager script when the game state progresses
    // This means zombies have a lower chance to drop items as you progress through the game
    public float probOfAllPossibleItems = 0.8f;

    private int bulletsDropped = 10;
    private int shellsDropped = 10;
    private int fuelDropped = 25;

    public void DropItem(Vector3 dropLocation)
    {
        float randomValue = Random.value;
        GameObject droppedItem;

        // Calulates Wieghts for each of the items that can be dropped based on the players status
        // E.g. their current ammo or current health
        float handgunAmmoDropWeight = CalculateHandgunAmmoDropWeight();
        float medkitDropWeight = CalculateMedkitDropWeight();
        float shotgunAmmoDropWeight = CalculateShotgunAmmoDropWeight();
        float flameFuelDropWeight = CalculateFlameFuelDropWeight();

        float totalOfWeights = handgunAmmoDropWeight + medkitDropWeight;

        if(player.hasShotgun)
        {
            totalOfWeights += shotgunAmmoDropWeight;
        }
        if (player.hasFlameThrower)
        {
            totalOfWeights += flameFuelDropWeight;
        }

        float probabilityPerWeight = probOfAllPossibleItems / totalOfWeights;

        float handgunAmmoDropProbability = probabilityPerWeight * handgunAmmoDropWeight;
        float medkitDropProbability = probabilityPerWeight * medkitDropWeight;
        float shotgunAmmoDropProbability = probabilityPerWeight * shotgunAmmoDropWeight;
        float flameFuelDropProbability = probabilityPerWeight * flameFuelDropWeight;

        if (randomValue < handgunAmmoDropProbability)
        {
            // Drop handgun ammo
            droppedItem = Instantiate(handgunAmmoPrefab, dropLocation + new Vector3(0, 0.5f, 0), Quaternion.identity);
            droppedItem.GetComponent<HandgunAmmo>().ammoAmount = bulletsDropped;
        }
        else if (randomValue < handgunAmmoDropProbability + medkitDropProbability)
        {
            // Drop medkit
            droppedItem = Instantiate(medkitPrefab, dropLocation + new Vector3(0, 0.5f, 0), Quaternion.identity);
        }
        else if (player.hasShotgun)
        {
            float probability;
            if (!player.hasFlameThrower)
            {
                probability = handgunAmmoDropProbability + medkitDropProbability + shotgunAmmoDropProbability + flameFuelDropProbability;
            }
            else
            {
                probability = handgunAmmoDropProbability + medkitDropProbability + shotgunAmmoDropProbability;
            }
            if(randomValue < probability)
            {
                // Drop shotgun ammo (only if shotgun has been picked up)
                droppedItem = Instantiate(shotgunAmmoPrefab, dropLocation + new Vector3(0, 0.5f, 0), Quaternion.identity);
                droppedItem.GetComponent<ShotgunAmmo>().ammoAmount = shellsDropped;
            }
        }
        else if (player.hasFlameThrower)
        {
            float probability;
            if (!player.hasShotgun)
            {
                probability = handgunAmmoDropProbability + medkitDropProbability + shotgunAmmoDropProbability + flameFuelDropProbability;
            }
            else
            {
                probability = handgunAmmoDropProbability + medkitDropProbability + flameFuelDropProbability;
            }
            if (randomValue < probability)
            {
                // Drop flame thrower fuel (only if flame thrower has been picked up)
                droppedItem = Instantiate(flameFuelPrefab, dropLocation + new Vector3(0, 0.5f, 0), Quaternion.identity);
                droppedItem.GetComponent<FlameThrowerFuel>().fuelAmount = fuelDropped;
            }
        }
        else
        {
            // No item dropped
            droppedItem = null;
        }
    }

    private float CalculateHandgunAmmoDropWeight()
    {
        int ammoWeight = 1;
        int totalAmmo = player.currentHandgunAmmo + player.handgunAmmoStockPile;

        if(totalAmmo <= 12)
        {
            ammoWeight = 3;
            bulletsDropped = Random.Range(15, 20);
        }
        else if(totalAmmo <= 24)
        {
            ammoWeight = 2;
            bulletsDropped = Random.Range(10, 15);
        } else
        {
            bulletsDropped = Random.Range(5, 10);
        }

        return ammoWeight;
    }

    private float CalculateShotgunAmmoDropWeight()
    {
        float ammoWeight = 0.5f;
        int totalAmmo = player.currentShotgunAmmo + player.shotgunAmmoStockPile;

        if (totalAmmo <= 5)
        {
            ammoWeight = 2f;
            shellsDropped = Random.Range(5, 8);
        }
        else if (totalAmmo <= 10)
        {
            ammoWeight = 1f;
            shellsDropped = Random.Range(4, 5);
        }
        else
        {
            shellsDropped = Random.Range(2, 3);
        }

        return ammoWeight;
    }

    private float CalculateFlameFuelDropWeight()
    {
        float ammoWeight = 0.5f;
        int totalAmmo = (int)player.currentFlame + player.flameStockPile;

        if (totalAmmo <= 50)
        {
            ammoWeight = 2f;
            fuelDropped = Random.Range(50, 75);
        }
        else if (totalAmmo <= 100)
        {
            ammoWeight = 1f;
            fuelDropped = Random.Range(25, 40);
        }
        else
        {
            fuelDropped = Random.Range(15, 25);
        }

            return ammoWeight;
    }

    private float CalculateMedkitDropWeight()
    {
        int medkitWeight = 1;

        if (player.health == 1)
        {
            medkitWeight = 5;
        }
        else if (player.health == 2)
        {
            medkitWeight = 3;
        }

        return medkitWeight;
    }
}

