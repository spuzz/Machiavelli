using UnityEngine;
using UnityEngine.UI;

// Add a UI Socket transform to your enemy
// Attack this script to the socket
// Link to a canvas prefab that contains NPC UI
public class OpCentreUI : MonoBehaviour
{

    [SerializeField] Canvas canvas;
    [SerializeField] Image background;
    [SerializeField] OperationCentre opCentre;
    HexGameUI hexGameUI;
    Camera cameraToLookAt;

    bool visible = true;
    public OperationCentre OpCentre
    {
        get
        {
            return opCentre;
        }

        set
        {
            opCentre = value;
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
    public void SetPlayerColour(Color color)
    {
        color.a = 1;
        background.color = color;
    }


    private void Awake()
    {
        cameraToLookAt = Camera.main;
        hexGameUI = FindObjectOfType<HexGameUI>();
    }

    public void SelectOpCentre()
    {
        hexGameUI.SelectOpCentre(opCentre);
    }
    // Update is called once per frame 
    void LateUpdate()
    {
        transform.position = new Vector3(opCentre.Location.transform.position.x, opCentre.Location.transform.position.y + 20, opCentre.Location.transform.position.z);
        transform.LookAt(cameraToLookAt.transform);
        transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
        transform.Translate(new Vector3(0, 0, 6));

    }
}