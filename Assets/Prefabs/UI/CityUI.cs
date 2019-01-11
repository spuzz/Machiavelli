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
    [SerializeField] RawImage unitBackground;

    [SerializeField] City city;
    Camera cameraToLookAt;

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

    bool visible = true;
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

    private void Awake()
    {
        cameraToLookAt = Camera.main;
        cityHealthBar.CityObject = city;
    }



    // Update is called once per frame 
    void LateUpdate()
    {
        transform.position = new Vector3(city.GetHexCell().transform.position.x, city.GetHexCell().transform.position.y + 14, city.GetHexCell().transform.position.z);
        transform.LookAt(cameraToLookAt.transform);
        transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
        transform.Translate(new Vector3(0, 0, 6));

    }

    public void SetColour(Color color)
    {
        unitBackground.color = new Color(color.r, color.g, color.b, 0.5f);
    }

    public void SetBackground(Texture background)
    {
        unitBackground.texture = background;
    }


    public void UpdateHealthBar()
    {
        if (cityHealthBar)
        {
            cityHealthBar.UpdateHealth();
        }

    }
}