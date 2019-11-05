using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public bool isMe;

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

    public static Player me;

    void Awake ()
    {
        if (isMe)
            me = this;
    }

    void Start ()
    {
        // set the game ui text
        if (isMe)
        {
            GameUI.instance.UpdateUnitCountText(units.Count);
            GameUI.instance.UpdateFoodText(food);

            CameraController.instance.FocusOnPosition(unitSpawnPos.position);
        }

        // create the initial unit
        food += unitCost;
        CreateNewUnit();
    }

    // called when a unit gathers a certain resource
    public void GainResource (ResourceType resourceType, int amount)
    {
        switch(resourceType)
        {
            case ResourceType.Food:
            {
                food += amount;
                
                if(isMe)
                    GameUI.instance.UpdateFoodText(food);

                break;
            }
        }
    }

    // creates a new unit for the player
    public void CreateNewUnit()
    {
        if (food - unitCost < 0)
            return;

        GameObject unitObj = Instantiate(unitPrefab, unitSpawnPos.position, Quaternion.identity, transform);
        Unit unit = unitObj.GetComponent<Unit>();

        unit.Initialize(this);

        units.Add(unit);
        food -= unitCost;

        if (onUnitCreated != null)
            onUnitCreated.Invoke(unit);

        if (isMe)
        {
            GameUI.instance.UpdateUnitCountText(units.Count);
            GameUI.instance.UpdateFoodText(food);
        }
    }
    // is this my unit?
    public bool IsMyUnit (Unit unit)
    {
        return units.Contains(unit);
    }
}