using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(PlayersParametersSO))]
public class PlayersParametersSOEditor : Editor
{
    // Cache des éditeurs pour éviter de les recréer à chaque frame
    private Dictionary<ScriptableObject, Editor> _cachedEditors = new Dictionary<ScriptableObject, Editor>();
    public override void OnInspectorGUI()
    {
        PlayersParametersSO parameters = (PlayersParametersSO)target;

        // Option 1: Dessinez l'inspecteur par défaut en haut pour les champs standard
        DrawDefaultInspector();

        if (parameters.PlayerParameters == null)
        {
            EditorGUILayout.HelpBox("Veuillez assigner les paramètres du joueur.", MessageType.Warning);
            return;
        }

        // Ligne de séparation claire
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("📦 Players parameters", EditorStyles.whiteLargeLabel);
        EditorGUILayout.Space(10);

        // --- Cache ou Création de l'Éditeur Enfant ---
        if (!_cachedEditors.ContainsKey(parameters.PlayerParameters) || _cachedEditors[parameters.PlayerParameters] == null)
        {
            // Crée l'éditeur qui gère l'affichage personnalisé
            _cachedEditors[parameters.PlayerParameters] = Editor.CreateEditor(parameters.PlayerParameters);
        }

        Editor targetEditor = _cachedEditors[parameters.PlayerParameters];

        // Affichage du cadre de l'éditeur enfant
        EditorGUILayout.BeginVertical("box");

        // Affiche le nom de l'objet (pour savoir quel éditeur on modifie)
        EditorGUILayout.LabelField($"✨ {parameters.PlayerParameters.name}", EditorStyles.boldLabel);

        // Dessine l'inspecteur de l'objet référencé
        targetEditor.OnInspectorGUI();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);

        if (parameters.EnemyParameters == null)
        {
            EditorGUILayout.HelpBox("Veuillez assigner les paramètres des ennemis.", MessageType.Warning);
            return;
        }

        // --- Cache ou Création de l'Éditeur Enfant ---
        if (!_cachedEditors.ContainsKey(parameters.EnemyParameters) || _cachedEditors[parameters.EnemyParameters] == null)
        {
            // Crée l'éditeur qui gère l'affichage personnalisé
            _cachedEditors[parameters.EnemyParameters] = Editor.CreateEditor(parameters.EnemyParameters);
        }

        Editor targetEditor2 = _cachedEditors[parameters.EnemyParameters];

        // Affichage du cadre de l'éditeur enfant
        EditorGUILayout.BeginVertical("box");

        // Affiche le nom de l'objet (pour savoir quel éditeur on modifie)
        EditorGUILayout.LabelField($"✨ {parameters.EnemyParameters.name}", EditorStyles.boldLabel);

        // Dessine l'inspecteur de l'objet référencé
        targetEditor2.OnInspectorGUI();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
        /*
        // --- Code d'éditeur personnalisé ---
        GUILayout.Space(10);
        EditorGUILayout.LabelField("⚙️ Dégâts Calculés", EditorStyles.boldLabel);

        // Exemple de logique d'affichage personnalisée
        float baseCritDmg = parameters.BaseDamage * 1.5f;
        EditorGUILayout.LabelField($"Dégâts Critiques de Base (x1.5) : {baseCritDmg:F2}");

        if (GUILayout.Button("Valider les Équilibres"))
        {
            Debug.Log("Vérification des paramètres de combat terminée.");
        }
        // --- Fin du code personnalisé ---
        */
        if (GUI.changed)
        {
            EditorUtility.SetDirty(parameters);
        }
    }
}