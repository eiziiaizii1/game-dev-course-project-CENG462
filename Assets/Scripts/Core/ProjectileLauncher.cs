using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;
    
    // Settings
    [SerializeField] private float fireRate; // shots per second
    [SerializeField] private float muzzleFlashDuration; // seconds the flash is visible

    private float previousFireTime;
    private float muzzleFlashTimer;
    private bool shouldFire;

    void Update()
    {
        // 1. Handle Muzzle Flash Timer (Client Side Visuals)
        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) return;
        
        // 2. Fire Rate Logic
        if (!shouldFire) return;
        
        if (Time.time < (1 / fireRate) + previousFireTime) return;

        // 3. Fire Execution
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        previousFireTime = Time.time;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance =
            Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }

        // Inform other clients to spawn dummy projectiles
        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner)
        {
            // The firing client already spawned its own dummy in Update()
            return;
        }
        SpawnDummyProjectile(spawnPos, direction);
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        // 1. visual Logic (Muzzle Flash) is handled HERE so it works for everyone [cite: 33, 37]
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        // 2. Spawn Visual Projectile
        GameObject projectileInstance =
            Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        // 3. Physics Ignore (Client side) [cite: 52]
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            // Fixed: Use 'velocity' 
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool isPressed)
    {
        shouldFire = isPressed;
    }
}