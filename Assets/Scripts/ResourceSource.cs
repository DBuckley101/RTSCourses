using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Food
}

public class ResourceSource : MonoBehaviour
{
    public ResourceType type;
    public int quantity;

    // called when a unit gathers the resource
    public void GatherResource (int amount, Player gatheringPlayer)
    {
        quantity -= amount;

        int amountToGive = amount;

        // make sure we don't give more than we have
        if (quantity < 0)
            amountToGive = amount + quantity;

        gatheringPlayer.GainResource(type, amountToGive);
    }
}