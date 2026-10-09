/*
* Author: Tyler
* Contributors: Brad Dixon
* Last Modified: 09/18/2026
* Summary: Reads in all the player's inputs and throws them through public events
* To Do:   Add more player inputs as needed.
*/


using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

//needs the playerinput to function
[RequireComponent(typeof(PlayerInput))]
public class InputManager : BaseManager
{
    public const string MidRunInputString = "MidRun";
    public const string UpgradeMenuInputString = "UpgradeMenu";

    public static InputManager Instance;

    public Vector2 CurrentMousePosition;

    public bool ControllerIsEnabled;

    private PlayerInput pInput;
    private InputAction ToggleDebugConsole;

    #region Mid Run InputActions
    private InputAction move;
    private InputAction shoot;
    private InputAction aim;
    private InputAction abilityOne;
    private InputAction abilityTwo;
    private InputAction interact;
    private InputAction dash;
    private InputAction toggleUpgradeMenuInMidRun;
    private InputAction pause;
    private InputAction swapWeapon;

    #endregion

    #region Upgrade Menu Input Actions

    private InputAction ToggleUpgradeMenuInUpgradeMenu;

    private InputAction SwapFocusToTarot;

    private InputAction SwapFocusToInventory;

    private InputAction SwapFocusToGrid;

    private InputAction EnableWeapon1;

    private InputAction EnableWeapon2;

    private InputAction SelectPin;

    private InputAction MousePosition;

    private InputAction MoveSelectedObject;

    private InputAction DropHoldingPin;
    #endregion

    #region Setup
    /// <summary>
    /// initializes the player inputs
    /// </summary>
    /// <returns></returns>
    public async override Awaitable InitManager()
    {
        await base.InitManager();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        await SetUpInputActions();
    }

    /// <summary>
    /// Performs first time set up for the input actions
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetUpInputActions()
    {
        //grabs the player input and enables the map
        pInput = GetComponent<PlayerInput>();
        pInput.currentActionMap = pInput.actions.FindActionMap(MidRunInputString, true);
        pInput.currentActionMap.Enable();


        
        EnableEvergreenPublicEvents();
        //sets up the individual input actions
        EnableMidRunPublicEvents();

        await Task.CompletedTask;
    }

    

    /// <summary>
    /// enables the evergreen public events
    /// </summary>
    private void EnableEvergreenPublicEvents()
    {
        pInput.onControlsChanged += PInput_onControlsChanged;

        UIPublicEvents.UpgradeMenuOpened += SwapActionMapToUpgradeMenu;
        UIPublicEvents.UpgradeMenuClosed += SwapActionMapToMidRun;
        GenericPublicEvents.AllManagersInitialized += ThrowCurrentControlScheme;
    }

    /// <summary>
    /// disables the evergreen public events
    /// </summary>
    private void DisableEvergreenPublicEvents()
    {
        pInput.onControlsChanged -= PInput_onControlsChanged;
        UIPublicEvents.UpgradeMenuOpened -= SwapActionMapToUpgradeMenu;
        UIPublicEvents.UpgradeMenuClosed -= SwapActionMapToMidRun;
        GenericPublicEvents.AllManagersInitialized -= ThrowCurrentControlScheme;
    }


    /// <summary>
    /// unsubscribes from all public events
    /// </summary>
    private void OnDestroy()
    {
        DisableEvergreenPublicEvents();

        if (move != null)
        {
            DisableMidRunPublicEvents();
        }
        if (ToggleUpgradeMenuInUpgradeMenu != null)
        {
            DisableUpgradeMenuPublicEvents();
        }
        
    }

    #endregion

    #region Control Scheme Changing
    /// <summary>
    /// Triggers when the player swaps their input controls
    /// </summary>
    /// <param name="obj"></param>
    private void PInput_onControlsChanged(PlayerInput obj)
    {
        ThrowCurrentControlScheme();
    }

    /// <summary>
    /// Calls sub functions depending on the type of control scheme thats currently being used
    /// </summary>
    private void ThrowCurrentControlScheme()
    {
        if (pInput.currentControlScheme.ToLower() == "controller")
        {
            ControllerEnabled();
        }
        else if (pInput.currentControlScheme.ToLower() == "keyboardmouse")
        {
            KeyboardMouseEnabled();
        }
    }

    /// <summary>
    /// throws an event when the controller becomes enabled
    /// </summary>
    private void ControllerEnabled()
    {
        ControllerIsEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        InputPublicEvents.ControllerEnabled?.Invoke();
    }
    
    /// <summary>
    /// throws an event when the keyboard and mouse become enabled
    /// </summary>
    private void KeyboardMouseEnabled()
    {
        ControllerIsEnabled = false;
        Cursor.lockState = CursorLockMode.None;
        InputPublicEvents.KeyboardMouseEnabled?.Invoke();
    }

    #endregion

    #region enabling and disabling public events

    /// <summary>
    /// enables the midrun action map
    /// </summary>
    public void SwapActionMapToMidRun()
    {
        pInput.currentActionMap.Disable();
        pInput.SwitchCurrentActionMap(MidRunInputString);
        pInput.currentActionMap.Enable();

        DisableUpgradeMenuPublicEvents();
        EnableMidRunPublicEvents();

        Debug.Log("mid run enabled");

    }

    /// <summary>
    /// enables the upgrade menu action map
    /// </summary>
    public void SwapActionMapToUpgradeMenu()
    {
        pInput.currentActionMap.Disable();
        pInput.SwitchCurrentActionMap(UpgradeMenuInputString);
        pInput.currentActionMap.Enable();

        DisableMidRunPublicEvents();
        EnableUpgradeMenuPublicEvents();

        Debug.Log("upgrade menu enabled");
    }

    /// <summary>
    /// enables the midrun public events
    /// </summary>
    private void EnableMidRunPublicEvents()
    {
        //needs to be redone every time the action map changes
        move = pInput.currentActionMap.FindAction("Move");
        shoot = pInput.currentActionMap.FindAction("Shoot");
        interact = pInput.currentActionMap.FindAction("Interact");
        aim = pInput.currentActionMap.FindAction("Aim");
        abilityOne = pInput.currentActionMap.FindAction("AbilityOne");
        abilityTwo = pInput.currentActionMap.FindAction("AbilityTwo");
        dash = pInput.currentActionMap.FindAction("Dash");
        toggleUpgradeMenuInMidRun = pInput.currentActionMap.FindAction("ToggleUpgradeMenu");
        pause = pInput.currentActionMap.FindAction("Pause");
        swapWeapon = pInput.currentActionMap.FindAction("SwapWeapon");
        ToggleDebugConsole = pInput.currentActionMap.FindAction("ToggleDebugConsole");
        


        move.performed += Move_performed;
        move.canceled += Move_canceled;

        shoot.started += Shoot_started;
        shoot.canceled += Shoot_canceled;

        interact.started += Interact_started;
        interact.canceled += Interact_canceled;

        aim.performed += Aim_performed;

        abilityOne.started += AbilityOne_started;

        abilityTwo.started += AbilityTwo_started;

        dash.started += Dash_started;

        toggleUpgradeMenuInMidRun.started += ToggleUpgradeMenu_started;

        pause.started += Pause_started;

        swapWeapon.started += SwapWeapon_started;

        ToggleDebugConsole.started += ToggleDebugConsole_started;

        
    }

    



    /// <summary>
    /// enables the upgrade menu public events
    /// </summary>
    private void EnableUpgradeMenuPublicEvents()
    {
        ToggleUpgradeMenuInUpgradeMenu = pInput.currentActionMap.FindAction("ToggleUpgradeMenu");
        SwapFocusToTarot = pInput.currentActionMap.FindAction("SwapFocusToTarot");
        SwapFocusToInventory = pInput.currentActionMap.FindAction("SwapFocusToInventory");
        SwapFocusToGrid = pInput.currentActionMap.FindAction("SwapFocusToGrid");
        EnableWeapon1 = pInput.currentActionMap.FindAction("EnableWeapon1");
        EnableWeapon2 = pInput.currentActionMap.FindAction("EnableWeapon2");
        SelectPin = pInput.currentActionMap.FindAction("SelectPin");
        MousePosition = pInput.currentActionMap.FindAction("MousePosition");
        MoveSelectedObject = pInput.currentActionMap.FindAction("Move");
        ToggleDebugConsole = pInput.currentActionMap.FindAction("ToggleDebugConsole");
        DropHoldingPin = pInput.currentActionMap.FindAction("CancelHoldingItem");

        ToggleUpgradeMenuInUpgradeMenu.started += ToggleUpgradeMenuInUpgradeMenu_started;
        SwapFocusToGrid.started += SwapFocusToGrid_started;
        SwapFocusToInventory.started += SwapFocusToInventory_started;
        SwapFocusToTarot.started += SwapFocusToTarot_started;
        EnableWeapon1.started += EnableWeapon1_started;
        EnableWeapon2.started += EnableWeapon2_started;
        SelectPin.started += SelectPin_started;
        SelectPin.canceled += SelectPin_canceled;
        MousePosition.performed += Aim_performed;
        MoveSelectedObject.performed += Aim_performed;
        ToggleDebugConsole.started += ToggleDebugConsole_started;
        MoveSelectedObject.canceled += MoveSelectedObject_canceled;
        DropHoldingPin.started += DropHoldingPin_started;
    }

    



    /// <summary>
    /// disables the mid run public events
    /// </summary>
    private void DisableMidRunPublicEvents()
    {
        move.performed -= Move_performed;
        move.canceled -= Move_canceled;

        shoot.started -= Shoot_started;
        shoot.canceled -= Shoot_canceled;

        interact.started -= Interact_started;
        interact.canceled -= Interact_canceled;

        aim.performed -= Aim_performed;

        abilityOne.started -= AbilityOne_started;

        abilityTwo.started -= AbilityTwo_started;

        dash.started -= Dash_started;

        toggleUpgradeMenuInMidRun.started -= ToggleUpgradeMenu_started;
        
    }

    /// <summary>
    /// disables the upgrade menu public events
    /// </summary>
    private void DisableUpgradeMenuPublicEvents()
    {
        ToggleUpgradeMenuInUpgradeMenu.started -= ToggleUpgradeMenuInUpgradeMenu_started;
        SwapFocusToGrid.started -= SwapFocusToGrid_started;
        SwapFocusToInventory.started -= SwapFocusToInventory_started;
        SwapFocusToTarot.started -= SwapFocusToTarot_started;
        EnableWeapon1.started -= EnableWeapon1_started;
        EnableWeapon2.started -= EnableWeapon2_started;
        SelectPin.started -= SelectPin_started;
        SelectPin.canceled -= SelectPin_canceled;
        MousePosition.performed -= Aim_performed;
        DropHoldingPin.started -= DropHoldingPin_started;
    }


    #endregion

    #region Mid Run InputHandling Functions

    /// <summary>
    /// Throws the movepressed public event
    /// </summary>
    /// <param name="obj"></param>
    private void Move_performed(InputAction.CallbackContext obj)
    {
        InputPublicEvents.MovePressed?.Invoke(obj.ReadValue<Vector2>());

    }

    /// <summary>
    /// Throws the movecancelled public event
    /// </summary>
    /// <param name="obj"></param>
    private void Move_canceled(InputAction.CallbackContext obj)
    {
        InputPublicEvents.MoveReleased?.Invoke();
    }

    /// <summary>
    /// Throws the shootpressed public event
    /// </summary>
    /// <param name="obj"></param>
    private void Shoot_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.ShootPressed?.Invoke();
    }

    /// <summary>
    /// Throws the shootcancelled public event
    /// </summary>
    /// <param name="obj"></param>
    private void Shoot_canceled(InputAction.CallbackContext obj)
    {
        InputPublicEvents.ShootReleased?.Invoke();
    }

    /// <summary>
    /// Throws the interactpressed public event
    /// </summary>
    /// <param name="obj"></param>
    private void Interact_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.InteractPressed?.Invoke();
        Debug.Log("Interact pressed");
    }

    /// <summary>
    /// Throws the interactcancelled public event
    /// </summary>
    /// <param name="obj"></param>
    private void Interact_canceled(InputAction.CallbackContext obj)
    {
        InputPublicEvents.InteractReleased?.Invoke();
    }

    /// <summary>
    /// Calls the public event that returns the mouse's position
    /// </summary>
    /// <param name="obj"></param>
    private void Aim_performed(InputAction.CallbackContext obj)
    {
        if (!ControllerIsEnabled)
        {
            CurrentMousePosition = obj.ReadValue<Vector2>();
            InputPublicEvents.MouseMoved?.Invoke(CurrentMousePosition);
        }
        else
        {
            InputPublicEvents.PlayerAimed?.Invoke(obj.ReadValue<Vector2>());
        }
        
    }

    /// <summary>
    /// Calls the public event for pressing the ability one button
    /// </summary>
    /// <param name="obj"></param>
    private void AbilityOne_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.AbilityOnePressed?.Invoke();
    }

    /// <summary>
    /// Calls the public event for pressing the ability two button
    /// </summary>
    /// <param name="obj"></param>
    private void AbilityTwo_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.AbilityTwoPressed?.Invoke();
    }

    /// <summary>
    /// Calls the public event for toggling the upgrade menu
    /// </summary>
    /// <param name="obj"></param>
    private void ToggleUpgradeMenu_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.ToggleUpgradeMenuPressed?.Invoke();
    }

    /// <summary>
    /// Calls the public event for dash
    /// </summary>
    /// <param name="obj"></param>
    private void Dash_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.DashPressed?.Invoke();
    }

    /// <summary>
    /// calls the public event that swaps the player's equipped weapon
    /// </summary>
    /// <param name="obj"></param>
    private void SwapWeapon_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.SwapWeaponPressed?.Invoke();
    }

    /// <summary>
    /// calls the public event that toggles the pause menu
    /// </summary>
    /// <param name="obj"></param>
    private void Pause_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.PausePressed?.Invoke();
    }

    /// <summary>
    /// Cancels the player aiming
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private void MoveSelectedObject_canceled(InputAction.CallbackContext obj)
    {
        InputPublicEvents.AimCancelled?.Invoke();
    }

    #endregion

    #region UpgradeMenu Input Handling Functions

    private void DropHoldingPin_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.DropPin?.Invoke();
    }

    /// <summary>
    /// calls the public event that 'clicks' on the currently selected pin
    /// </summary>
    /// <param name="obj"></param>
    private void SelectPin_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.SelectPin?.Invoke();
        Debug.Log("Pin Selected");
    }

    private void SelectPin_canceled(InputAction.CallbackContext obj)
    {
        if (!ControllerIsEnabled)
        {
            InputPublicEvents.PinReleased?.Invoke();
        }
        
    }


    /// <summary>
    /// calls the public event that enables the grid for weapon 2
    /// </summary>
    /// <param name="obj"></param>
    private void EnableWeapon2_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.EnableWeapon2?.Invoke();
    }

    /// <summary>
    /// calls the public event that enables the grid for weapon 1
    /// </summary>
    /// <param name="obj"></param>
    private void EnableWeapon1_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.EnableWeapon1?.Invoke();
    }

    /// <summary>
    /// calls the public event that swaps the current focus to tarot
    /// </summary>
    /// <param name="obj"></param>
    private void SwapFocusToTarot_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.SwapFocusToTarot?.Invoke();
    }

    /// <summary>
    /// calls the public event that swaps the current focus to the inventory
    /// </summary>
    /// <param name="obj"></param>
    private void SwapFocusToInventory_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.SwapFocusToInventory?.Invoke();
    }

    /// <summary>
    /// calls the public event that swaps the current focus to the grid
    /// </summary>
    /// <param name="obj"></param>
    private void SwapFocusToGrid_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.SwapFocusToGrid?.Invoke();
    }

    /// <summary>
    /// calls the public event that closes/opens the upgrade menu
    /// </summary>
    /// <param name="obj"></param>
    private void ToggleUpgradeMenuInUpgradeMenu_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.ToggleUpgradeMenuPressed?.Invoke();
    }



    #endregion

    /// <summary>
    /// throws the event to turn on and off the debug console
    /// </summary>
    /// <param name="obj"></param>
    private void ToggleDebugConsole_started(InputAction.CallbackContext obj)
    {
        InputPublicEvents.ToggleDebugConsole?.Invoke();
    }
}
