// EquipmentsParametersContainerSOEditor.cs (Doit être dans un dossier 'Editor')
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Sonar;

[CustomEditor(typeof(EquipmentsParametersContainerSO))]
public class EquipmentsParametersContainerSOEditor : Editor
{
    // Cache des éditeurs pour éviter de les recréer à chaque frame
    private Dictionary<ScriptableObject, Editor> _cachedEditors = new Dictionary<ScriptableObject, Editor>();

    public override void OnInspectorGUI()
    {
        EquipmentsParametersContainerSO container = (EquipmentsParametersContainerSO)target;

        // 1. Dessiner le conteneur lui-même (facultatif, mais utile pour ajouter/supprimer des éléments)
        DrawDefaultInspector();
        
        // Ligne de séparation claire
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("📦 Paramètres des équipements", EditorStyles.whiteLargeLabel);
        EditorGUILayout.Space(10);
        
        // 2. Itérer et Dessiner les Éditeurs Personnalisés
        foreach (ScriptableObject parametersSO in container.EquipmentsParameters)
        {
            if (parametersSO == null) continue;

            // --- Cache ou Création de l'Éditeur Enfant ---
            if (!_cachedEditors.ContainsKey(parametersSO) || _cachedEditors[parametersSO] == null)
            {
                // Crée l'éditeur qui gère l'affichage personnalisé
                _cachedEditors[parametersSO] = Editor.CreateEditor(parametersSO);
            }
            Editor targetEditor = _cachedEditors[parametersSO];
            // --- Fin du Cache ---

            
            // Affichage du cadre de l'éditeur enfant
            EditorGUILayout.BeginVertical("box");
            
            // Affiche le nom de l'objet (pour savoir quel éditeur on modifie)
            EditorGUILayout.LabelField($"✨ {parametersSO.name}", EditorStyles.boldLabel);

            // Dessine l'inspecteur de l'objet référencé
            targetEditor.OnInspectorGUI();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
    }


}