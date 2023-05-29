using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    [Header("Functional Options")]
    [SerializeField] private bool CanMove = true;
    [SerializeField] private bool CanSprint = true;
    [SerializeField] private bool CanJump = true;
    [SerializeField] private bool CanCrouch = true;
    [SerializeField] private bool SlopesSliding = true;
    [SerializeField] private bool useStamina = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement Parameters")]
    [SerializeField] private float WalkSpeed = 8f;
    [SerializeField] private float SprintSpeed = 18f;
    [SerializeField] private float CrouchSpeed = 4f;
    [SerializeField] private float SlopeSlideSpeed = 10f;

    [Header("Health Parameters")]
    [SerializeField] internal float maxHealth = 100;
    [SerializeField] private float timeBeforeRegenStarts = 3;
    [SerializeField] private float healthValueIncrement = 1;
    [SerializeField] private float healthTimeIncrement = 0.1f;

    internal float currentHealth;
    private Coroutine regeneratingHealth;
    public static Action<float> OnTakeDamage;
    public static Action<float, float> OnDamage;
    public static Action<float, float> OnHeal;

    [Header("Stamina Parameters")]
    [SerializeField] private float maxStamina = 100;
    [SerializeField] private float staminaUseMultiplier = 5;
    [SerializeField] private float timeBeforeStaminaRegenStarts = 5;
    [SerializeField] private float staminaValueIncrement = 2;
    [SerializeField] private float staminaTimeIncrement = 0.1f;
    [SerializeField] private Image staminaUI = default;
    [SerializeField] private Image staminaClawUI = default;
    private float currentStamina;
    private Coroutine regeneratingStamina;

    public static Action<float, float> OnStaminaChange;


    [Header("Jumping Parameters")]
    [SerializeField] private float Gravity = 9.81f;
    [SerializeField] private float JumpForce = 2.5f;

    [Header("Crouching Parameters")]
    [SerializeField] private float CrouchHeight = 0.4f;
    [SerializeField] private float StandingHeight = 2f;
    [SerializeField] private float TimeToCrouch = 0.2f;
    [SerializeField] private Vector3 CrouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 StandingCenter = new Vector3(0, 0, 0);

    [Header("References")]
    [SerializeField] private PauseMenu PauseMenu;

    [Header("AI Movement")]
    [SerializeField] PlayerAIMovement[] playerPositions;
    [SerializeField] private bool AutoMove = false;
    [SerializeField] private bool AutoShoot = false;
    [SerializeField] private float FieldOfView = 45f;
    [SerializeField] private Transform CurrentTarget;
    [SerializeField] internal PlayerGun PlayerGunComponent;

    public List<Transform> targets = new List<Transform>();

    [Serializable]
    public class PlayerAIMovement
    {
        public float x;
        public float z;
    }
    private int AICounter = 0;

    private float horizontalAxis;
    private float verticalAxis;

    private Vector3 moveDirection;
    private CharacterController characterController;
    private Camera playerCamera;
    private bool ShouldJump => !PauseMenu.GamePaused && Input.GetKey(jumpKey) && characterController.isGrounded && !isCrouching;
    private bool ShouldCrouch => !PauseMenu.GamePaused && Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;
    private bool isCrouching;
    private bool duringCrouchAnimation;

    private Vector3 hitPointNormal;
    public GameOverScript GameOverScript;

    private bool IsSliding
    {
        get
        {
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 1f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    private void OnEnable()
    {
        OnTakeDamage += ApplyDamage;
    }
    private void OnDisable()
    {
        OnTakeDamage -= ApplyDamage;   
    }

    public bool IsGrounded()
    {
        return characterController.isGrounded;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool IsSprinting => CanSprint && Input.GetKey(sprintKey) && verticalAxis > 0;
    public bool IsCrouching
    {
        get
        {
            return isCrouching;
        }
        private set
        {
            isCrouching = value;
        }
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        PlayerGunComponent = GameObject.FindGameObjectWithTag("Gun").GetComponent<PlayerGun>();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    private void UpdateAxises()
    {
        if (PauseMenu.GamePaused)
            return;

        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
    }

    private void HandleMovementInput()
    {
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * verticalAxis) + (transform.TransformDirection(Vector3.right) * horizontalAxis);
        moveDirection = Vector3.Normalize(moveDirection);
        moveDirection.y = moveDirectionY;
    }

    private void HandleJump()
    {
        if (ShouldJump)
            moveDirection.y = JumpForce;
    }

    private void HandleCrouch()
    {
        if (ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private void HandleStamina() 
    {
        if(IsSprinting) 
        {
            if (regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina = null;
                StopCoroutine(ImageFade.FadeImage(true, staminaUI, staminaClawUI));
                StartCoroutine(ImageFade.FadeImage(false, staminaUI, staminaClawUI));
            }

            currentStamina -=staminaUseMultiplier * Time.deltaTime;

            if (currentStamina < 0)
                currentStamina = 0;

            OnStaminaChange?.Invoke(currentStamina, maxStamina);

            if (currentStamina <= 0)
                CanSprint = false;
        }

        if (!IsSprinting && currentStamina < maxStamina && regeneratingStamina == null)
        {
            regeneratingStamina = StartCoroutine(RegenerateStamina());
            StartCoroutine(ImageFade.FadeImage(true, staminaUI, staminaClawUI));
        }
    }

    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 2.5f))
        {
            yield break;
        }

        duringCrouchAnimation = true;
        float timeElapsed = 0f;

        float targetHeight = isCrouching ? StandingHeight : CrouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? StandingCenter : CrouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < TimeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / TimeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / TimeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;
        duringCrouchAnimation = false;
    }

    private void ApplyDamage(float dmg) 
    {
        currentHealth -= dmg;
        OnDamage?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            KillPlayer();
        else if (regeneratingHealth != null)
            StopCoroutine(regeneratingHealth);

        regeneratingHealth = StartCoroutine(RegenerateHealth());
    }

    private void KillPlayer() 
    {
            currentHealth = 0;
        if (regeneratingHealth != null)
            StopCoroutine(regeneratingHealth);

        print("Dead");
        GameOverScript.Setup();
    }

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(healthTimeIncrement);

        while(currentHealth < maxHealth) 
        {
            currentHealth += healthValueIncrement;

            if(currentHealth > maxHealth)
                currentHealth = maxHealth;

            OnHeal?.Invoke(currentHealth, maxHealth);
            yield return timeToWait;
        }

        regeneratingHealth = null;
    }

    private IEnumerator RegenerateStamina() 
    {

        yield return new WaitForSeconds(timeBeforeStaminaRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncrement);

        while(currentStamina < maxStamina) 
        {
            if(currentStamina > 0)
                CanSprint = true;

            currentStamina += staminaValueIncrement;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            OnStaminaChange?.Invoke(currentStamina, maxStamina);

            yield return timeToWait;
        }
        regeneratingStamina = null;
    }

    private void ApplyFinalMovements()
    {
        //TO DO: limit player's movements while in mid-air

        float speed = (isCrouching ? CrouchSpeed : (IsSprinting ? SprintSpeed : WalkSpeed));

        if (!characterController.isGrounded)
            moveDirection.y -= Gravity * Time.deltaTime;

        if (characterController.velocity.y <= -1 && characterController.isGrounded)
            moveDirection.y = 0;

        if (SlopesSliding && IsSliding)
        {
            var angle = Vector3.Angle(hitPointNormal, Vector3.up);
            var angleMultiplier = (angle - characterController.slopeLimit) / characterController.slopeLimit;
            Vector3 slopeDirection = Vector3.Cross(Vector3.Cross(hitPointNormal, Vector3.down), hitPointNormal);
            moveDirection += slopeDirection * (angleMultiplier * SlopeSlideSpeed);
        }

        Vector3 moveVector = new Vector3(
            moveDirection.x * speed,
            moveDirection.y * WalkSpeed, //so the player always jump the same height
            moveDirection.z * speed
            );

        characterController.Move(moveVector * Time.deltaTime);
    }

    private void moveTowardsTheGoal()
    {
        if(playerPositions.Count() > 0)
        {
            PlayerAIMovement playerAIMovement = playerPositions[AICounter];
            float speed = (isCrouching ? CrouchSpeed : (IsSprinting ? SprintSpeed : WalkSpeed));
            float distanceToTarget = 
                //Vector3.Distance(transform.position, new Vector3(0.0f, transform.position.y, 0.0f)); 
                Vector3.Distance(transform.position, new Vector3(playerAIMovement.x, transform.position.y, playerAIMovement.z)); 

            if (distanceToTarget > 0.1f)
            {
                transform.position = 
                    //Vector3.MoveTowards(transform.position, new Vector3(0.0f, transform.position.y, 0.0f), speed * Time.deltaTime);
                    Vector3.MoveTowards(transform.position, new Vector3(playerAIMovement.x, transform.position.y, playerAIMovement.z), speed * Time.deltaTime);
            }
            else
            {
                if (AICounter + 1 < playerPositions.Count()) AICounter++;
                else AICounter = 0;
                // transform.rotation = new Quaternion(0.0f, 90 * AICounter, 0.0f, 1.0f);
                GameObject.FindGameObjectWithTag("Player").transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                transform.rotation = Quaternion.AngleAxis(270 * AICounter, transform.up);
            }
        }
    }

    bool CanSeeTarget(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToTarget);

        if (angle <= FieldOfView / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToTarget, out hit))
            {
                if (hit.collider.transform == target)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void AimAtTarget(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position;

        //Poprawka ze względu, że obiekt jest liczony od samego spodu
        directionToTarget.y += 0.5f;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(targetRotation, transform.rotation, Time.deltaTime);
    }

    public void AddTarget(Transform target)
    {
        if (!targets.Contains(target))
        {
            targets.Add(target);
        }
    }

    public void RemoveTarget(Transform target)
    {
        if (targets.Contains(target))
        {
            targets.Remove(target);
        }
    }

    Transform FindClosestVisibleTarget()
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in targets)
        {
            float distance = Vector3.Distance(target.position, transform.position);
            if (distance < closestDistance && CanSeeTarget(target))
            {
                closestTarget = target;
                closestDistance = distance;
            }
        }

        return closestTarget;
    }

    void Update()
    {
        if (CanMove)
        {
            UpdateAxises();
            HandleMovementInput();
            if (CanJump)
                HandleJump();
            if (CanCrouch)
                HandleCrouch();
            if (useStamina)
                HandleStamina();

            ApplyFinalMovements();
        } else if (AutoMove)
        {
            moveTowardsTheGoal();
            CurrentTarget = FindClosestVisibleTarget();
            if (AutoShoot && CurrentTarget != null)
            {
                if (CanSeeTarget(CurrentTarget))
                {
                    AimAtTarget(CurrentTarget);
                    PlayerGunComponent.shooting = true;
                    PlayerGunComponent.HandleShots();
                    PlayerGunComponent.shooting = false;
                }
            } else {
                PlayerGunComponent.Reload();
            }
        }
    }
}
