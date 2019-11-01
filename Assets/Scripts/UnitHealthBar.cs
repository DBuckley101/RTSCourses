using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    public RectTransform healthFill;

    private float maxSize;

    private Camera cam;

    void Awake ()
    {
        // get the components
        cam = Camera.main;

        maxSize = healthFill.sizeDelta.x;
    }

    // called when the unit's health is modified
    public void UpdateHealthBar (int curHp, int maxHp)
    {
        float healthPercent = (float)curHp / (float)maxHp;
        healthFill.sizeDelta = new Vector2(maxSize * healthPercent, healthFill.sizeDelta.y);
    }
}