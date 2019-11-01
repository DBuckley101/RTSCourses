using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [Header("Units")]
    public List<Unit> units = new List<Unit>();

    [Header("Resources")]
    public int food;

    [Header("Components")]
    public GameObject unitPrefab;
    public Transform unitSpawnPos;

    // events
    [System.Serializable]
    public class UnitCreatedEvent : UnityEvent<Unit> { }
    public UnitCreatedEvent onUnitCreated;

    public static int unitCost = 10;

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

    // creates a new unit for the player
    public void CreateNewUnit ()
    {
        if(food - unitCost < 0)
            return;

        GameObject unitObj = Instantiate(unitPrefab, unitSpawnPos.position, Quaternion.identity);
        Unit unit = unitObj.GetComponent<Unit>();

        unit.Initialize(this);

        units.Add(unit);
        food -= unitCost;

        if(onUnitCreated != null)
            onUnitCreated.Invoke(unit);
    }
}