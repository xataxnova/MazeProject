using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SortAlgorithm
{
    /// <summary>
    /// Worst time O(N^2)
    /// Average time O(N^2)
    /// Stable Sort( if we have iv and ir, iv is before ir, 
    /// after sort, iv is till before ir, the sort algorithm is stable )
    /// Space O(1)
    /// </summary>
    /// <param name="int_array">Input int array </param>
    public static void BubbleSortInt(List<int> int_array)
    {
        for (int i = 0; i < int_array.Count - 1; i++)
        {
            for (int j = 0; j < int_array.Count - 1 - i; j++)
            {
                //swap
                if (int_array[j] > int_array[j + 1])
                {
                    int temp = int_array[j + 1];
                    int_array[j + 1] = int_array[j];
                    int_array[j] = temp;
                }
            }
        }
    }


    public static void QuickSortInt(List<int> iarray)
    {
        int low = 0;
        int high = iarray.Count-1;
        if (iarray.Count > 1)
        {
            QS_Sort(iarray, low, high);
        }
    }

    private static void QS_Sort(List<int> iarray, int low, int high)
    {
        if (low >= high)
            return; //递归终止条件
        //完成一轮排序，index本身的位置是最开始的那个Key值，不需要再参与排序了。
        int index = QS_SortUnit(iarray, low, high);
        //index左边继续排
        QS_Sort(iarray, low, index-1);
        //index右边继续排
        QS_Sort(iarray, index + 1, high);
    }

    private static int QS_SortUnit(List<int> iarray, int low, int high)
    {
        int key = iarray[low];

        while (low < high)
        {
            //这里一定是大于等于，不然要死循环的。
            while (iarray[high] >= key && high > low)
            {
                --high;
            }
            iarray[low] = iarray[high];

            while (iarray[low] <= key && high > low)
            {
                ++low;
            }
            iarray[high] = iarray[low];
        }

        iarray[low] = key;
        return high;
    }
}
