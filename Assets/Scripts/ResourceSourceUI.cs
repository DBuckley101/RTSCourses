using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// !!! RENAME to ResourceSourcePopUp !!!
public class ResourceSourceUI : MonoBehaviour
{
    public GameObject popUpPanel;
    public TextMeshProUGUI resourceQuantityText;
    public ResourceSource resource;

    void OnMouseEnter ()
    {
        popUpPanel.SetActive(true);
    }

    void OnMouseExit ()
    {
        popUpPanel.SetActive(false);
    }

    public void OnResourceQuantityChange ()
    {
        resourceQuantityText.text = resource.quantity.ToString();
    }
}