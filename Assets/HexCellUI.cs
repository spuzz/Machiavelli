using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HexCellUI : MonoBehaviour {

    [SerializeField] HexCell hexCell;
    [SerializeField] Canvas canvas;
    [SerializeField] Canvas toggleCanvas;
    [SerializeField] Toggle toggle;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI foodText;
    [SerializeField] TextMeshProUGUI scienceText;
    [SerializeField] TextMeshProUGUI productionText;
    City currentCityOwner;

    public City CurrentCityOwner
    {
        get
        {
            return currentCityOwner;
        }

        set
        {
            currentCityOwner = value;
        }
    }

    private void Awake()
    {

    }

    void LateUpdate()
    {
        transform.position = new Vector3(hexCell.transform.position.x, hexCell.transform.position.y + 1, hexCell.transform.position.z);

    }

    public void EnableCanvas(bool enable)
    {
        canvas.gameObject.SetActive(enable);
        toggleCanvas.gameObject.SetActive(enable);
    }

    public void UpdateUI()
    {
        HexCellGameData gameData = hexCell.GetComponent<HexCellGameData>();
        productionText.text = gameData.Production.ToString();
        foodText.text = gameData.Food.ToString();
        scienceText.text = gameData.Science.ToString();
        goldText.text = gameData.Gold.ToString();
        
    }

    public void SetWorked(bool worked)
    {
        toggle.isOn = worked;

    }

    public void SetToggleLocked(bool locked)
    {
        if(locked)
        {
            toggle.interactable = false;
            toggle.graphic.color = Color.blue;
        }
        else
        {
            toggle.interactable = true;
            toggle.graphic.color = Color.green;
        }
    }

    public void ToggleClicked(bool selected)
    {
        GameObject clickedToggle = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (clickedToggle == null)
        {
            return;
        }

        if (currentCityOwner)
        {
            if (selected == false)
            {
                currentCityOwner.RemoveWorkedCell(gameObject.GetComponentInParent<HexCell>());
            }
            else
            {
                if (CurrentCityOwner.CanWorkAnotherCell())
                {
                    currentCityOwner.AddWorkedCell(gameObject.GetComponentInParent<HexCell>());
                    toggle.isOn = true;
                }
                else
                {
                    toggle.isOn = false;
                }
            }
        }


    }
}
