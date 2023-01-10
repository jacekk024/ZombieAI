using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Functional Options")]
    [SerializeField] private bool CanLookAround = true;
    [SerializeField] private bool ViewBobbingActive = true;

    [Header("Camera parameters")]
    [SerializeField] private float Sensitivity = 350f;
    [SerializeField] private float WalkBobSpeed = 14f;
    [SerializeField] private float WalkBobAmount = 0.1f;
    [SerializeField] private float SprintBobSpeed = 18f;
    [SerializeField] private float SprintBobAmount = 0.2f;
    [SerializeField] private float CrouchBobSpeed = 8f;
    [SerializeField] private float CrouchBobAmount = 0.05f;

    [Header("References")]
    [SerializeField] private Transform playerBody;// to inform about rotation of player
    [SerializeField] private PlayerMove playerMove;
    float xRotation = 0f;

    private float defaultYPos;
    private float timer;

    // Start is called before the first frame update
    void Awake()
    {
        defaultYPos = transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void HandleMouseLook()
    {
        if (CanLookAround)
        {
            float mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    void HandleViewBob()
    {
        if (!playerMove.IsGrounded())
            return;

        Vector3 moveDir = playerMove.GetMoveDirection();

        if (Mathf.Abs(moveDir.x) > 0.1f || Mathf.Abs(moveDir.z) > 0.1f)
        {
            timer += Time.deltaTime * (playerMove.IsCrouching ? CrouchBobSpeed : (playerMove.IsSprinting ? SprintBobSpeed : WalkBobSpeed)); ;
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (playerMove.IsCrouching ? CrouchBobAmount : (playerMove.IsSprinting ? SprintBobAmount : WalkBobAmount)),
                transform.localPosition.z
                );

        }
        else if (transform.localPosition.y != defaultYPos)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, defaultYPos, transform.localPosition.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseLook();
        if (ViewBobbingActive)
            HandleViewBob();
    }
}
