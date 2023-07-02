using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [Header("Gun Parameters")]
    [SerializeField] public string Name = "";
    [SerializeField] private int Damage = 20;
    [SerializeField] private bool AllowButtonHold = false;
    [SerializeField] private float Range = 100;
    [SerializeField] private float ShotsDelay = 0.05f;
    [SerializeField] private float ReloadTime = 1;
    [SerializeField] private int BulletsShotAtOnce = 1;
    [SerializeField] private float TimeBetweenShots = 0;
    [SerializeField] private int MagazineSize = 30;
    [SerializeField] private int StartingEquipmentBullets = 60;
    [SerializeField] private float ShotSpread = 0.02f;
    [SerializeField] private float SpreadWalkingMultiplier = 1.1f;
    [SerializeField] private float SpreadSprintingMultiplier = 1.4f;

    public int bulletsLeft;
    private int bulletsLeftForSingleShot;
    private int bulletsInInventory;
    public bool shooting;
    private bool readyToShoot;
    private bool reloading;
    private bool allowInvoke;

    [Header("References")]
    [SerializeField] private Transform AttackPoint;
    [SerializeField] private GameObject MuzzleFlash;
    [SerializeField] private GameObject BulletHole;
    [SerializeField] private UI uiGun;

    [Header("Sounds")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip reloadClip;

    [Header("AI Collider Values")]
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public float projectileSpeed = 20f;
    [SerializeField] public float projectileLifetime = 5f;

    private RaycastHit rayHit;
    private InputController inputController;
    private PlayerMove playerMove;
    private PlayerItemHandler playerItemHandler;
    private Camera playerCamera;
    private AudioSource GunAudioSource;

    private bool ShouldReload => inputController.GetReloadInput() && bulletsLeft < MagazineSize && !reloading;
    private bool ShouldShoot => readyToShoot && shooting && !reloading && bulletsLeft > 0;

    private int layerMask = ~(1 << 11); //hit everything except player (layer #11)

    void Start()
    {
        inputController = GetComponentInParent<InputController>();
        playerMove = GetComponentInParent<PlayerMove>();
        playerItemHandler = GetComponentInParent<PlayerItemHandler>();
        playerCamera = GetComponentInParent<Camera>();
        GunAudioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        uiGun = GameObject.Find("Canvas").GetComponent<UI>();
        bulletsLeft = MagazineSize;
        readyToShoot = true;
        bulletsInInventory = StartingEquipmentBullets;

        uiGun.UpdateAmmunition(bulletsLeft);
        uiGun.UpdateEquipmentAmmunition(bulletsInInventory);
    }

    void Update()
    {
        HandleShootingInput();
        HandleReload();
        HandleShots();
    }

    private float GetActualSpread()
    {
        float newSpread = ShotSpread;
        Vector3 moveDir = playerMove.GetMoveDirection();

        if (playerMove.IsSprinting)
            newSpread *= SpreadSprintingMultiplier;
        else if (moveDir.x > 0.05f || moveDir.z > 0.05f)
            newSpread *= SpreadWalkingMultiplier;

        return newSpread;
    }

    private void HandleShootingInput()
    {
        if(!playerItemHandler.Interactable)
            shooting = inputController.GetWeaponShotInput(AllowButtonHold);
    }

    private void HandleReload()
    {
        if (ShouldReload)
            Reload();
    }

    public void HandleShots()
    {
        if (ShouldShoot)
        {
            bulletsLeftForSingleShot = BulletsShotAtOnce;
            Shoot();
        }
    }

    public void Reload()
    {
        reloading = true;
        GunAudioSource.PlayOneShot(reloadClip);
        Invoke(nameof(ReloadFinished), ReloadTime);
    }

    private void ReloadFinished()
    {
        int bulletsToLoad = Math.Min(MagazineSize, bulletsInInventory) - bulletsLeft;

        bulletsLeft += bulletsToLoad;
        bulletsInInventory -= bulletsToLoad;
        reloading = false;
        uiGun.UpdateAmmunition(bulletsLeft);
        uiGun.UpdateEquipmentAmmunition(bulletsInInventory);
    }

    public void AddAmmoByItem(int amount)
    {
        bulletsInInventory += amount;
        uiGun.UpdateEquipmentAmmunition(bulletsInInventory);
    }

    private void Shoot()
    {
        float spread = GetActualSpread();
        readyToShoot = false;

        //Spread
        float x = UnityEngine.Random.Range(-spread, spread);
        float y = UnityEngine.Random.Range(-spread, spread);
        float z = UnityEngine.Random.Range(-spread, spread);

        Vector3 shotDirection = playerCamera.transform.forward + new Vector3(x, y, z);

        GunAudioSource.PlayOneShot(shootClip);
        GameObject projectile = Instantiate(projectilePrefab, playerCamera.transform.position, Quaternion.identity);
        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.velocity = shotDirection * projectileSpeed;
        Destroy(projectile, projectileLifetime);

        //Raycast shot
        if (Physics.Raycast(playerCamera.transform.position, shotDirection, out rayHit, Range, layerMask))
        {
            //TO DO: check if enemy was hit and reduce its HP
            ZombieController zombieController = rayHit.transform.GetComponent<ZombieController>();

            if (zombieController != null)
            {
                zombieController.TakeDamage(Damage);
            }

            var bhGraphic = Instantiate(BulletHole, rayHit.point, Quaternion.FromToRotation(Vector3.forward, rayHit.normal), rayHit.transform);
            Destroy(bhGraphic, 0.8f);
        }
        var shotGraphic = Instantiate(MuzzleFlash, AttackPoint.position, Quaternion.identity, AttackPoint);
        Destroy(shotGraphic, 0.2f);


        
        bulletsLeft--;
        bulletsLeftForSingleShot--;

        if (!IsInvoking(nameof(ResetShot)) && !readyToShoot)
        {
            Invoke(nameof(ResetShot), ShotsDelay);
            allowInvoke = false;
        }

        if (bulletsLeftForSingleShot > 0 && bulletsLeft > 0)
        {
            Invoke(nameof(Shoot), TimeBetweenShots);
        }
        uiGun.UpdateAmmunition(bulletsLeft);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }
}
