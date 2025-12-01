// AllParametersContainerSOEditor.cs (Doit être dans un dossier 'Editor')
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(PlayerParametersSO))]
public class PlayerParametersSOEditor : Editor
{
    // Cache des éditeurs pour éviter de les recréer à chaque frame
    private Dictionary<ScriptableObject, Editor> _cachedEditors = new Dictionary<ScriptableObject, Editor>();

    public override void OnInspectorGUI()
    {
        PlayerParametersSO container = (PlayerParametersSO)target;

        // 1. Dessiner le conteneur lui-même (facultatif, mais utile pour ajouter/supprimer des éléments)
        DrawDefaultInspector();

        // Ligne de séparation claire
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("📦 Player parameters", EditorStyles.whiteLargeLabel);
        EditorGUILayout.Space(10);

        if (container.HealthCharacteristics == null)
        {
            EditorGUILayout.HelpBox("Veuillez assigner les caractéristiques de santé.", MessageType.Warning);
            return;
        }

        // --- Cache ou Création de l'Éditeur Enfant ---
        if (!_cachedEditors.ContainsKey(container.HealthCharacteristics) || _cachedEditors[container.HealthCharacteristics] == null)
        {
            // Crée l'éditeur qui gère l'affichage personnalisé
            _cachedEditors[container.HealthCharacteristics] = Editor.CreateEditor(container.HealthCharacteristics);
        }

        Editor targetEditor = _cachedEditors[container.HealthCharacteristics];

        // Affichage du cadre de l'éditeur enfant
        EditorGUILayout.BeginVertical("box");

        // Affiche le nom de l'objet (pour savoir quel éditeur on modifie)
        EditorGUILayout.LabelField($"✨ {container.HealthCharacteristics.name}", EditorStyles.boldLabel);

        // Dessine l'inspecteur de l'objet référencé
        targetEditor.OnInspectorGUI();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);

        if (container.StartWeapons == null)
        {
            EditorGUILayout.HelpBox("Veuillez assigner les armes de départ.", MessageType.Warning);
            return;
        }

        // --- Cache ou Création de l'Éditeur Enfant ---
        if (!_cachedEditors.ContainsKey(container.StartWeapons) || _cachedEditors[container.StartWeapons] == null)
        {
            // Crée l'éditeur qui gère l'affichage personnalisé
            _cachedEditors[container.StartWeapons] = Editor.CreateEditor(container.StartWeapons);
        }

        Editor targetEditor2 = _cachedEditors[container.StartWeapons];

        // Affichage du cadre de l'éditeur enfant
        EditorGUILayout.BeginVertical("box");

        // Affiche le nom de l'objet (pour savoir quel éditeur on modifie)
        EditorGUILayout.LabelField($"✨ {container.StartWeapons.name}", EditorStyles.boldLabel);

        // Dessine l'inspecteur de l'objet référencé
        targetEditor2.OnInspectorGUI();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);

        if (container.StartEquipments == null)
        {
            EditorGUILayout.HelpBox("Veuillez assigner les équipements de départ.", MessageType.Warning);
            return;
        }

        // --- Cache ou Création de l'Éditeur Enfant ---
        if (!_cachedEditors.ContainsKey(container.StartEquipments) || _cachedEditors[container.StartEquipments] == null)
        {
            // Crée l'éditeur qui gère l'affichage personnalisé
            _cachedEditors[container.StartEquipments] = Editor.CreateEditor(container.StartEquipments);
        }

        Editor targetEditor3 = _cachedEditors[container.StartEquipments];

        // Affichage du cadre de l'éditeur enfant
        EditorGUILayout.BeginVertical("box");

        // Affiche le nom de l'objet (pour savoir quel éditeur on modifie)
        EditorGUILayout.LabelField($"✨ {container.StartEquipments.name}", EditorStyles.boldLabel);

        // Dessine l'inspecteur de l'objet référencé
        targetEditor3.OnInspectorGUI();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);

    }
}