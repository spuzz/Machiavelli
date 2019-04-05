using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCellTextEffectHandler : MonoBehaviour {

    [SerializeField] GameObject textEffectPrefab;
    [SerializeField] float delayTime = 0.5f;
    float delay = 0;
    Queue<HexCellTextEffect> textEffects = new Queue<HexCellTextEffect>();
    public void AddTextEffect(string newText, Transform transform, Color color, float newDuration = 0)
    {
        HexCellTextEffect effect = Instantiate(textEffectPrefab).GetComponent<HexCellTextEffect>();
        effect.Show(newText, transform, color, newDuration);
        textEffects.Enqueue(effect);
    }

    private void Update()
    {
        delay -= Time.deltaTime;
        if(textEffects.Count > 0 && delay < 0)
        {
            HexCellTextEffect effect = textEffects.Dequeue();
            effect.Play();
            delay = delayTime;
        }
        
    }
}
