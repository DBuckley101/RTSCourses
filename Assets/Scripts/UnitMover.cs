using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    public static Vector3[] GetUnitGroupDestinations (Vector3 moveToPos, Unit[] units, float unitGap)
    {
        Vector3[] destinations = new Vector3[units.Length];

        int rows = Mathf.RoundToInt(Mathf.Sqrt(units.Length));
        int cols = Mathf.CeilToInt((float)units.Length / (float)rows);

        int curRow = 0;
        int curCol = 0;
        
        float width = ((float)rows - 1) * unitGap;
        float length = ((float)cols - 1) * unitGap;

        for(int x = 0; x < units.Length; x++)
        {
            destinations[x] = moveToPos + (new Vector3(curRow, 0, curCol) * unitGap) - new Vector3(length / 2, 0, width / 2);
            curCol++;

            if(curCol == rows)
            {
                curCol = 0;
                curRow++;
            }
        }

        return destinations;
    }

    public static Vector3[] GetUnitGroupDestinationsAroundResource (Vector3 resourcePos, Unit[] units)
    {
        Vector3[] destinations = new Vector3[units.Length];

        float unitDistanceGap = 360.0f / (float)units.Length;

        for(int x = 0; x < units.Length; x++)
        {
            float angle = unitDistanceGap * x;
            Vector3 dir = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
            destinations[x] = resourcePos + dir;
        }

        return destinations;
    }
}