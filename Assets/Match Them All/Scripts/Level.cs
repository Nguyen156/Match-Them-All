using UnityEngine;

public class Level : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private ItemPlacer itemPlacer;

    [Header(" Settings ")]
    [SerializeField] private int duration;
    public int Duration => this.duration;


    public ItemLevelData[] GetGoals() => itemPlacer.GetGoals();

    public Item[] GetItems() => itemPlacer.GetItems();
   
}
