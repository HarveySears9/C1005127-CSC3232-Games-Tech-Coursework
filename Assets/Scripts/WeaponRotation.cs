using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRotation : MonoBehaviour
{
    Sprite sprite;
    // uses trig to calculate angle the weapon should be set to using the player position and the mouse as points
    public void Update()
    {
        if (!InventoryScreen.isPaused)
        {
            var direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
