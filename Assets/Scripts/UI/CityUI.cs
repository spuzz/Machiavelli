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
            if(city)
            {
                city.onInfoChange -= UpdateInfo;
            }
            city = value;
            if (city)
            {
                city.onInfoChange += UpdateInfo;
                cityHealthBar.CityObject = city;
            }
            UpdateInfo(city);
        }
    }

    private void OnEnable()
    {
        if(city)
        {
            city.onInfoChange += UpdateInfo;
            UpdateInfo(city);
        }
        
    }

    private void OnDisable()
    {
        if(city)
        {
            city.onInfoChange -= UpdateInfo;
        }
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
    }


    private void Awake()
    {
        cameraToLookAt = Camera.main;
        cityHealthBar.CityObject = city;
        hexGameUI = FindObjectOfType<HexGameUI>();
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

    }
}