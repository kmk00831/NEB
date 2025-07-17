// using UnityEditor;
// using UnityEditor.Build.Reporting;
// using UnityEngine;
// using System.Linq;

// public class ExportAndroidProject
// {
//     [MenuItem("Build/Export Android Studio Project")]
//     public static void ExportProject()
//     {
//         // Build Settings에서 체크된 씬 목록 자동 가져오기
//         string[] scenes = EditorBuildSettings.scenes
//             .Where(scene => scene.enabled)
//             .Select(scene => scene.path)
//             .ToArray();

//         // 내보낼 Android Studio 프로젝트 경로
//         string exportPath = "Builds/AndroidStudioProject";

//         // 빌드 옵션 설정
//         BuildPlayerOptions buildOptions = new BuildPlayerOptions
//         {
//             scenes = scenes,
//             locationPathName = exportPath,
//             target = BuildTarget.Android,
//             options = BuildOptions.None  // 일반 빌드
//         };

//         // 빌드 실행
//         BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
//         BuildSummary summary = report.summary;

//         if (summary.result == BuildResult.Succeeded)
//         {
//             Debug.Log($"Android Studio 프로젝트 export 성공: {exportPath}");
//         }
//         else
//         {
//             Debug.LogError($"export 실패: {summary.result}");
//         }
//     }
// }


