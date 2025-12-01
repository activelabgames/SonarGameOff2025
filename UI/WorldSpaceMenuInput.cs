using UnityEngine;
using UnityEngine.UIElements;

public class WorldSpaceMenuInput : MonoBehaviour
{
    [Header("Configurations")]
    public UIDocument uiDocument; // Votre document UI
    public RenderTexture renderTexture; // La texture où l'UI est dessinée
    public Collider meshCollider; // Le collider du Quad

    private void OnEnable()
    {
        // On dit au Panel Settings : "Stop ! N'utilise pas la souris par défaut."
        // "Utilise ma fonction personnalisée pour calculer la position du curseur."
        uiDocument.panelSettings.SetScreenToPanelSpaceFunction(ConvertScreenToPanelSpace);
    }

    private void OnDisable()
    {
        // On remet le comportement par défaut si on désactive le script
        uiDocument.panelSettings.SetScreenToPanelSpaceFunction(null);
    }

    // Cette fonction est appelée par Unity à chaque frame pour savoir où est la souris DANS l'UI
    private Vector2 ConvertScreenToPanelSpace(Vector2 screenPosition)
    {
        Debug.Log("WorldSpaceMenuInput: Converting screen position " + screenPosition);
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        // On lance un rayon physique
        if (meshCollider.Raycast(ray, out hit, 1000f))
        {
            // hit.textureCoord nous donne la position UV (entre 0 et 1) sur le Quad
            Vector2 pixelUV = hit.textureCoord;

            // ATTENTION : Parfois la Render Texture inverse le Y. 
            // Si vos boutons sont inversés haut/bas, décommentez la ligne suivante :
            pixelUV.y = 1 - pixelUV.y;

            // On convertit cette position 0-1 en pixels réels de l'interface (ex: 1920x1080)
            pixelUV.x *= renderTexture.width;
            pixelUV.y *= renderTexture.height;

            return pixelUV;
        }

        // Si on ne touche pas le menu, on renvoie une position "impossible" (loin de l'écran)
        // pour être sûr qu'aucun bouton ne reste en surbrillance (hover).
        return new Vector2(-1000, -1000);
    }
}