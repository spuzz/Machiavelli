using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public string text;
    public string header;
    public bool editModeOnly = false;
    public List<KeyValuePair<int,string>> symbolWithText = new List<KeyValuePair<int, string>>();
    private HUD hud;

    private void Awake()
    {
        hud = FindObjectOfType<HUD>();
    }

    public void AddText(string text)
    {
        this.text += "<color=white>" + text + "</color><br>";
    }

    public void SetHeader(string text)
    {
        header = text;
    }

    public void AddSymbolWithText(int symbolNumber, string text)
    {
        this.text += "<color=yellow>" + "<sprite=" + symbolNumber + " >: " + text + "</color><br>";
    }

    public void Clear()
    {
        text = "";
        header = "";
        symbolWithText.Clear();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(editModeOnly == false || hud.IsInEditMode())
        {
            hud.ShowToolTip(CreateToolTipText());
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hud.HideToolTip();
    }

    private void OnDisable()
    {
        hud.HideToolTip();
    }
    public string CreateToolTipText()
    {
        string finalText = "";
        if(!string.IsNullOrEmpty(header))
        {
            finalText += "<size=24><color=yellow>" + header + "</size></color><br>";
        }
        finalText += "<size=16>" + text + "</size><br>";
        return finalText;
    }
}
