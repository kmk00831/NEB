// using UnityEditor;
// using UnityEditor.SceneManagement;

// [InitializeOnLoad]
// public static class AutoSceneLoader
// {
//     static AutoSceneLoader()
//     {
//         // 플레이 모드로 전환되기 전, 자동 실행
//         EditorApplication.playModeStateChanged += OnPlayModeChanged;
//     }

//     private static void OnPlayModeChanged(PlayModeStateChange state)
//     {
//         if (state == PlayModeStateChange.ExitingEditMode)
//         {
//             string targetScenePath = "Assets/Scenes/AutoINB.unity"; // 열고 싶은 씬 경로 //수정할 코드

//             if (System.IO.File.Exists(targetScenePath))
//             {
//                 EditorSceneManager.OpenScene(targetScenePath);
//                 UnityEngine.Debug.Log($"🔄 자동으로 {targetScenePath} 씬 열림");
//             }
//             else
//             {
//                 UnityEngine.Debug.LogWarning($"❌ 지정된 씬이 존재하지 않음: {targetScenePath}");
//             }
//         }
//     }
// }
