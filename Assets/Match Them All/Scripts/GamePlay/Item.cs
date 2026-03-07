using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    [Header(" Data ")]
    [SerializeField] private EItemName itemName;
    public EItemName ItemName => this.itemName;

    private Spot spot;
    public Spot Spot => this.spot;

    [SerializeField] private Sprite icon;
    public Sprite Icon => this.icon;

    [Header(" Elements ")]
    [SerializeField] private Material outlineMat;
    private Material baseMat;

    private Rigidbody rb;
    private Collider cd;
    private Renderer rd;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cd = GetComponentInChildren<Collider>();
        rd = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        baseMat = rd.material;
    }

    public void AssignSpot(Spot spot) => this.spot = spot;

    public void UnassignSpot() => spot = null;

    public void EnableShadow()
    {
        rd.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    public void DisableShadows()
    {
        rd.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void EnablePhysics()
    {
        rb.isKinematic = false;
        cd.enabled = true;
    }

    public void DisablePhysics()
    {
        rb.isKinematic = true;
        cd.enabled = false;
    }

    public void Select()
    {
        rd.materials = new Material[] { baseMat, outlineMat};
    }

    public void Deselect()
    {
        rd.materials = new Material[] { baseMat };
    }

    public void ApplyRandomForce(float magnitude)
        => rb.AddForce(Random.onUnitSphere * magnitude, ForceMode.VelocityChange);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .02f);
    }
}
