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
    public UnitState state;         // current state of the unit
    public int curHp;               // how much health the unit currently has
    public int maxHp;

    public int minAttackDamage;        // damage dealt to enemy units
    public int maxAttackDamage;
    public float attackRate;        // duration between attacks
    private float lastAttackTime;

    public float attackDistance;

    public float pathUpdateRate = 1.0f;
    private float lastPathUpdateTime;

    public int gatherAmount;        // amount of a resource gathered per grab
    public float gatherRate;        // duration between resource grabs
    private float lastGatherTime;

    [HideInInspector]
    public ResourceSource curResourceSource;

    private Unit curEnemyTarget;

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

        // set the initial state to 'Idle'
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
        if(toState == UnitState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }

    // called every frame the 'Move' state is active
    void MoveUpdate ()
    {
        if(Vector3.Distance(transform.position, navAgent.destination) == 0.0f)
            SetState(UnitState.Idle);
    }

    // called every frame the 'MoveToResource' state is active
    void MoveToResourceUpdate ()
    {
        // if our resource is gone, go idle
        if (curResourceSource == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        if(Vector3.Distance(transform.position, navAgent.destination) == 0.0f)
            SetState(UnitState.Gather);
    }

    // called every frame the 'MoveToEnemy' state is active
    void MoveToEnemyUpdate ()
    {
        // if our target is dead, go idle
        if (curEnemyTarget == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        if(Time.time - lastPathUpdateTime > pathUpdateRate)
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(curEnemyTarget.transform.position);
        }

        if(Vector3.Distance(transform.position, curEnemyTarget.transform.position) <= attackDistance)
            SetState(UnitState.Attack);
    }

    // called every frame the 'Gather' state is active
    void GatherUpdate ()
    {
        // if the resource is gone, go idle
        if(curResourceSource == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        // look at the resource
        LookAt(curResourceSource.transform.position);

        // gather the resource every 'gatherRate' seconds
        if(Time.time - lastGatherTime > gatherRate)
        {
            lastGatherTime = Time.time;
            curResourceSource.GatherResource(gatherAmount, player);
        }
    }

    // called every frame the 'Attack' state is active
    void AttackUpdate ()
    {
        // if our target is dead, go idle
        if (curEnemyTarget == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        // if we're still moving, stop
        if(!navAgent.isStopped)
            navAgent.isStopped = true;

        // attack every 'attackRate' seconds
        if(Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            curEnemyTarget.TakeDamage(Random.Range(minAttackDamage, maxAttackDamage));
        }

        // look at the enemy
        LookAt(curEnemyTarget.transform.position);

        // if we're too far away, move towards the enemy
        if(Vector3.Distance(transform.position, curEnemyTarget.transform.position) > attackDistance)
            SetState(UnitState.MoveToEnemy);
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
        if(state == UnitState.Dead)
            return;

        // change state
        SetState(UnitState.Move);

        // navigation
        navAgent.isStopped = false;
        navAgent.SetDestination(pos);
    }

    // move to a resource and begin to gather it
    public void GatherResource (ResourceSource resource, Vector3 pos)
    {
        // we can't gather if we're dead
        if(state == UnitState.Dead)
            return;

        // change state
        curResourceSource = resource;
        SetState(UnitState.MoveToResource);

        // navigation
        navAgent.isStopped = false;
        navAgent.SetDestination(pos);
    }

    public void AttackUnit (Unit target)
    {
        // we can't attack if we're dead
        if(state == UnitState.Dead)
            return;

        // change state
        curEnemyTarget = target;
        SetState(UnitState.MoveToEnemy);
    }

    // rotate to face the given position
    void LookAt (Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    // called when an enemy unit attacks us
    public void TakeDamage (int damage)
    {
        curHp -= damage;

        if (curHp <= 0)
            Die();

        healthBar.UpdateHealthBar(curHp, maxHp);
    }

    // called when our health reaches 0
    void Die ()
    {
        player.units.Remove(this);
        SetState(UnitState.Dead);
        Destroy(gameObject);
    }

    void OnCollisionEnter (Collision collision)
    {
        if(collision.gameObject.CompareTag("Unit"))
        {
            if(state == UnitState.MoveToResource)
            {
                if(Vector3.Distance(transform.position, curResourceSource.transform.position) < 2)
                {
                    navAgent.SetDestination(UnitMover.GetUnitDestinationAroundResource(curResourceSource.transform.position));
                }
            }
        }
    }
}