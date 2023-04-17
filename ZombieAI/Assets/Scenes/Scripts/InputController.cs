using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private PlayerInput playerInput;

    private InputAction movementAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction sprintAction;
    private InputAction reloadAction;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        movementAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        crouchAction = playerInput.actions["Crouch"];
        sprintAction = playerInput.actions["Sprint"];
        reloadAction = playerInput.actions["GunReload"];

    }

    public Vector2 GetMovementInput()
    {
        return movementAction.ReadValue<Vector2>();
    }

    public bool GetJumpInput()
    {
        return jumpAction.IsPressed();
    }
    
    public bool GetCrouchInput()
    {
        return crouchAction.WasPressedThisFrame();
    }

    public bool GetSprintInput()
    {
        return sprintAction.IsPressed();
    }

    public bool GetReloadInput()
    {
        return reloadAction.WasPressedThisFrame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
