using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum UnitState
{
    Idle,
    Move,
    MoveToResource,
    MoveToEnemy,
    Gather,
    Attack,
    Dead
}

public class Unit : MonoBehaviour
{
    [Header("Stats")]
    public UnitState state;     // current state of the unit
    public int curHp;           // how much health the unit currently has
    public int maxHp;

    public int attackDamage;    // damage dealt to enemy units
    public float attackRate;    // duration between attacks
    private float lastAttackTime;

    public int gatherAmount;    // amount of a resource gathered per grab
    public float gatherRate;    // duration between resource grabs
    private float lastGatherTime;
    private ResourceSource curResourceSource;

    [Header("Components")]
    public GameObject selectionVisual;
    public UnitHealthBar healthBar;

    private Player player;

    // components
    private NavMeshAgent navAgent;

    void Awake ()
    {
        // get the components
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update ()
    {
        switch(state)
        {
            case UnitState.Gather:
                GatherUpdate();
                break;
            case UnitState.Attack:
                AttackUpdate();
                break;
        }
    }

    // set's our current state
    void SetState (UnitState toState)
    {
        state = toState;
    }

    void GatherUpdate ()
    {
        // if the resource is gone, go idle
        if(curResourceSource == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        if(Time.time - lastGatherTime > gatherRate)
        {
            lastGatherTime = Time.time;
            curResourceSource.GatherResource(gatherAmount, player);
        }
    }

    void AttackUpdate ()
    {

    }

    // toggles the selection ring around our feet
    public void ToggleSelectionVisual (bool selected)
    {
        selectionVisual.SetActive(selected);
    }

    // moves the unit to a specific position
    public void MoveToPosition (Vector3 pos)
    {
        // we can't move if we're dead
        if (state == UnitState.Dead)
            return;

        SetState(UnitState.Move);
        navAgent.SetDestination(pos);
    }

    public void GatherResource (ResourceSource resource)
    {

    }
}