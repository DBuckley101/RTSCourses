using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStateBubble : MonoBehaviour
{
    public Image stateBubble;

    [Header("Sprites")]
    public Sprite idleState;
    public Sprite gatherSprite;
    public Sprite attackSprite;

    // called when our state changes
    public void OnStateChange (UnitState state)
    {
        stateBubble.enabled = true;

        switch(state)
        {
            case UnitState.Idle:
                stateBubble.sprite = idleState;
                break;
            case UnitState.Gather:
                stateBubble.sprite = gatherSprite;
                break;
            case UnitState.Attack:
                stateBubble.sprite = attackSprite;
                break;
            default:
                stateBubble.enabled = false;
                break;
        }
    }
}