using UnityEngine;
using Unity.Netcode;
 
public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private Transform turretTransform; // rotating pivot
    [SerializeField] private InputReader inputReader;   // provides AimPosition
    [SerializeField] private float turretRotationSpeed = 15f;
 
    private void LateUpdate()
    {
        if (!IsOwner) return; // Only owner aims locally
 
        // 1) Screen-space cursor
        Vector2 aimScreen = inputReader.AimPosition;
 
        // 2) Screen → world (for orthographic 2D, z=0 is fine)
        Vector3 aimWorld3 = Camera.main.ScreenToWorldPoint(
            new Vector3(aimScreen.x, aimScreen.y, 0f)
        );
        Vector2 aimWorld = (Vector2)aimWorld3;
 
        // 3) Direction from turret to cursor
        Vector2 turretPos = (Vector2)turretTransform.position;

        // Without normalizing, if not used lerp normalizing isn't required
        //Vector2 dir = aimWorld - turretPos; 
        
        // Via lerp Optional smoothing

        Vector2 dir = (aimWorld - turretPos).normalized;
        float dampingSpeed = turretRotationSpeed;
        // The magical frame-rate independent damping formula
        float t = 1f - Mathf.Exp(-dampingSpeed * Time.deltaTime);
        Vector2 smoothedDir = Vector2.Lerp(turretTransform.up, dir, t);
 
        // 4) Rotate turret to face cursor
        //turretTransform.up = dir; // use .right if your art faces +X
        turretTransform.up = smoothedDir;
    }
}