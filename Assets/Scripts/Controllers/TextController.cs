using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{

    private float speed;
    private Vector3 direction;
    private float fadeTime;
    public AnimationClip textAnimationClip;
    private bool isTextAnimated;

    // Update is called once per frame
    void Update()
    {
        if (!this.isTextAnimated)
        {
            float translation = speed * Time.deltaTime;

            transform.Translate(direction * translation);
        }
        
    }

    public void Initialize(float speed, Vector3 direction, float fadeTime, bool isTextAnimated)
    {
        this.speed = speed;
        this.direction = direction;
        this.fadeTime = fadeTime;
        this.isTextAnimated = isTextAnimated;
        if (this.isTextAnimated)
        {
            this.GetComponent<Animator>().SetTrigger("Animated");
            this.StartCoroutine(AnimateText());
        }
        else
        {
            this.StartCoroutine(FadeOut());
        }
        
    }

    private IEnumerator AnimateText()
    {
        yield return new WaitForSeconds(textAnimationClip.length);
        this.isTextAnimated = false;
        this.StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = this.GetComponent<TextMeshProUGUI>().color.a;

        float rate = 1.0f / fadeTime;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color temporaryColor = this.GetComponent<TextMeshProUGUI>().color;
            this.GetComponent<TextMeshProUGUI>().color = new Color(temporaryColor.r, temporaryColor.g, temporaryColor.b, Mathf.Lerp(startAlpha, 0, progress));

            progress += rate * Time.deltaTime;

            yield return null;
        }

        Destroy(this.gameObject);
    }
}
