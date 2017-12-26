using UnityEngine;

namespace TileMazeMaker
{
    public class Matrix3x3
    {
        const float EPISILON = 0.000001f;
        const int DataCount = 9;
        const int RowCount = 3;//RowCount is Equal to ColCount!        
        static float[] FloatIdentity = { 1, 0, 0, 0, 1, 0, 0, 0, 1 };
        static float[] FloatZero = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static Matrix3x3 sm_Identity = new Matrix3x3(FloatIdentity);
        static Matrix3x3 sm_Zero = new Matrix3x3(FloatZero);

        public static Matrix3x3 Identity
        {
            get
            {
                return sm_Identity;
            }
        }

        public static Matrix3x3 Zero
        {
            get
            {
                return sm_Zero;
            }
        }

        //use float array, do not use vector3 to store data
        //cause it is far too slow.
        float[] m_Data = new float[DataCount];

        //HackMe:if too slow inline this.
        bool IsZero(float var)
        {
            return var > -EPISILON && var < EPISILON;
        }

        public float this[int row_index, int col_index]
        {
            get
            {
                return m_Data[row_index * RowCount + col_index];
            }
            set
            {
                m_Data[row_index * RowCount + col_index] = value;
            }
        }

        public Matrix3x3(Vector3[] vecs)
        {
            if (vecs.Length == RowCount)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    for (int index = 0; index < RowCount; index++)
                    {
                        m_Data[i * RowCount + index] = vecs[i][index];
                    }
                }
            }
            else
            {
                Debug.LogError("Error Vector3 size is not equal to 3");
            }
        }

        /// <summary>
        /// 用数组初始化矩阵，可以选择是否深度拷贝。
        /// </summary>
        /// <param name="in_data"></param>
        /// <param name="deep_copy"></param>
        public Matrix3x3(float[] in_data, bool deep_copy = false )
        {
            if (in_data.Length == DataCount)
            {
                if (deep_copy)
                {
                    for (int i = 0; i < DataCount; i++)
                    {
                        m_Data[i] = in_data[i];
                    }
                }
                else
                {
                    m_Data = in_data;
                }
            }
            else
            {
                Debug.LogError("Error Vector3 size is not equal to 3");
            }
        }

        public Matrix3x3()
        {
            float [] data = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            m_Data = data;
        }

        public static Matrix3x3 operator*(Matrix3x3 a, Matrix3x3 b)
        {
            Matrix3x3 temp = new Matrix3x3();
            for (int row_index = 0; row_index < RowCount; row_index++)
            {
                for (int col_index = 0; col_index < RowCount; col_index++)
                {
                    temp[row_index, col_index] =
                        a[row_index, 0] * b[0, col_index] +
                        a[row_index, 1] * b[1, col_index] +
                        a[row_index, 2] * b[2, col_index];
                }
            }

            return temp;
        }
    }

}
