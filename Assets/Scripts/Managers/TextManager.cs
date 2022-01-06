using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{

    public GameObject textModel;
    public RectTransform canvasRectTransform;
    public float speed;
    public Vector3 direction;
    private string textContent;
    private Color textColor;
    private float fadeTime;
    private bool isAnimated;

    public void CreateText(Vector3 position, string text, Color color, float fadeTime, bool isAnimated)
    {
        GameObject textObject = Instantiate(textModel, position, Quaternion.identity);
        textObject.transform.SetParent(canvasRectTransform);
        textObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        textObject.GetComponent<TextController>().Initialize(speed, direction, fadeTime, isAnimated);
        textObject.GetComponent<TextMeshProUGUI>().text = text;
        textObject.GetComponent<TextMeshProUGUI>().color = color;
    }

    public string TextContent { get => textContent; set => textContent = value; }
    public Color TextColor { get => textColor; set => textColor = value; }
    public float FadeTime { get => fadeTime; set => fadeTime = value; }
    public bool IsAnimated { get => isAnimated; set => isAnimated = value; }
}
