#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace StroopGame.EditorTools
{
    /// <summary>
    /// WebGL 自動建置。
    /// 可從選單手動跑，也可從命令列 batch mode 跑（CI / Claude 自動化）。
    ///
    /// 命令列範例：
    ///   Unity -quit -batchmode -nographics \
    ///     -projectPath "<專案路徑>" \
    ///     -executeMethod StroopGame.EditorTools.WebGLBuilder.BuildFromCommandLine \
    ///     -buildOutput "<輸出資料夾>" -logFile "<log路徑>"
    /// </summary>
    public static class WebGLBuilder
    {
        const string DEFAULT_OUTPUT = "Build/WebGL";
        const string FALLBACK_SCENE = "Assets/Scenes/SampleScene.unity";

        [MenuItem("StroopGame/Build WebGL")]
        public static void BuildMenu()
        {
            string path = Build(DEFAULT_OUTPUT);
            if (path != null)
                EditorUtility.RevealInFinder(path);
        }

        /// <summary>命令列入口。</summary>
        public static void BuildFromCommandLine()
        {
            string output = DEFAULT_OUTPUT;
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
                if (args[i] == "-buildOutput") output = args[i + 1];

            string result = Build(output);
            // batch mode 一定要明確 exit code，CI 才知道成敗
            if (Application.isBatchMode)
                EditorApplication.Exit(result != null ? 0 : 1);
        }

        static string Build(string outputDir)
        {
            // 1. 找要 build 的場景
            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                if (File.Exists(FALLBACK_SCENE))
                {
                    scenes = new[] { FALLBACK_SCENE };
                    Debug.LogWarning($"[WebGLBuilder] Build Settings 沒場景，改用 {FALLBACK_SCENE}");
                }
                else
                {
                    Debug.LogError("[WebGLBuilder] 找不到任何場景可 build！");
                    return null;
                }
            }

            // 2. WebGL Player 設定（對應 BuildGuide.md）
            PlayerSettings.runInBackground = true;
            PlayerSettings.defaultWebScreenWidth = 1280;
            PlayerSettings.defaultWebScreenHeight = 720;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled; // 好托管、避免伺服器設定問題
            PlayerSettings.WebGL.dataCaching = false;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;        // 減小體積、加快 build
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.Low);

            // 3. 確保輸出資料夾存在
            string fullOut = Path.GetFullPath(outputDir);
            Directory.CreateDirectory(fullOut);

            // 4. Build
            var opts = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputDir,
                target = BuildTarget.WebGL,
                targetGroup = BuildTargetGroup.WebGL,
                options = BuildOptions.None,
            };

            Debug.Log($"[WebGLBuilder] 開始 build → {fullOut}");
            BuildReport report = BuildPipeline.BuildPlayer(opts);
            BuildSummary s = report.summary;

            if (s.result == BuildResult.Succeeded)
            {
                Debug.Log($"[WebGLBuilder] ✅ 成功！大小 {s.totalSize / (1024 * 1024)} MB，輸出：{fullOut}");
                return fullOut;
            }

            Debug.LogError($"[WebGLBuilder] ❌ 失敗：result={s.result}, errors={s.totalErrors}");
            return null;
        }
    }
}
#endif
