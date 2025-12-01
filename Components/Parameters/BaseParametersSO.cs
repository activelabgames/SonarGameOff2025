using UnityEngine;

// Assurez-vous que tous vos paramètres en héritent
public abstract class BaseParametersSO : ScriptableObject
{
    // Vous pouvez placer ici des propriétés communes si nécessaire
    // Par exemple, une description pour l'éditeur
    [TextArea(1, 3)]
    public string EditorDescription = "Paramètres d'équilibrage pour ce système.";
}