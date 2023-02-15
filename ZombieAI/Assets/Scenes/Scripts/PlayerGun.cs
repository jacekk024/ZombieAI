using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [Header("Gun Parameters")]
    [SerializeField] private int Damage = 20;
    [SerializeField] private bool AllowButtonHold = false;
    [SerializeField] private float Range = 100;
    [SerializeField] private float ShotsDelay = 0.05f;
    [SerializeField] private float ReloadTime = 1;
    [SerializeField] private int BulletsShotAtOnce = 1;
    [SerializeField] private float TimeBetweenShots = 0;
    [SerializeField] private int MagazineSize = 30;
    [SerializeField] private float ShotSpread = 0.02f;
    [SerializeField] private float SpreadWalkingMultiplier = 1.1f;
    [SerializeField] private float SpreadSprintingMultiplier = 1.4f;

    private int bulletsLeft;
    private int bulletsLeftForSingleShot;
    private bool shooting;
    private bool readyToShoot;
    private bool reloading;
    private bool allowInvoke;

    [Header("Controls")]
    [SerializeField] private KeyCode ShootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode ReloadKey = KeyCode.R;

    [Header("References")]
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private Transform AttackPoint;
    [SerializeField] private PlayerMove PlayerMove;
    [SerializeField] private GameObject MuzzleFlash;
    [SerializeField] private GameObject BulletHole;
    [SerializeField] private PauseMenu PauseMenu;
    [SerializeField] private UI uiGun;


    private RaycastHit rayHit;
    private bool ShouldReload => !PauseMenu.GamePaused && Input.GetKeyDown(ReloadKey) && bulletsLeft < MagazineSize && !reloading;
    private bool ShouldShoot => readyToShoot && shooting && !reloading && bulletsLeft > 0;

    private int layerMask = ~(1 << 11); //hit everything except player (layer #11)

    void Start()
    {
        uiGun = GameObject.Find("Canvas").GetComponent<UI>();
    }

    private void Awake()
    {
        bulletsLeft = MagazineSize;
        readyToShoot = true;
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
        Vector3 moveDir = PlayerMove.GetMoveDirection();

        if (PlayerMove.IsSprinting)
            newSpread *= SpreadSprintingMultiplier;
        else if (moveDir.x > 0.05f || moveDir.z > 0.05f)
            newSpread *= SpreadWalkingMultiplier;

        return newSpread;
    }

    private void HandleShootingInput()
    {
        if (PauseMenu.GamePaused)
            return;

        if (AllowButtonHold)
            shooting = Input.GetKey(ShootKey);
        else
            shooting = Input.GetKeyDown(ShootKey);
    }

    private void HandleReload()
    {
        if (ShouldReload)
            Reload();
    }

    private void HandleShots()
    {
        if (ShouldShoot)
        {
            bulletsLeftForSingleShot = BulletsShotAtOnce;
            Shoot();
        }
    }

    private void Reload()
    {
        reloading = true;
        Invoke(nameof(ReloadFinished), ReloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = MagazineSize;
        reloading = false;
        uiGun.UpdateAmmunition(bulletsLeft);
    }

    private void Shoot()
    {
        float spread = GetActualSpread();
        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);

        Vector3 shotDirection = PlayerCamera.transform.forward + new Vector3(x, y, z);

        //Raycast shot
        if (Physics.Raycast(PlayerCamera.transform.position, shotDirection, out rayHit, Range, layerMask))
        {
            //TO DO: check if enemy was hit and reduce its HP
            ZombieController zombieController = rayHit.transform.GetComponent<ZombieController>();

            if(zombieController != null)
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
        uiGun.UpdateAmmunition(bulletsLeft);

        if (!IsInvoking(nameof(ResetShot)) && !readyToShoot)
        {
            Invoke(nameof(ResetShot), ShotsDelay);
            allowInvoke = false;
        }

        if (bulletsLeftForSingleShot > 0 && bulletsLeft > 0)
            Invoke(nameof(Shoot), TimeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }



}
