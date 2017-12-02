using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen 
{

    public class TileRect
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public TileRect() 
        {
            x = 0;
            y = 0;
            width = 0;
            height = 0;
        }

        public TileRect(int x1, int y1, int w, int h) 
        {
            x = x1;
            y = y1;
            width = w;
            height = h;            
        }

        public void SetByTwoPoint(int x1, int y1, int x2, int y2) 
        {
            x = x1 < x2 ? x1 : x2;
            y = y1 < y2 ? y1 : y2;
            width = Mathf.Abs(x1 - x2) + 1;
            height = Mathf.Abs(y1 - y2) + 1;
        }

        public int x_end 
        {
            get 
            {
                return x + width - 1;
            }
        }

        public int y_end 
        {
            get 
            {
                return y + height - 1;
            }
        }
    }

}
