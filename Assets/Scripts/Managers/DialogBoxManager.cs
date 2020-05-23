using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBoxManager : MonoBehaviour
{
    [SerializeField]
    public GameObject dialogTextBoxModel;
    private GameObject dialogTextBoxGameObject;

    // Start is called before the first frame update
    void Start()
    {
        DialogTextBoxGameObject = Instantiate(dialogTextBoxModel, this.gameObject.transform.position, Quaternion.identity);
        DialogTextBoxGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDialogTextBox(string newText)
    {
        Canvas canvas = DialogTextBoxGameObject.GetComponentInChildren<Canvas>();
        RectTransform rectTransform = canvas.gameObject.GetComponentInChildren<RectTransform>();
        TextMeshProUGUI textMeshProUGUI = rectTransform.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        textMeshProUGUI.text = newText;
    }


    public void ToggleDialogTextBox()
    {
        DialogTextBoxGameObject.SetActive(!DialogTextBoxGameObject.activeSelf);
    }

    public GameObject DialogTextBoxGameObject { get => dialogTextBoxGameObject; set => dialogTextBoxGameObject = value; }
}
