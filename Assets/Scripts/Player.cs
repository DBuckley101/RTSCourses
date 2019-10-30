using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Units")]
    public List<Unit> units = new List<Unit>();

    [Header("Resources")]
    public int food;

    // called when a unit gathers a certain resource
    public void GainResource (ResourceType resourceType, int amount)
    {
        switch(resourceType)
        {
            case ResourceType.Food:
                food += amount;
                break;
        }
    }
}