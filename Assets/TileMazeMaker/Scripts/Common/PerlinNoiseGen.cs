using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker 
{
    public class PerlinNoiseGen : MonoBehaviour
    {
        [SerializeField]
        int tex_width = 512;
        [SerializeField]
        int tex_height = 512;
        [SerializeField]
        float x_org = 0;
        [SerializeField]
        float y_org = 0;
        [SerializeField]
        [Range(0.1f, 10.0f)]
        float scale = 1;
        private Color[] pixels;
        private Texture2D tex2D;
        private Renderer rdr;

        public void Prepare()
        {
            rdr = GetComponent<Renderer>();
            tex2D = new Texture2D(tex_width, tex_height);
            pixels = new Color[tex_height * tex_width];
            rdr.material.mainTexture = tex2D;
        }

        public void SetTexture()
        {
            float y = 0.0f;
            while (y < tex2D.height)
            {
                float x = 0.0f;
                while (x < tex2D.width)
                {
                    float xCoord = x_org + x / tex2D.width * scale;
                    float yCoord = y_org + y / tex2D.height * scale;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);
                    pixels[(int)(y * tex2D.width + x)] = new Color(sample, sample, sample);
                    x += 1.0f;
                }
                y += 1.0f;
            }
            tex2D.SetPixels(pixels);
            tex2D.Apply();
        }

        // Use this for initialization
        void Start()
        {
            Prepare();
        }

        float old_scale = 0;
        // Update is called once per frame
        void Update()
        {
            if (scale != old_scale)
            {
                old_scale = scale;
                SetTexture();
            }
        }
    }

}
