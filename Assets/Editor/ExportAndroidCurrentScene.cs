// using UnityEditor;
// using UnityEditor.Build.Reporting;
// using UnityEditor.SceneManagement;
// using UnityEngine;
// using System.IO;

// [InitializeOnLoad]
// public class ExportAndroidCurrentSceneOnPlayExit
// {
//     static string targetPath1 = "/Users/maybday/PycharmProjects/ZeroMapAppAutoGeneration/ZeroMapAppAutoGeneration/result/Unity Android 빌드 파일";
//     static string targetPath2 = "/Users/maybday/PycharmProjects/ZeroMapAppAutoGeneration/ZeroMapAppAutoGeneration/result/Unity Android 빌드 파일";

//     static ExportAndroidCurrentSceneOnPlayExit()
//     {
//         EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
//     }

//     private static void OnPlayModeStateChanged(PlayModeStateChange state)
//     {
//         if (state == PlayModeStateChange.ExitingPlayMode)
//         {
//             ExportTargetSceneToAndroid();
//         }
//     }

//     private static void ExportTargetSceneToAndroid()
//     {
//         // ✅ 여기서 특정 씬 경로를 지정
//         string targetScenePath = "Assets/Scenes/AutoINB.unity"; // <-- 원하는 씬 경로로 수정

//         if (!File.Exists(targetScenePath))
//         {
//             Debug.LogError($"❌ 지정된 씬이 존재하지 않습니다: {targetScenePath}");
//             return;
//         }

//         Debug.Log($"✅ 빌드 대상 씬: {targetScenePath}");

//         string exportPath = "Builds/AndroidStudioExport_TargetScene";

//         BuildPlayerOptions options = new BuildPlayerOptions
//         {
//             scenes = new[] { targetScenePath },
//             locationPathName = exportPath,
//             target = BuildTarget.Android,
//             options = BuildOptions.None
//         };

//         BuildReport report = BuildPipeline.BuildPlayer(options);
//         BuildSummary summary = report.summary;

//         if (summary.result == BuildResult.Succeeded)
//         {
//             Debug.Log($"✅ 자동 빌드 완료: {exportPath}");

//             string libsFolder = Path.Combine(exportPath, "unityLibrary/libs");
//             if (Directory.Exists(libsFolder))
//             {
//                 string[] libFiles = Directory.GetFiles(libsFolder);

//                 foreach (string filePath in libFiles)
//                 {
//                     string fileName = Path.GetFileName(filePath);
//                     string destDir = targetPath1.Trim();
//                     Directory.CreateDirectory(destDir);

//                     string destPath = Path.Combine(destDir, fileName);
//                     File.Copy(filePath, destPath, true);
//                 }

//                 Debug.Log("✅ libs 내부 파일 복사 완료");
//             }
//             else
//             {
//                 Debug.LogWarning("⚠️ libs 폴더가 존재하지 않음");
//             }

//             string[] foldersForTarget2 = new[]
//             {
//                 "unityLibrary/src/main/assets",
//                 "unityLibrary/src/main/Il2CppOutputProject",
//                 "unityLibrary/src/main/jniLibs",
//                 "unityLibrary/src/main/jniStaticLibs",
//                 "unityLibrary/src/main/resources"
//             };

//             foreach (string relativePath in foldersForTarget2)
//             {
//                 string sourcePath = Path.Combine(exportPath, relativePath);
//                 string destPath = Path.Combine(targetPath2, Path.GetFileName(relativePath));
//                 CopyDirectory(sourcePath, destPath);
//             }

//             Debug.Log("✅ 나머지 폴더 복사 완료");
//         }
//         else
//         {
//             Debug.LogError($"❌ 빌드 실패: {summary.result}");
//         }
//     }

//     private static void CopyDirectory(string sourceDir, string targetDir)
//     {
//         if (!Directory.Exists(sourceDir))
//         {
//             Debug.LogWarning($"⚠️ 복사할 경로가 존재하지 않음: {sourceDir}");
//             return;
//         }

//         Directory.CreateDirectory(targetDir);

//         foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
//         {
//             string relativePath = file.Substring(sourceDir.Length + 1);
//             string targetFilePath = Path.Combine(targetDir, relativePath);
//             Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));
//             File.Copy(file, targetFilePath, true);
//         }
//     }
// }
