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
        DialogTextBoxGameObject.SetActive(!DialogTextBoxGameObject.activeInHierarchy);
    }

    public void DisplayDialogAndTextForGivenAmountOfTime(float waitTimeInSeconds, bool isGameMustBePaused, string textToDisplay)
    {
        SetDialogTextBox(textToDisplay);
        ToggleDialogTextBox();
        this.WaitForGivenAmountOfTime(waitTimeInSeconds, isGameMustBePaused, DialogTextBoxGameObject.activeInHierarchy);
        
    }

    public void WaitForGivenAmountOfTime(float waitTimeInSeconds, bool isGameMustBePaused, bool isDialogDisplayToBeShutdown = false)
    {
        StartCoroutine(GenericWaitBehavior(waitTimeInSeconds, isGameMustBePaused, isDialogDisplayToBeShutdown));
    }

    private IEnumerator GenericWaitBehavior(float waitTimeInSeconds, bool isGameMustBePaused = false, bool isDialogDisplayToBeShutdown = false)
    {
        GameData.isPaused = isGameMustBePaused;
        yield return new WaitForSeconds(waitTimeInSeconds);

        if (isGameMustBePaused)
        {
            GameData.isPaused = !GameData.isPaused;
        }

        if (isDialogDisplayToBeShutdown)
        {
            ToggleDialogTextBox();
        }
    }

    public GameObject DialogTextBoxGameObject { get => dialogTextBoxGameObject; set => dialogTextBoxGameObject = value; }
}
