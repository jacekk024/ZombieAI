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
    private InputAction cameraAction;
    private InputAction shootAction;
    private InputAction invOpenAction;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        movementAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        crouchAction = playerInput.actions["Crouch"];
        sprintAction = playerInput.actions["Sprint"];
        reloadAction = playerInput.actions["GunReload"];
        cameraAction = playerInput.actions["Camera Look"];
        shootAction = playerInput.actions["Shoot"];
        invOpenAction = playerInput.actions["Open Inventory"];
    }

    private Vector2 GetVector2DInput(InputAction action)
    {
        if (Time.timeScale > 0.0f)
            return action.ReadValue<Vector2>();
        else
            return new Vector2(0.0f, 0.0f);
    }

    private bool GetInputKey(InputAction action)
    {
        return Time.timeScale > 0.0f && action.IsPressed();
    }

    private bool GetInputKeyDown(InputAction action, bool ignoreTimeFreeze = false)
    {
        if (ignoreTimeFreeze)
            return action.WasPerformedThisFrame();
        else
            return Time.timeScale > 0.0f && action.WasPerformedThisFrame();
    }

    public Vector2 GetMovementInput()
    {
        return GetVector2DInput(movementAction);
    }

    public bool GetJumpInput()
    {
        return GetInputKey(jumpAction);
    }

    public bool GetCrouchInput()
    {
        return GetInputKeyDown(crouchAction);
    }

    public bool GetSprintInput()
    {
        return GetInputKey(sprintAction);
    }

    public bool GetReloadInput()
    {
        return GetInputKeyDown(reloadAction);
    }

    public Vector2 GetCameraLookInput()
    {
        return GetVector2DInput(cameraAction);
    }

    public bool GetWeaponShotInput(bool allowButtonHold)
    {
        return allowButtonHold ? GetInputKey(shootAction) : GetInputKeyDown(shootAction);
    }

    public bool GetInventoryOpenInput()
    {
        return !GameplaySettings.isPauseMenuOpen && GetInputKeyDown(invOpenAction, true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
