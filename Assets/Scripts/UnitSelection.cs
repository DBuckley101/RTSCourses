using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public RectTransform selectionBox;
    public Player player;

    public LayerMask unitLayerMask;

    private List<Unit> selectedUnits = new List<Unit>();
    private Vector2 startPos;
    private bool mouseDown;

    private Camera cam;
     
    void Awake ()
    {
        cam = Camera.main;
    }

    void Update ()
    {
        // mouse down
        if(Input.GetMouseButtonDown(0))
        {
            ToggleSelectionVisual(false);
            selectedUnits = new List<Unit>();

            TrySelect(Input.mousePosition);
            startPos = Input.mousePosition;
            mouseDown = true;
        }

        // mouse up
        if(Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            ReleaseSelectionBox();
        }

        // are we holding down the mouse click?
        if(mouseDown)
            UpdateSelectionBox(Input.mousePosition);
    }

    void TrySelect (Vector2 screenPos)
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100, unitLayerMask))
        {
            selectedUnits.Add(hit.collider.GetComponent<Unit>());
        }

        ToggleSelectionVisual(true);
    }

    void UpdateSelectionBox (Vector2 curMousePos)
    {
        // enable the selection box
        if (!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        float width = curMousePos.x - startPos.x;
        float height = curMousePos.y - startPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);
    }

    void ReleaseSelectionBox ()
    {
        selectionBox.gameObject.SetActive(false);

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach (Unit unit in player.units)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
            {
                selectedUnits.Add(unit);
            }
        }

        ToggleSelectionVisual(true);
    }

    void ToggleSelectionVisual (bool selected)
    {
        foreach (Unit unit in selectedUnits)
        {
            unit.ToggleSelectionVisual(selected);
        }
    }

    // returns whether or not we're selecting a unit or units
    public bool HasUnitsSelected ()
    {
        return selectedUnits.Count > 0 ? true : false;
    }

    // returns the selected units
    public Unit[] GetSelectedUnits ()
    {
        return selectedUnits.ToArray();
    }
}