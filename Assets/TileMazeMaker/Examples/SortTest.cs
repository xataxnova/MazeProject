using TileMazeMaker;
using System.Collections.Generic;
using UnityEngine;

public class SortTest : MonoBehaviour
{
	// Use this for initialization
	void Start () {
        int[] ia = { 3, 6, 7, 8, 9, 4, 2, 0, 9 };

        List<int> iArray = new List<int>(ia);
        LogArray(iArray);
        //SortAlgorithm.BubbleSortInt(iArray);
        SortAlgorithm.QuickSortInt(iArray);
        LogArray(iArray);

        Vector3[] v3 = new Vector3[3];
        v3[0] = new Vector3(1, 0, 0);
        v3[1] = new Vector3(0, 1, 0);
        v3[2] = new Vector3(0, 0, 1);
        Matrix mat = new Matrix(v3);
        Debug.Log(mat);

        

    }

    public void LogArray(List<int> i_array)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var i in i_array)
        {
            sb.Append(i);
            sb.Append(",");             
        }
        sb.Remove(sb.Length - 1, 1);
        Debug.Log(sb.ToString());           
    }
	
}
