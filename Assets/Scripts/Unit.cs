using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

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

    public Player player;

    // events
    [System.Serializable]
    public class StateChangeEvent : UnityEvent<UnitState> { }
    public StateChangeEvent onStateChange;

    // components
    private NavMeshAgent navAgent;

    void Start ()
    {
        // get the components
        navAgent = GetComponent<NavMeshAgent>();

        SetState(UnitState.Idle);
    }

    // called when the unit spawns
    public void Initialize (Player owner)
    {
        player = owner;
    }

    void Update ()
    {
        switch(state)
        {
            case UnitState.Move:
                MoveUpdate();
                break;
            case UnitState.MoveToResource:
                MoveToResourceUpdate();
                break;
            case UnitState.MoveToEnemy:
                MoveToEnemyUpdate();
                break;
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

        if(onStateChange != null)
            onStateChange.Invoke(state);

        // don't move if idle
        if (toState == UnitState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }

    void MoveUpdate ()
    {
        if (Vector3.Distance(transform.position, navAgent.destination) == 0.0f)
            SetState(UnitState.Idle);
    }

    void MoveToResourceUpdate ()
    {
        if (Vector3.Distance(transform.position, navAgent.destination) == 0.0f)
            SetState(UnitState.Gather);
    }

    void MoveToEnemyUpdate ()
    {
        if (Vector3.Distance(transform.position, navAgent.destination) == 0.0f)
            SetState(UnitState.Attack);
    }

    void GatherUpdate ()
    {
        // if the resource is gone, go idle
        if(curResourceSource == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        LookAt(curResourceSource.transform.position);

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

        // navigation
        navAgent.isStopped = false;
        navAgent.SetDestination(pos);
    }

    public void GatherResource (ResourceSource resource, Vector3 pos)
    {
        // we can't gather if we're dead
        if (state == UnitState.Dead)
            return;

        curResourceSource = resource;
        SetState(UnitState.MoveToResource);

        // navigation
        navAgent.isStopped = false;
        navAgent.SetDestination(pos);
    }

    void LookAt (Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}