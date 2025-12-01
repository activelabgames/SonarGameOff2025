using TMPro;
using UnityEngine;

public class VariableTMP : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private BaseVariableSO variable;

    private void Awake()
    {
        if (tmpText == null)
        {
            tmpText = GetComponent<TMP_Text>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Debug.Log("Setting tmpText text");
        tmpText.text = "" + variable;
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log("Updating tmpText text");
        tmpText.text = "" + variable;
    }
}