using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker
{
    public class PerlinRandom
    {
        float[] values;

        public float x_org = 0;
        public float y_org = 0;
        public float scale = 10;
        public int width = 512;
        public int height = 512;

        /// <summary>
        /// 不生成Texture，而只生成PerinNorse二维随机数数组。
        /// </summary>
        public void Generate()
        {
            values = new float[width * height];

            float y = 0.0f;
			float real_scale = scale;// * Mathf.PI / 3;

            while (y < height)
            {
                

                float x = 0.0f;
                while (x < width)
                {
                    float xCoord = x_org + x / width * real_scale;
                    float yCoord = y_org + y / height * real_scale;
                    values[(int)(y * width + x)] = Mathf.PerlinNoise(xCoord, yCoord);
                    x += 1.0f;
                }
                y += 1.0f;
            }
        }



        public float[] GetAll
        {
            get
            {
                return values;
            }
        }

        public float GetValueAt(int x_pos, int y_pos)
        {
            return values[y_pos * width + x_pos];
        }
    }
}