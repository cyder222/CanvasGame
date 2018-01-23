using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MyBuilder
{
    // ビルド実行でAndroidのapkを作成する例
    [UnityEditor.MenuItem("Tools/Build Project AllScene Android")]
    public static void BuildProjectAllSceneAndroid()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        List<string> allScene = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                allScene.Add(scene.path);
            }
        }
        PlayerSettings.applicationIdentifier = "taiki.sample.canvasgame";
        PlayerSettings.statusBarHidden = true;
        BuildPipeline.BuildPlayer(
            allScene.ToArray(),
            "canvasgame.apk",
            BuildTarget.Android,
            BuildOptions.None
        );
    }
    [UnityEditor.MenuItem("Tools/Build Project AllScene WebGL")]
    public static void BuildProjectForWebGl()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        List<string> allScene = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                allScene.Add(scene.path);
            }
        }
        PlayerSettings.applicationIdentifier = "taiki.sample.canvasgame";
        PlayerSettings.statusBarHidden = true;
        BuildPipeline.BuildPlayer(
            allScene.ToArray(),
            "Bin/webgl/",
            BuildTarget.WebGL,
            BuildOptions.None
        );
    }
}