using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public float checkRate = 1.0f;
    private ResourceSource[] resources;

    private Player player;

    void Awake ()
    {
        player = GetComponent<Player>();
    }

    void Start ()
    {
        InvokeRepeating("Check", 0.0f, checkRate);
        resources = GameObject.FindObjectsOfType<ResourceSource>();
    }

    void Check ()
    {
        // if we can create a new unit, do so
        if (player.food >= Player.unitCost)
            player.CreateNewUnit();
    }

    public void OnUnitCreated (Unit unit)
    {
        unit.GetComponent<UnitAI>().InitializeAI(this, unit);
    }

    public ResourceSource GetClosestResource (Vector3 pos)
    {
        ResourceSource[] closest = new ResourceSource[3];
        float[] closestDist = new float[3];
        //ResourceSource closest = null;
        //float closestDist = 0.0f;

        foreach(ResourceSource resource in resources)
        {
            if(resource != null)
            {
                float dist = Vector3.Distance(pos, resource.transform.position);

                for(int x = 0; x < closest.Length; x++)
                {
                    if(closest[x] == null)
                    {
                        closest[x] = resource;
                        closestDist[x] = dist;
                        break;
                    }
                    else
                    {
                        if(dist < closestDist[x])
                        {
                            closest[x] = resource;
                            closestDist[x] = dist;
                            break;
                        }
                    }
                }
            }
        }

        return closest[Random.Range(0, closest.Length)];
    }
}