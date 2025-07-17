// using UnityEngine;
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine.SceneManagement;
// using System.Linq;

// [InitializeOnLoad]
// public static class SceneCopier
// {
//     private const string playOnceKey = "SceneCopier_PlayedOnce";
//     private static double startTime = 0;
//     private static bool isWaiting = false;

//     static SceneCopier()
//     {
//         EditorApplication.update += TryAutoPlayOnce;
//         EditorApplication.playModeStateChanged += OnPlayModeChanged;
//     }

//     // 1회 자동 재생 로직 (2초 지연)
//     private static void TryAutoPlayOnce()
//     {
//         if (SessionState.GetBool(playOnceKey, false))
//         {
//             EditorApplication.update -= TryAutoPlayOnce;
//             return;
//         }

//         if (!DXFLoader.modelingFinished)
//         {
//             Debug.Log("⏳ 모델링 완료 대기 중...");
//             return;
//         }


//         if (!isWaiting)
//         {
//             startTime = EditorApplication.timeSinceStartup;
//             isWaiting = true;
//             Debug.Log("5초 후 자동 Play 예정");
//             return;
//         }

//         // 2초 경과하면 실행
//         if (EditorApplication.timeSinceStartup - startTime >= 5.0)
//         {
//             EditorApplication.update -= TryAutoPlayOnce;
//             if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
//             {
//                 Debug.Log("자동 Play 시작");
//                 SessionState.SetBool(playOnceKey, true);
//                 EditorApplication.isPlaying = true;
//             }
//         }
//     }

//     // Play 모드 상태 변화 감지 → 복사 & 자동 종료
//     private static void OnPlayModeChanged(PlayModeStateChange state)
//     {
//         if (state == PlayModeStateChange.ExitingEditMode)
//         {
//             CopyObjectsOnce();
//         }
//         else if (state == PlayModeStateChange.EnteredPlayMode)
//         {
//             Debug.Log("⏹ 자동으로 PlayMode 종료");
//             EditorApplication.isPlaying = false;
//         }
//     }

//     // 오브젝트 복사 로직 (INBScene → INBScene2)
//     private static void CopyObjectsOnce()
//     {
//         string sourceScenePath = "Assets/Scenes/INBAnd.unity"; //수정할 코드
//         string targetScenePath = "Assets/Scenes/AutoINBAnd.unity";//옮겨질 씬 //수정할 코드

//         if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
//         {
//             Scene targetScene = EditorSceneManager.OpenScene(targetScenePath, OpenSceneMode.Additive);

//             bool hasMainCamera = targetScene.GetRootGameObjects().Any(obj => obj.name == "Main Camera");
//             bool hasGameRoot = targetScene.GetRootGameObjects().Any(obj => obj.name == "GameObject");

//             if (!hasMainCamera || !hasGameRoot)
//             {
//                 Scene sourceScene = EditorSceneManager.OpenScene(sourceScenePath, OpenSceneMode.Additive);

//                 foreach (GameObject obj in sourceScene.GetRootGameObjects())
//                 {
//                     if (obj.name == "Main Camera" && !hasMainCamera)
//                     {
//                         GameObject clone = Object.Instantiate(obj);
//                         clone.name = obj.name;
//                         SceneManager.MoveGameObjectToScene(clone, targetScene);
//                     }

//                     if (obj.name == "GameObject" && !hasGameRoot)
//                     {
//                         GameObject clone = Object.Instantiate(obj);
//                         clone.name = obj.name;
//                         SceneManager.MoveGameObjectToScene(clone, targetScene);
//                     }
//                 }

//                 EditorSceneManager.SaveScene(targetScene);
//                 Debug.Log("test에 오브젝트 복사 완료");
//             }
//             else
//             {
//                 Debug.Log("이미 오브젝트 존재. 복사하지 않음");
//             }

//             EditorSceneManager.CloseScene(targetScene, true);
//         }
//     }
// }
