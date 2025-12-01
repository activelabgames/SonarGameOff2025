using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject splashEffectPrefab;
    public LayerMask ClickableLayer;

    [SerializeField] private Vector2EventChannelSO InputMoveToDestinationEvent;

    // Used to communicate player position to SoundManager in ManagerScene
    [SerializeField] private Vector3EventChannelSO PlayerPositionEvent;

    [SerializeField] private GameObjectAndVector3EventChannelSO MoveToDestinationEvent;

    public EmptyEventChannelSO EngineEvent;
    public EmptyEventChannelSO SonarEvent;
    [SerializeField] private GameObjectEventChannelSO UseActiveSonarEvent;

    [SerializeField] private EmptyEventChannelSO UIEngineEvent;
    [SerializeField] private GameObjectEventChannelSO ToggleEngineEvent;

    private void OnEnable()
    {
        //Debug.Log("PlayerController enabled.");
        if (InputMoveToDestinationEvent != null)
        {
            //Debug.Log("Subscribing to MoveToDestinationEvent");
            InputMoveToDestinationEvent.OnEventRaised += HandleTap;
        }

        if (SonarEvent != null)
        {
            SonarEvent.OnEventRaised += HandleSonarEvent;
        }
        if (UIEngineEvent != null)
        {
            UIEngineEvent.OnEventRaised += HandleUIEngineEvent;
        }
        if (EngineEvent != null)
        {
            EngineEvent.OnEventRaised += HandleKeyboardEngineEvent;
        }
    }

    private void OnDisable()
    {
        //Debug.Log("PlayerController disabled.");
        if (InputMoveToDestinationEvent != null)
        {
            //Debug.Log("Subscribing to MoveToDestinationEvent");
            InputMoveToDestinationEvent.OnEventRaised -= HandleTap;
        }

        if (SonarEvent != null)
        {
            SonarEvent.OnEventRaised -= HandleSonarEvent;
        }
        if (UIEngineEvent != null)
        {
            UIEngineEvent.OnEventRaised -= HandleUIEngineEvent;
        }
        if (EngineEvent != null)
        {
            EngineEvent.OnEventRaised -= HandleKeyboardEngineEvent;
        }
    }

    private void Update()
    {
        PlayerPositionEvent?.RaiseEvent(transform.position);
    }

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void HandleTap(Vector2 screenPosition)
    {
            if (IsPointerOverUI(screenPosition))
        {
            // Le clic est ignoré car il a touché un élément UI.
            return; 
        }
        //Debug.Log("Tap at: " + screenPosition);
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 200f, Color.red, 2f);



        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ClickableLayer))
        {
            //Debug.Log("Hit object: " + hit.collider.gameObject.name);
            // Si l'objet n'est pas interactif, on déplace le joueur.
            MoveToDestinationEvent?.RaiseEvent(gameObject, hit.point);
            if (splashEffectPrefab != null)
            {
                Vector3 splashPoint = hit.point;
                Instantiate(splashEffectPrefab, hit.point, Quaternion.identity, null);
            }
        }
    }

    /// <summary>
    /// Vérifie si la position donnée (provenant de l'Input System) touche un élément UI.
    /// </summary>
    /// <param name="screenPosition">Position de la souris sur l'écran.</param>
    /// <returns>True si un élément UI est sous le pointeur.</returns>
    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
        {
            return false;
        }
        // 1. Initialiser les données de l'événement de pointeur
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPosition;

        // 2. Créer une liste pour stocker les résultats du Raycast
        List<RaycastResult> results = new List<RaycastResult>();

        // 3. Effectuer le Raycast sur l'UI
        EventSystem.current.RaycastAll(eventData, results);

        // 4. Vérifier s'il y a des résultats
        return results.Count > 0;
    }

    private void HandleSonarEvent()
    {
        UseActiveSonarEvent?.RaiseEvent(gameObject);
    }

    public void HandleUIEngineEvent()
    {
        Debug.Log("PlayerController: UI Engine event received.");
        ToggleEngineEvent?.RaiseEvent(gameObject);
    }

    public void HandleKeyboardEngineEvent()
    {
        Debug.Log("PlayerController: Keyboard Engine event received.");
        ToggleEngineEvent?.RaiseEvent(gameObject);
    }
}