using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Add a UI Socket transform to your enemy
// Attack this script to the socket
// Link to a canvas prefab that contains NPC UI
public class CityUI : MonoBehaviour
{

    // Works around Unity 5.5's lack of nested prefabs
    [Tooltip("The UI canvas prefab")]
    [SerializeField] Canvas canvas;
    [SerializeField] CityHealthBar cityHealthBar;
    [SerializeField] Image cityStateSymbol;
    [SerializeField] Text cityName;
    [SerializeField] Text population;
    [SerializeField] City city;
    [SerializeField] RawImage backGround;
    [SerializeField] ToolTip toolTip;
    [SerializeField] Image happiness;
    [SerializeField] TextMeshProUGUI loyalPoliticians;
    [SerializeField] TextMeshProUGUI falteringPoliticians;
    [SerializeField] TextMeshProUGUI rivalPoliticians;

    [SerializeField] Sprite happy;
    [SerializeField] Sprite content;
    [SerializeField] Sprite unhappy;

    Camera cameraToLookAt;
    HexGameUI hexGameUI;
    bool visible = true;
    public City CityObject
    {
        get
        {
            return city;
        }

        set
        {
            city = value;
            UpdateInfo(city);
        }
    }

    private void OnEnable()
    {
        city.onInfoChange += UpdateInfo;
        UpdateInfo(city);
    }

    private void OnDisable()
    {
        city.onInfoChange -= UpdateInfo;
        UpdateInfo(city);
    }

    public bool Visible
    {
        get
        {
            return visible;
        }

        set
        {
            visible = value;
            canvas.enabled = value;
        }
    }

    public Image CityStateSymbol
    {
        get
        {
            return cityStateSymbol;
        }

        set
        {
            cityStateSymbol = value;
        }
    }

    public void SetPlayerColour(Color color)
    {
        Color cityColor = new Color(color.r, color.g, color.b, 0.6f);
        backGround.color = cityColor;
        if (color == Color.black)
        {
            cityColor = Color.grey;
        }
        cityStateSymbol.color = cityColor;
    }


    private void Awake()
    {
        cameraToLookAt = Camera.main;
        cityHealthBar.CityObject = city;
        hexGameUI = FindObjectOfType<HexGameUI>();

    }

    private void Start()
    {
        
    }

    // Update is called once per frame 
    void LateUpdate()
    {
        transform.position = new Vector3(city.GetHexCell().transform.position.x, city.GetHexCell().transform.position.y + 20, city.GetHexCell().transform.position.z);
        transform.LookAt(cameraToLookAt.transform);
        transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
        transform.Translate(new Vector3(0, 0, 6));

    }

    public void UpdateHealthBar()
    {
        if (cityHealthBar)
        {
            cityHealthBar.UpdateHealth();
        }

    }

    public void SelectCity()
    {
        hexGameUI.SelectCity(city);
    }

    public void UpdateInfo(City city)
    {
        toolTip.Clear();
        toolTip.SetHeader("City Info");
        toolTip.AddText("");
        population.text = city.Population.ToString();

        UpdateLoyalty(city);

        UpdateHappiness(city);
    }

    private void UpdateHappiness(City city)
    {
        if (city.CityResouceController.GetHappiness() < 0)
        {
            happiness.sprite = unhappy;
        }
        else if (city.CityResouceController.GetHappiness() == 0)
        {
            happiness.sprite = content;
        }
        else if (city.CityResouceController.GetHappiness() > 0)
        {
            happiness.sprite = happy;
        }
    }

    private void UpdateLoyalty(City city)
    {
        int loyalPol = 0;
        int falteringPol = 0;
        int rivalPol = 0;

        foreach (Politician pol in city.GetCityState().GetPoliticians())
        {
            if (pol.ControllingPlayer)
            {
                if (pol.ControllingPlayer.IsHuman)
                {
                    if (pol.Loyalty >= 50)
                    {
                        loyalPol += 1;
                    }
                    else
                    {
                        falteringPol += 1;
                    }
                }
            }
            else
            {
                rivalPol += 1;
            }
        }

        loyalPoliticians.text = loyalPol.ToString();
        falteringPoliticians.text = falteringPol.ToString();
        rivalPoliticians.text = rivalPol.ToString();
    }
}