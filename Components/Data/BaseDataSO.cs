using UnityEngine;

namespace Sonar
{
    public abstract class BaseDataSO : ScriptableObject
    {
        // Vous pouvez placer ici des propriétés communes si nécessaire
        // Par exemple, une description pour l'éditeur
        [TextArea(1, 3)]
        public string EditorDescription = "Données transitoires pour ce système.";
    }
}

