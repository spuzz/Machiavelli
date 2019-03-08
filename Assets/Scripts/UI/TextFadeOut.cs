using System.Collections;
using UnityEngine;
using UnityEngine.UI;
class TextFadeOut : MonoBehaviour
{
    //Fade time in seconds
    public float fadeOutTime;
    Text text;
    Color originalColor;
    public void Awake()
    {
        text = GetComponent<Text>();
        originalColor = text.color;
    }
    public void FadeOut()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine());
    }
    private IEnumerator FadeOutRoutine()
    {

        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
        {
            text.color = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b,0), Mathf.Min(1, t / fadeOutTime));
            yield return null;
        }
        gameObject.SetActive(false);
    }
}