using UnityEngine;
using UnityEngine.UI;

// Add a UI Socket transform to your enemy
// Attack this script to the socket
// Link to a canvas prefab that contains NPC UI
public class UnitUI : MonoBehaviour {

    // Works around Unity 5.5's lack of nested prefabs
    [Tooltip("The UI canvas prefab")]
    [SerializeField] Canvas canvas;
    [SerializeField] RawImage unitBackground;
    [SerializeField] UnitHealthBar unitHealthBar;
    [SerializeField] RawImage unitSymbol;

    [SerializeField] Texture defaultSymbol;
    [SerializeField] Texture defaultBackGround;

    Unit unit;
    Camera cameraToLookAt;

    public Unit Unit
    {
        get
        {
            return unit;
        }

        set
        {
            unit = value;
            unitHealthBar.Unit = unit;
            if (unit.BackGround)
            {
                SetBackground(unit.BackGround);
            }

            if (unit.Symbol)
            {
                SetSymbol(unit.Symbol);
            }
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
        SetBackground(defaultBackGround);
        SetSymbol(defaultSymbol);
    }



    // Update is called once per frame 
    void LateUpdate()
    {
        transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y + 14, unit.transform.position.z);
        transform.LookAt(cameraToLookAt.transform);
        transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
        transform.Translate(new Vector3(0, 0, 6));
        
    }

    public void SetColour(Color color)
    {
        unitBackground.color = new Color(color.r,color.g,color.b,1);
    }

    public void SetSymbol(Texture symbol)
    {
        unitSymbol.texture = symbol;
    }

    public void SetBackground(Texture background)
    {
        unitBackground.texture = background;
    }


    public void UpdateHealthBar()
    {
        if(unitHealthBar)
        {
            unitHealthBar.UpdateHealth();
        }
        
    }
}