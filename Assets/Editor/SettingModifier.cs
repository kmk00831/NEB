using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class SettingModifier // 클래스 이름
{
    static SettingModifier()
    {
        // 에디터 시작 시 한번 실행됨
        AddUserClickEventLayer(); // 클릭을 위한 이벤트 레이어 설정
        AddSetupObjectOnLoad(); // 4. Setup 오브젝트 추가 및 설정
    }

    // 유니티 에디터 메뉴에 항목 추가하기
    [MenuItem("MyTools/Modify WebGL Canvas Size")]
    public static void ModifyWebGLSwitching()
    {
        Debug.Log("⚙️ WebGL 설정 변경 스크립트 실행 (플랫폼 스위칭 포함)!");

        // ====== 여기서 빌드 타겟을 WebGL로 스위칭! ======
        Debug.Log($"🔄 현재 빌드 타겟: {EditorUserBuildSettings.activeBuildTarget}");

        // 현재 타겟이 WebGL이 아니면 스위칭
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
        {
            Debug.Log("▶️ 빌드 타겟을 WebGL로 스위칭 중...");
            // EditorUserBuildSettings.SwitchActiveBuildTarget(빌드 그룹, 빌드 타겟)
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            Debug.Log("✅ WebGL로 스위칭 완료!");
        }
        else
        {
            Debug.Log("✅ 이미 WebGL 타겟입니다.");
        }
        // ====== 플랫폼 스위칭 끝 ======
    }

    // 유니티 에디터 메뉴에 항목 추가하기
    [MenuItem("MyTools/Modify WebGL Settings (HTTP Allow)")]
    public static void ModifyWebGLSettingsAndAllowHTTP()
    {
        // HTTP 다운로드 허용 (Data Caching 설정)
        // PlayerSettings.WebGL.dataCaching 속성을 true로 설정!
        bool currentState = PlayerSettings.WebGL.dataCaching;
        Debug.Log($"➡️ 현재 데이터 캐싱 (HTTP 허용) 상태: {currentState}");

        PlayerSettings.WebGL.dataCaching = true; // <--- 이 부분이 HTTP 다운로드를 허용하는 설정!

        // 🔴 현재 설정 상태 확인 🔴
        InsecureHttpOption currentSetting = PlayerSettings.insecureHttpOption;
        Debug.Log($"➡️ 현재 'Allow downloads over HTTP' 설정 상태: {currentSetting}");

        // 🔴 'Allow downloads over HTTP' 설정을 'Always Allowed'로 변경 🔴
        PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
        
        Debug.Log($"✅ 데이터 캐싱 (HTTP 허용) 설정 변경 완료: {PlayerSettings.WebGL.dataCaching}");

        // ====== Player Settings 수정 끝! ======
    }

    // 유니티 에디터 메뉴에 항목 추가하기
    [MenuItem("MyTools/Modify WebGL Settings (Compression Format -> Gzip)")]
    public static void ModifyWebGLSettingsCompressionFormatTartgetGzip()
    {
        // 2. Compression Format을 Gzip으로 설정!
        WebGLCompressionFormat currentCompression = PlayerSettings.WebGL.compressionFormat;
        Debug.Log($"➡️ 현재 Compression Format: {currentCompression}");

        if (currentCompression != WebGLCompressionFormat.Gzip)
        {
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip; // <--- Gzip으로 설정!
            Debug.Log($"✅ Compression Format 설정 변경 완료: {PlayerSettings.WebGL.compressionFormat}");

            // 서버 설정도 같이 해줘야 압축된 파일이 제대로 제공됩니다. [[6]] [[7]]
            // 서버(Apache, Nginx 등)에서 Content-Encoding: gzip 헤더를 보내도록 설정 필요. [[7]]
            Debug.LogWarning("⚠️ Compression Format을 Gzip으로 설정했습니다. 웹 서버에서 Content-Encoding: gzip 헤더를 보내도록 설정해야 합니다.");
        }
        else
        {
            Debug.Log($"✅ Compression Format은 이미 Gzip으로 설정되어 있습니다.");
        }
    }


    public static void AddSetupObjectOnLoad()
    {
        Debug.Log("✨ 오브젝트 자동 추가 스크립트 실행!");

        // 이미 해당 이름의 오브젝트가 씬에 있는지 체크!
        GameObject existingObject = GameObject.Find("Setup");

        if (existingObject == null) // 오브젝트가 씬에 없을 때만 새로 생성
        {
            GameObject myNewObject = new GameObject("Setup");
            myNewObject.transform.position = new Vector3(0, 0, 0); // 위치 설정
            myNewObject.AddComponent<Setup>();
            Setup setupScript = myNewObject.GetComponent<Setup>();
            setupScript.SceneName = "NEB";
            setupScript.ModelingName = "Modeling";
            Debug.Log($"✅ '{myNewObject.name}' 오브젝트가 씬에 추가되었습니다.");

            // 추가된 오브젝트를 Undo/Redo 히스토리에 등록
            Undo.RegisterCreatedObjectUndo(myNewObject, "Create My Automatically Added Object");

            // ====== 변경 사항 저장 ======
            if (!Application.isBatchMode) // 에디터 GUI 모드일 때만 저장 시도
            {
                Debug.Log("ℹ️ GUI 모드이므로 씬 저장 시도.");
                if (!EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene()))
                {
                    Debug.LogWarning("⚠️ 현재 씬 저장에 실패했습니다.");
                    // GUI 모드에서 저장 실패 시 사용자에게 알림
                }
                else
                {
                    Debug.Log("✅ 현재 씬에 변경 사항 (오브젝트 추가) 저장 완료.");
                }
            }
            else
            {
                Debug.Log("ℹ️ 배치 모드이므로 씬 저장을 건너뜁니다.");
                // 배치 모드에서는 씬 저장 실패 경고를 무시하도록 할 수 있음
            }
        }
        else
        {
            Debug.Log($"ℹ️ '{existingObject.name}' 오브젝트는 이미 씬에 존재합니다. 새로 추가하지 않습니다.");
        }
        Debug.Log("✨ 오브젝트 자동 추가 스크립트 실행 종료.");
    }
    
    [MenuItem("Tools/Add Layer: UserClickEvent")]
    public static void AddUserClickEventLayer()
    {
        string layerName = "UserClickEvent";

        // ProjectSettings/TagManager.asset에 접근
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        bool alreadyExists = false;

        // 이미 존재하는지 확인
        for (int i = 8; i < layersProp.arraySize; i++) // 0~7은 Unity 예약된 레이어
        {
            SerializedProperty layerSP = layersProp.GetArrayElementAtIndex(i);
            if (layerSP != null && layerSP.stringValue == layerName)
            {
                alreadyExists = true;
                break;
            }
        }

        if (!alreadyExists)
        {
            for (int i = 8; i < layersProp.arraySize; i++)
            {
                SerializedProperty layerSP = layersProp.GetArrayElementAtIndex(i);
                if (layerSP != null && string.IsNullOrEmpty(layerSP.stringValue))
                {
                    layerSP.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();
                    Debug.Log("✅ Layer 'UserClickEvent' 추가 완료!");
                    return;
                }
            }

            Debug.LogError("❌ 사용할 수 있는 빈 Layer 슬롯이 없습니다! (8~31)");
        }
        else
        {
            Debug.Log("ℹ️ Layer 'UserClickEvent'는 이미 존재합니다.");
        }
    }
    
    public static void PerformWebGLBuild()
    {
        Debug.Log("🚀 WebGL 자동 빌드 프로세스 시작 (executeMethod/메뉴 호출됨)!");

        // 씬 가져오기
        string targetScenePath = "Assets/Scenes/NEBWeb.unity";
        EditorSceneManager.OpenScene(targetScenePath, OpenSceneMode.Single); // 씬 로드! [[3]]
        
        // 터미널 빌드 시에는 static 생성자가 호출되지 않으므로,
        // 빌드에 필요한 설정과 오브젝트 추가 로직을 여기서 **호출**해야 합니다.

        Debug.Log("▶️ 설정 및 오브젝트 추가 단계 시작...");
        ModifyWebGLSwitching(); // 1. 빌드 타겟 스위칭
        ModifyWebGLSettingsAndAllowHTTP(); // 2. HTTP 다운로드 허용 설정
        ModifyWebGLSettingsCompressionFormatTartgetGzip(); // 3. Compression Format 설정
        AddUserClickEventLayer(); // 클릭을 위한 이벤트 레이어 설정
        AddSetupObjectOnLoad(); // 4. Setup 오브젝트 추가 및 설정
        Debug.Log("✅ 설정 및 오브젝트 추가 단계 완료.");

        // ====== 씬 변경 사항 저장! ======
        Debug.Log("💾 씬 변경 사항 저장 중...");
        // bool saveSuccess = EditorSceneManager.SaveCurrentScene(); // 이 라인 대신
        bool saveSuccess = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene()); // 이 라인을 사용!
        if (saveSuccess)
        {
            Debug.Log("✅ 씬 변경 사항 저장 완료!");
        }
        else
        {
            Debug.LogError("❌ 씬 변경 사항 저장 실패!");
        }
        // ===============================

        // 5. ====== 실제 빌드 실행! ======
        Debug.Log("▶️ WebGL 빌드 시작...");

        // 빌드에 포함할 씬 목록 가져오기 (Build Settings에 등록된 씬 사용)
        EditorBuildSettingsScene[] levels = EditorBuildSettings.scenes;
        // string[] scenes = Array.ConvertAll(levels, level => level.path);
        string[] scenes = { "Assets/Scenes/INBWeb.unity" }; // <--- 여기에 원하는 씬 경로 지정!

        // 빌드 결과물이 저장될 폴더 경로 설정
        string projectRootPath = Path.GetDirectoryName(Application.dataPath); // 프로젝트 루트 경로
        string buildPath = Path.Combine(projectRootPath, "TerminalBuilds", "INB", "WebGLBuild");

        // 빌드 폴더가 없으면 생성
        if (!Directory.Exists(buildPath))
        {
            Directory.CreateDirectory(buildPath);
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None; // 또는 원하는 빌드 옵션 조합


        // 빌드 시작!
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        // 6. 빌드 결과 확인 및 처리
        BuildSummary summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("🎉 WebGL 빌드 성공! 경로: " + buildPath + " (총 크기: " + summary.totalSize + " bytes)");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("😭 WebGL 빌드 실패!");
            // report 객체에서 자세한 오류 정보 확인 가능
        }
        else if (summary.result == BuildResult.Cancelled)
        {
            Debug.LogWarning("😅 WebGL 빌드 취소됨.");
        }
        else
        {
            Debug.Log("🤔 WebGL 빌드 결과: " + summary.result);
        }

        Debug.Log("🏁 WebGL 자동 빌드 프로세스 완료!");
    }
}
