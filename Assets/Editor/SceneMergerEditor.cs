// #if UNITY_EDITOR
// using UnityEngine;
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine.SceneManagement;

// [InitializeOnLoad]
// public static class SceneAutoMerger
// {
//     static SceneAutoMerger()
//     {
//         EditorApplication.playModeStateChanged += OnPlayModeChanged;
//     }

//     static void OnPlayModeChanged(PlayModeStateChange state)
//     {
//         if (state == PlayModeStateChange.ExitingEditMode)
//         {
//             // INBScene2가 열려있을 때만 작동
//             if (SceneManager.GetActiveScene().name == "INBScene2")
//             {
//                 EditorApplication.delayCall += MergeCameraAndPlayer;
//             }
//         }
//     }

//     static void MergeCameraAndPlayer()
//     {
//         string sourcePath = "Assets/Scenes/INBScene.unity";
//         string targetPath = "Assets/Scenes/INBScene2.unity";

//         // 대상 씬 다시 열기 (Single 모드)
//         Scene targetScene = EditorSceneManager.OpenScene(targetPath, OpenSceneMode.Single);

//         // 중복 확인
//         bool hasCamera = GameObject.Find("Main Camera") != null;
//         bool hasPlayer = GameObject.Find("GameObject") != null;

//         if (hasCamera && hasPlayer)
//         {
//             Debug.Log("✅ INBScene2에 Main Camera 및 Player 이미 존재. 복사 안 함.");
//             return;
//         }

//         // INBScene 열기
//         if (!System.IO.File.Exists(sourcePath))
//         {
//             Debug.LogError("❌ INBScene 파일을 찾을 수 없습니다. 경로를 확인하세요.");
//             return;
//         }

//         Scene sourceScene = EditorSceneManager.OpenScene(sourcePath, OpenSceneMode.Additive);
//         int copyCount = 0;

//         foreach (GameObject obj in sourceScene.GetRootGameObjects())
//         {
//             if ((obj.name == "Main Camera" && !hasCamera) ||
//                 (obj.name == "GameObject" && !hasPlayer))
//             {
//                 GameObject clone = Object.Instantiate(obj);
//                 clone.name = obj.name;
//                 SceneManager.MoveGameObjectToScene(clone, targetScene);
//                 copyCount++;
//                 Debug.Log($"➕ 복사됨: {obj.name}");
//             }
//         }

//         // INBScene 닫기 + 저장
//         EditorSceneManager.CloseScene(sourceScene, true);
//         EditorSceneManager.SaveScene(targetScene);
//         Debug.Log($"✅ INBScene2 저장 완료: {copyCount}개 오브젝트 복사됨");
//     }
// }
// #endif
