using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCommander : MonoBehaviour
{
    private UnitSelection unitSelection;
    private Camera cam;

    public GameObject selectionMarkerPrefab;
    public LayerMask layerMask;

    void Awake ()
    {
        unitSelection = FindObjectOfType<UnitSelection>();
        cam = Camera.main;
    }

    void Update ()
    {
        // did we press down our right-mouse button and do we have units selected?
        if(Input.GetMouseButtonDown(1) && unitSelection.HasUnitsSelected())
        {
            // shoot a raycast from our mouse, to see what we hit
            RaycastHit hit;
            Unit[] selectedUnits = unitSelection.GetSelectedUnits();

            if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100, layerMask))
            {
                if(hit.collider != null)
                {
                    // did we click on the ground?
                    if (hit.collider.gameObject.CompareTag("Ground"))
                    {
                        UnitsMoveToPosition(hit.point, selectedUnits);
                        CreateSelectionMarker(hit.point, false);
                    }

                    // did we click on a resource?
                    else if (hit.collider.gameObject.CompareTag("Resource"))
                    {
                        UnitsGatherResource(hit.collider.GetComponent<ResourceSource>(), selectedUnits);
                        CreateSelectionMarker(hit.collider.transform.position, true);
                    }

                    // did we click on an enemy?
                    else if (hit.collider.gameObject.CompareTag("Unit"))
                    {
                        Unit enemy = hit.collider.gameObject.GetComponent<Unit>();
                        
                        if(!Player.me.IsMyUnit(enemy))
                        {
                            UnitsAttackEnemy(enemy, selectedUnits);
                            CreateSelectionMarker(enemy.transform.position, false);
                        }
                    }
                }
            }
        }
    }

    // called when we command units to move somewhere
    void UnitsMoveToPosition (Vector3 movePos, Unit[] units)
    {
        // are just selecting 1 unit?
        if (units.Length == 1)
        {
            units[0].MoveToPosition(movePos);
        }
        // otherwise, calculate the unit group formation
        else
        {
            Vector3[] destinations = UnitMover.GetUnitGroupDestinations(movePos, units, 2);

            for (int x = 0; x < units.Length; x++)
                units[x].MoveToPosition(destinations[x]);
        }
    }

    // called when we command units to gather a resource
    void UnitsGatherResource (ResourceSource resource, Unit[] units)
    {
        // are just selecting 1 unit?
        if (units.Length == 1)
        {
            units[0].GatherResource(resource, UnitMover.GetUnitDestinationAroundResource(resource.transform.position));
        }
        // otherwise, calculate the unit group formation
        else
        {
            Vector3[] destinations = UnitMover.GetUnitGroupDestinationsAroundResource(resource.transform.position, units);

            for (int x = 0; x < units.Length; x++)
                units[x].GatherResource(resource, destinations[x]);
        }
    }

    // called when we command units to attack an enemy
    void UnitsAttackEnemy (Unit target, Unit[] units)
    {
        for (int x = 0; x < units.Length; x++)
            units[x].AttackUnit(target);
    }

    void CreateSelectionMarker (Vector3 pos, bool large)
    {
        GameObject marker = Instantiate(selectionMarkerPrefab, new Vector3(pos.x, 0.01f, pos.z), Quaternion.identity);

        if(large)
            marker.transform.localScale = Vector3.one * 3;
    }
}