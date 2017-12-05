using UnityEngine;
using System;
using System.Collections.Generic;

namespace TileMazeMaker
{
    [Serializable]
    public class Matrix
    {
        const int C_ROWS = 3;
        const int C_COLS = 3;

        public Vector3[] vector3x3;

        public float [] element;

        //return row i col j
        public float this[int i, int j]
        {
            get
            {
                return vector3x3[i][j];
            }
            set
            {
                vector3x3[i][j] = value;
            }
        }

        public Matrix(Vector3[] vecs)
        {
            if (vecs.Length == C_ROWS)
            {
                vector3x3 = vecs;
            }
            else
            {
                Debug.LogError("Error, Matrix Rows does not match");
            }
        }

        public Matrix(float[,] m)
        {
            vector3x3 = new Vector3[C_ROWS];

            for (int i = 0; i < C_ROWS; i++)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    vector3x3[i][j]= m[i, j];
                }
            }
        }

        #region 矩阵数学运算
        public static Matrix MAbs(Matrix a)
        {
            Matrix _thisCopy = a.DeepCopy();
            for (int i = 0; i < C_ROWS; i++)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    _thisCopy[i, j] = Math.Abs(a[i, j]);
                }
            }
            return _thisCopy;
        }
        /// <summary>
        /// 矩阵相加
        /// </summary>
        /// <param name="a">第一个矩阵,和b矩阵必须同等大小</param>
        /// <param name="b">第二个矩阵</param>
        /// <returns>返回矩阵相加后的结果</returns>
        public static Matrix operator +(Matrix a, Matrix b)
        {
            Vector3[] new_vec_33 = new Vector3[C_ROWS];
            for (int i = 0; i < C_ROWS; i++)
            {
                new_vec_33[i] = a.vector3x3[i] + b.vector3x3[i];
            }
            return new Matrix(new_vec_33);
        }
        /// <summary>
        /// 矩阵相减
        /// </summary>
        /// <param name="a">第一个矩阵,和b矩阵必须同等大小</param>
        /// <param name="b">第二个矩阵</param>
        /// <returns>返回矩阵相减后的结果</returns>
        public static Matrix operator -(Matrix a, Matrix b)
        {
            Vector3[] new_vec_33 = new Vector3[C_ROWS];
            for (int i = 0; i < C_ROWS; i++)
            {
                new_vec_33[i] = a.vector3x3[i] - b.vector3x3[i];
            }           
            return new Matrix(new_vec_33);
        }
        /// <summary>
        /// 对矩阵每个元素取相反数
        /// </summary>
        /// <param name="a">二维矩阵</param>
        /// <returns>得到矩阵的相反数</returns>
        public static Matrix operator -(Matrix a)
        {
            Vector3[] new_vecs = new Vector3[C_ROWS];

            Matrix res = a;
            for (int i = 0; i < C_ROWS; i++)
            {
                new_vecs[i] = -a.vector3x3[i];
            }
            return new Matrix( new_vecs );
        }

        public static Vector3 operator *(Matrix a, Vector3 v)
        {
            return new Vector3(
                Vector3.Dot(a.vector3x3[0], v),
                Vector3.Dot(a.vector3x3[1], v),
                Vector3.Dot(a.vector3x3[2], v));
        }

        /// <summary>
        /// 矩阵相乘
        /// </summary>
        /// <param name="a">第一个矩阵</param>
        /// <param name="b">第二个矩阵,这个矩阵的行要与第一个矩阵的列相等</param>
        /// <returns>返回相乘后的一个新的矩阵</returns>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            float[,] res = new float[C_ROWS, C_COLS];
            for (int i = 0; i < C_ROWS; i++)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    for (int k = 0; k < C_COLS; k++)
                    {
                        res[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return new Matrix(res);
        }
        /// <summary>
        /// 矩阵与数相乘
        /// </summary>
        /// <param name="a">第一个矩阵</param>
        /// <param name="num">一个实数</param>
        /// <returns>返回相乘后的新的矩阵</returns>
        public static Matrix operator *(Matrix a, float num)
        {
            Vector3[] v3 = new Vector3[C_ROWS];
            
            for (int i = 0; i < C_ROWS; i++)
            {
                v3[i] = a.vector3x3[i] * num;
            }
            return new Matrix( v3 );
        }

        /// <summary>
        /// 矩阵转置
        /// </summary>
        /// <returns>返回当前矩阵转置后的新矩阵</returns>
        public Matrix Transpose()
        {
            float[,] res = new float[C_COLS, C_ROWS];
            {
                for (int i = 0; i < C_COLS; i++)
                {
                    for (int j = 0; j < C_ROWS; j++)
                    {
                        res[i, j] = this[j, i];
                    }
                }
            }
            return new Matrix(res);
        }
        /// <summary>
        /// 矩阵求逆
        /// </summary>
        /// <returns>返回求逆后的新的矩阵</returns>
        public Matrix Inverse()
        {
            //最后原始矩阵并不变，所以需要深拷贝一份
            Matrix _thisCopy = this.DeepCopy();
            if ( this.Determinant() != 0)
            {
                //初始化一个同等大小的单位阵
                Matrix res = _thisCopy.EMatrix();
                for (int i = 0; i < C_ROWS; i++)
                {
                    //首先找到第i列的绝对值最大的数，并将该行和第i行互换
                    int rowMax = i;
                    float max = Math.Abs(_thisCopy[i, i]);
                    for (int j = i; j < C_ROWS; j++)
                    {
                        if (Math.Abs(_thisCopy[j, i]) > max)
                        {
                            rowMax = j;
                            max = Math.Abs(_thisCopy[j, i]);
                        }
                    }
                    //将第i行和找到最大数那一行rowMax交换
                    if (rowMax != i)
                    {
                        _thisCopy.Exchange(i, rowMax);
                        res.Exchange(i, rowMax);

                    }
                    //将第i行做初等行变换，将第一个非0元素化为1
                    float r = 1.0f / _thisCopy[i, i];
                    _thisCopy.Exchange(i, -1, r);
                    res.Exchange(i, -1, r);
                    //消元
                    for (int j = 0; j < C_ROWS; j++)
                    {
                        //到本行后跳过
                        if (j == i)
                            continue;
                        else
                        {
                            r = -_thisCopy[j, i];
                            _thisCopy.Exchange(i, j, r);
                            res.Exchange(i, j, r);
                        }
                    }
                }
                return res;
            }
            else
            {
                throw new Exception("Determinant is zero");
            }
        }
        #region 重载比较运算符
        public static bool operator <(Matrix a, Matrix b)
        {
            bool issmall = true;
            for (int i = 0; i < C_ROWS; i++)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    if (a[i, j] >= b[i, j]) issmall = false;
                }
            }
            return issmall;
        }
        public static bool operator >(Matrix a, Matrix b)
        {
            bool issmall = true;
            for (int i = 0; i < C_ROWS; i++)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    if (a[i, j] <= b[i, j]) issmall = false;
                }
            }
            return issmall;
        }
        public static bool operator <=(Matrix a, Matrix b)
        {
            bool issmall = true;
            for (int i = 0; i < C_ROWS; i++)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    if (a[i, j] > b[i, j]) issmall = false;
                }
            }
            return issmall;
        }
        public static bool operator >=(Matrix a, Matrix b)
        {
            bool issmall = true;
            for (int i = 0; i < C_ROWS; i++)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    if (a[i, j] < b[i, j]) issmall = false;
                }
            }
            return issmall;
        }
        public static bool operator !=(Matrix a, Matrix b)
        {
            bool issmall = true;
            issmall = ReferenceEquals(a, b);
            if (issmall) return issmall;
            for (int i = 0; i < C_ROWS; i++)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    if (a[i, j] == b[i, j]) issmall = false;
                }
            }
            return issmall;
        }
        public static bool operator ==(Matrix a, Matrix b)
        {
            bool issmall = true;
            issmall = ReferenceEquals(a, b);
            if (issmall) return issmall;
            for (int i = 0; i < C_ROWS; i++)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    if (a[i, j] != b[i, j]) issmall = false;
                }
            }
            return issmall;
        }
        public override bool Equals(object obj)
        {
            Matrix b = obj as Matrix;
            return this == b;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
        public float Determinant()
        {
            Matrix _thisCopy = this.DeepCopy();
            //行列式每次交换行，都需要乘以-1
            float res = 1;
            for (int i = 0; i < C_ROWS; i++)
            {
                //首先找到第i列的绝对值最大的数
                int rowMax = i;
                float max = Math.Abs(_thisCopy[i, i]);
                for (int j = i; j < C_ROWS; j++)
                {
                    if (Math.Abs(_thisCopy[j, i]) > max)
                    {
                        rowMax = j;
                        max = Math.Abs(_thisCopy[j, i]);
                    }
                }
                //将第i行和找到最大数那一行rowMax交换,同时将单位阵做相同初等变换
                if (rowMax != i)
                {
                    _thisCopy.Exchange(i, rowMax);
                    res *= -1;
                }
                //消元
                for (int j = i + 1; j < C_ROWS; j++)
                {
                    float r = -_thisCopy[j, i] / _thisCopy[i, i];
                    _thisCopy.Exchange(i, j, r);
                }
            }
            //计算对角线乘积
            for (int i = 0; i < C_ROWS; i++)
            {
                res *= _thisCopy[i, i];
            }
            return res;
        }
        #endregion
        #region 初等变换
        /// <summary>
        /// 初等变换：交换第r1和第r2行
        /// </summary>
        /// <param name="r1">第r1行</param>
        /// <param name="r2">第r2行</param>
        /// <returns>返回交换两行后的新的矩阵</returns>
        public Matrix Exchange(int r1, int r2)
        {
            if (Math.Min(r2, r1) >= 0 && Math.Max(r1, r2) < C_ROWS)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    float temp = this[r1, j];
                    this[r1, j] = this[r2, j];
                    this[r2, j] = temp;
                }
                return this;
            }
            else
            {
                throw new Exception("超出索引");
            }
        }
        /// <summary>
        /// 初等变换：将r1行乘以某个数加到r2行
        /// </summary>
        /// <param name="r1">第r1行乘以num</param>
        /// <param name="r2">加到第r2行，若第r2行为负，则直接将r1乘以num并返回</param>
        /// <param name="num">某行放大的倍数</param>
        /// <returns></returns>
        public Matrix Exchange(int r1, int r2, float num)
        {
            if (Math.Min(r2, r1) >= 0 && Math.Max(r1, r2) < C_ROWS)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    this[r2, j] += this[r1, j] * num;
                }
                return this;
            }
            else if (r2 < 0)
            {
                for (int j = 0; j < C_COLS; j++)
                {
                    this[r1, j] *= num;
                }
                return this;
            }
            else
            {
                throw new Exception("Out of index");
            }
        }
        /// <summary>
        /// 得到一个同等大小的单位矩阵
        /// </summary>
        /// <returns>返回一个同等大小的单位矩阵</returns>
        public Matrix EMatrix()
        {
            if (C_ROWS == C_COLS)
            {
                float[,] res = new float[C_ROWS, C_COLS];
                for (int i = 0; i < C_ROWS; i++)
                {
                    for (int j = 0; j < C_COLS; j++)
                    {
                        if (i == j)
                            res[i, j] = 1;
                        else
                            res[i, j] = 0;
                    }
                }
                return new Matrix(res);
            }
            else
                throw new Exception("不是方阵，无法得到单位矩阵");
        }
        #endregion
        /// <summary>
        /// 深拷贝，仅仅将值拷贝给一个新的对象
        /// </summary>
        /// <returns>返回深拷贝后的新对象</returns>
        public Matrix DeepCopy()
        {
            Vector3[] v3 = new Vector3[C_ROWS];
            for (int i = 0; i < C_ROWS; i++)
            {
                v3[i] = vector3x3[i];
            }
            return new Matrix(v3);
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", vector3x3[0], vector3x3[1], vector3x3[2]);
        }
    }
}