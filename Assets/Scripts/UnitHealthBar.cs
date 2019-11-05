using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    public GameObject healthContainer;
    public RectTransform healthFill;

    private float maxSize;

    private Camera cam;

    void Awake ()
    {
        // get the components
        cam = Camera.main;

        maxSize = healthFill.sizeDelta.x;
        healthContainer.SetActive(false);
    }

    // called when the unit's health is modified
    public void UpdateHealthBar (int curHp, int maxHp)
    {
        healthContainer.SetActive(true);
        float healthPercent = (float)curHp / (float)maxHp;
        healthFill.sizeDelta = new Vector2(maxSize * healthPercent, healthFill.sizeDelta.y);
    }
}