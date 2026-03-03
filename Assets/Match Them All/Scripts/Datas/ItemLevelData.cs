using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public struct ItemLevelData
{
    public Item itemPrefab;
    public bool isGoal;

    [NaughtyAttributes.ValidateInput("ValidateAmount", "Amount must be multiple of 3")]
    [NaughtyAttributes.AllowNesting]
    public int amount;

    private bool ValidateAmount()
    {
        return amount % 3 == 0;
    }
}
   
