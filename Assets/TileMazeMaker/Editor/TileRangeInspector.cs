using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TileMazeMaker.TileGen
{

    [CustomEditor(typeof(TileRange))]
    public class TileRangeInspector : Editor
    {
        TileRange range;
        Texture2D gray_range;
        Texture2D active_range;
        int viewport_size = 400;

        public override void OnInspectorGUI()
        {
            if (range == null)
            {
                range = target as TileRange;
            }

            if (gray_range == null)
            {
                gray_range = Resources.Load<Texture2D>("Texture2d/range_gray") as Texture2D;
            }

            if (active_range == null)
            {
                active_range = Resources.Load<Texture2D>("Texture2D/range_active") as Texture2D;
            }

            ShowDiamondRangeInspector(range);

            EditorUtility.SetDirty(range);
        }

        void ShowDiamondRangeInspector(TileRange range)
        {
            if (range.customize_range == null)
                range.customize_range = new List<bool>();

            range.min_radius = EditorGUILayout.IntField("Min Radius", range.min_radius);
            range.max_radius = EditorGUILayout.IntField("Max Radius", range.max_radius);
            range.pitch = range.max_radius * 2 + 1;

            int tex_width = viewport_size / range.pitch;

            range.center_x = range.max_radius;
            range.center_y = range.max_radius;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Rebuild Range"))
            {
                range.customize_range.Clear();
                for (int col = 0; col < range.pitch; col++)
                {
                    for (int row = 0; row < range.pitch; row++)
                    {
                        int md = ManhatonDistance(col, row, range.center_x, range.center_y);
                        range.customize_range.Add(md <= range.max_radius && md >= range.min_radius);
                    }
                }
            }

            if (GUILayout.Button("Save Range"))
            {
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.EndHorizontal();

            if (range.customize_range.Count == range.pitch * range.pitch)
            {
                for (int col = 0; col < range.pitch; col++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int row = 0; row < range.pitch; row++)
                    {
                        if (range.customize_range[col * range.pitch + row])
                        {
                            if (GUILayout.Button(active_range,
                                GUILayout.Width(tex_width),
                                GUILayout.Height(tex_width),
                                GUILayout.MaxHeight(tex_width),
                                GUILayout.MaxWidth(tex_width)
                                ))
                            {
                                SwitchAt(range.customize_range, row, col, range.pitch);
                            }
                        }
                        else
                        {
                            if (GUILayout.Button(gray_range,
                               GUILayout.Width(tex_width),
                               GUILayout.Height(tex_width),
                               GUILayout.MaxHeight(tex_width),
                               GUILayout.MaxWidth(tex_width)
                               ))
                            {
                                SwitchAt(range.customize_range, row, col, range.pitch);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        public void SwitchAt(List<bool> flags, int row, int col, int pitch)
        {
            int index = col * pitch + row;
            flags[index] = !flags[index];
        }

        public static int ManhatonDistance(int x1, int y1, int x2, int y2)
        {
            return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
        }
    }
}