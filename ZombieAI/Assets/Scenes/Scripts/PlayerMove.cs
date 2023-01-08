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

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float sprintSpeed = 18f;
    [SerializeField] private float crouchSpeed = 4f;

    [Header("Jumping Parameters")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpForce = 2.5f;

    [Header("Crouching Parameters")]
    [SerializeField] private float crouchHeight = 0.4f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.2f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);

    private Vector3 moveDirection;
    private CharacterController characterController;
    private Camera playerCamera;

    private bool ShouldJump => Input.GetKey(jumpKey) && characterController.isGrounded && !isCrouching;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;
    private bool isCrouching;
    private bool duringCrouchAnimation;

    public bool IsGrounded()
    {
        return characterController.isGrounded;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool IsSprinting => CanSprint && Input.GetKey(sprintKey);
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
    }

    private void HandleMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * z) + (transform.TransformDirection(Vector3.right) * x);
        moveDirection = Vector3.Normalize(moveDirection);
        moveDirection.y = moveDirectionY;
    }

    private void HandleJump()
    {
        if (ShouldJump)
            moveDirection.y = jumpForce;
    }

    private void HandleCrouch()
    {
        if(ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private IEnumerator CrouchStand()
    {
        if(isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f)) 
        {
            yield break;
        }

        duringCrouchAnimation = true;
        float timeElapsed = 0f;

        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;
        duringCrouchAnimation = false;
    }

    private void ApplyFinalMovements()
    {
        //TO DO: limit player's movements while in mid-air

        float speed = (isCrouching ? crouchSpeed : (IsSprinting ? sprintSpeed : walkSpeed));

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        if (characterController.velocity.y <= -1 && characterController.isGrounded)
            moveDirection.y = 0;

        Vector3 moveVector = new Vector3(
            moveDirection.x * speed,
            moveDirection.y * walkSpeed, //so the player always jump the same height
            moveDirection.z * speed
            );

        characterController.Move(moveVector * Time.deltaTime);
    }

    void Update()
    {
        if(CanMove)
        {
            HandleMovementInput();
            if (CanJump)
                HandleJump();
            if (CanCrouch)
                HandleCrouch();

            ApplyFinalMovements();
        }
    }
}
