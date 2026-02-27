using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
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

    public void DisableShadows()
    {

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
}
