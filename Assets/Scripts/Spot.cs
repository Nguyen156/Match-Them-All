using UnityEngine;

public class Spot : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform itemParent;
    private Animator anim;

    [Header(" Settings ")]
    private Item item;
    public Item Item => this.item;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Populate(Item item)
    {
        this.item = item;
        item.transform.SetParent(itemParent);

        item.AssignSpot(this);
    }

    public void Clear() => item = null;

    public void BumpDown()
    {
        anim.Play("Bump", 0, 0);
    }

    public bool IsEmpty() => item == null;
    
}
