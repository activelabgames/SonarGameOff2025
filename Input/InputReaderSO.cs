using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Sonar/InputReader")]
public class InputReaderSO : ScriptableObject, PlayerControls.IGameActions
{
    private PlayerControls _playerControls;

    // ---------------------------------------------------------------------
    // Events Declaration
    // ---------------------------------------------------------------------

    //public event Action<Vector2> MoveToDestinationEvent;
    [SerializeField] private Vector2EventChannelSO InputMoveToDestinationEvent;

    //public event Action SonarEvent;
    public EmptyEventChannelSO SonarEvent;
    public EmptyEventChannelSO EngineEvent;
    //public event Action FireEvent;
    public Vector2EventChannelSO UseWeaponEvent;

    public Vector2EventChannelSO MousePositionEvent;
    public Vector2EventChannelSO MouseScrollEvent;
    public BoolEventChannelSO DragEvent;

    public Vector2EventChannelSO MoveCameraEvent;
    

    //public event Action ResumeGameEvent;
    [SerializeField] BoolEventChannelSO PauseGameEvent;
    //public event Action PauseGameEvent;
//    [SerializeField] BoolEventChannelSO ResumeGameEvent;

    private Vector2 tapPosition;
    private Vector2 rightTapPosition;


    // ---------------------------------------------------------------------
    // Lifecycle Management
    // ---------------------------------------------------------------------

    private void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();
            _playerControls.Game.SetCallbacks(this);
            //_playerControls.UI.SetCallbacks(this);
            // Active le contexte de jeu par défaut
            SetGame();
        }
    }

    private void OnDisable()
    {
        if (_playerControls != null)
        {
            _playerControls.Disable();
            //_playerControls.Dispose();
            _playerControls = null;
        }
    }

    // ---------------------------------------------------------------------
    // Context Switching
    // ---------------------------------------------------------------------

    public void SetGame()
    {
        _playerControls.Game.Enable();
        //_playerControls.UI.Disable();
    }

    public void SetUI()
    {
        _playerControls.Game.Disable();
        //_playerControls.UI.Enable();
    }

    /// <summary>
    /// Bascule entre le mode BuildMenu et le mode Construction.
    /// Déclenche l'événement correspondant au changement.
    /// </summary>
    public void TogglePauseMode() 
    {
        /*if (_playerControls.UI.enabled)
        {
            // FERMETURE DU MENU DE PAUSE
            SetGame();
            //ResumeGameEvent?.Invoke();
            PauseGameEvent?.RaiseEvent(true);
        }
        else
        {
            // OUVERTURE DU MENU
            SetUI();
            //PauseGameEvent?.Invoke();
            PauseGameEvent?.RaiseEvent(false);
        }*/
    }

    // ---------------------------------------------------------------------
    // IGameActions implementation
    // ---------------------------------------------------------------------

    public void OnTap(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log("InputReaderSO: OnTap performed at position " + tapPosition);
            //MoveToDestinationEvent?.Invoke(tapPosition);
            //Debug.Log("Raising MoveToDestinationEvent with position: " + tapPosition);
            //Debug.Log($"MoveToDestinationEvent is null: {MoveToDestinationEvent == null}");
            InputMoveToDestinationEvent?.RaiseEvent(tapPosition);
        }
    }

    public void OnTapPosition(InputAction.CallbackContext context)
    {
        tapPosition = context.ReadValue<Vector2>();
    }

    public void OnRightTapPosition(InputAction.CallbackContext context)
    {
        rightTapPosition = context.ReadValue<Vector2>();
    }

    public void OnSonar(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //SonarEvent?.Invoke();
            SonarEvent?.RaiseEvent();
        }
    }

    public void OnToggleEngine(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            EngineEvent?.RaiseEvent();
        }
    }

    public void OnUseWeapon(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //FireEvent?.Invoke();
            UseWeaponEvent?.RaiseEvent(rightTapPosition);
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            MousePositionEvent?.RaiseEvent(context.ReadValue<Vector2>());
        }
    }
    public void OnMouseScroll(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            MouseScrollEvent?.RaiseEvent(context.ReadValue<Vector2>());
        }
    }

    public void OnMoveCamera(InputAction.CallbackContext context)
    {
            MoveCameraEvent?.RaiseEvent(context.ReadValue<Vector2>());
    }

    public void OnDrag(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            DragEvent?.RaiseEvent(true);
        else if (context.phase == InputActionPhase.Canceled)
            DragEvent?.RaiseEvent(false);
    }

    // ---------------------------------------------------------------------
    // IUIActions implementation
    // ---------------------------------------------------------------------

    public void OnResumeGame(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            TogglePauseMode();
        }
    }
}