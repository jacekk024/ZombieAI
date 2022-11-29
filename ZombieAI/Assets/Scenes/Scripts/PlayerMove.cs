using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Functional Options")]
    [SerializeField] private bool CanMove = true;
    [SerializeField] private bool CanSprint = true;
    [SerializeField] private bool CanJump = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 18f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpForce = 8f;


    private Vector3 moveDirection;
    private Vector2 currentInput;

    private CharacterController characterController;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public float jumpHeight = 3f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;

    private bool IsSprinting => CanSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKey(jumpKey) && characterController.isGrounded;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
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

    private void ApplyFinalMovements()
    {
        //TO DO: limit player's movements while in mid-air

        float speed = (IsSprinting ? sprintSpeed : walkSpeed);

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        if (characterController.velocity.y <= -1 && characterController.isGrounded)
            moveDirection.y = 0;

        Vector3 moveVector = new Vector3(
            moveDirection.x * speed,
            moveDirection.y * walkSpeed, //so the player doesn't jump higher while sprinting
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

            ApplyFinalMovements();
        }
    }
}
