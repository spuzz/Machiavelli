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
    [SerializeField] RawImage playerColor;
    [SerializeField] Text cityName;
    [SerializeField] Text population;
    [SerializeField] City city;
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
            cityHealthBar.CityObject = city;
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

    public void SetCityStateColour(Color color)
    {
        Color cityColor = new Color(color.r, color.g, color.b, 1f);
        cityName.color = cityColor;
        population.color = cityColor;
    }

    public void SetPlayerColour(Color color)
    {
        color.a = 1;
        playerColor.color = color;
    }

    public void SetPopulation(string pop)
    {
        population.text = pop;
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
}