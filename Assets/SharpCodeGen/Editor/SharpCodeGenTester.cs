using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using SharpCodeGen;


public class SharpCodeGenTester : EditorWindow {

    [MenuItem("Tools/Sharp Code Gen Tester")]
    public static void GenerateUIFileMenu()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(SharpCodeGenTester));
        window.title = "SharpCodeGenTester";
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Test 1"))
        {
            SharpCodeFile code_file = new SharpCodeFile();
            SharpUsing using_01 = new SharpUsing();
            SharpUsing using_02 = new SharpUsing();
            SharpUsing using_03 = new SharpUsing();

            using_01.identity_name = "System.Collections";
            using_02.identity_name = "System.Collections.Generic";
            using_03.identity_name = "UnityEngine";

            code_file.block_usings.Add(using_01);
            code_file.block_usings.Add(using_02);
            code_file.block_usings.Add(using_03);

            string path = "Assets/SharpCodeGen/Test/test_sharp.cs";
            if (File.Exists(path) == false)
            {
                TextAsset ta = new TextAsset();
                AssetDatabase.CreateAsset(ta, path);
            }

            AssetDatabase.Refresh();
            code_file.ToCode( Application.dataPath + "/SharpCodeGen/Test/test_sharp.cs" );
          
        }
    }
}
