using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Functional Options")]
    [SerializeField] private bool CanMove = true;
    [SerializeField] private bool CanSprint = true;
    [SerializeField] private bool CanJump = true;
    [SerializeField] private bool CanCrouch = true;
    [SerializeField] private bool SlopesSliding = true;

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
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float timeBeforeRegenStarts = 3;
    [SerializeField] private float healthValueIncrement = 1;
    [SerializeField] private float healthTimeIncrement = 0.1f;

    private float currentHealth;
    private Coroutine regeneratingHealth;
    public static Action<float> OnTakeDamage;
    public static Action<float> OnDamage;
    public static Action<float> OnHeal;



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
        currentHealth = maxHealth;
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
        OnDamage?.Invoke(currentHealth);


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

            OnHeal?.Invoke(currentHealth);
            yield return timeToWait;
        }

        regeneratingHealth = null;
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

            ApplyFinalMovements();
        }
    }
}
