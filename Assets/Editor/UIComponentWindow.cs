using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CheckGitformLogWindow : EditorWindow
{
    [SerializeField]
    public bool isCreateWindow;
    [SerializeField]
    private List<UnityEngine.Object> selectedAssets = new List<UnityEngine.Object>();
    //跟目錄路徑
    private static string projectPath = Directory.GetParent(Application.dataPath).FullName;
    private static string gitStatusShort = "status --short";
    private static string gitBranch = "branch -a";
    private static string gitName = "git";
    private static string windowCMDName = "cmd.exe";
    private static string cmdKeepOpenOption = "/k";

    [MenuItem("Tools/Git/CheckGit")]
    private static void Open()
    {
        // MenuItem 被點擊時只開啟工具視窗；Git 指令由下方按鈕觸發。
        GetWindow<CheckGitformLogWindow>("CheckGit");
    }

    private void OnGUI()
    {
        // Unity 重新載入編輯器腳本後，保險確認清單仍然存在。
        if (selectedAssets == null)
        {
            selectedAssets = new List<UnityEngine.Object>();
        }

        //isCerateWindow = EditorGUILayout.Foldout("isCerateWindow", isCerateWindow);

        // 在 Index 欄位與素材欄位之間加入垂直間距。
        EditorGUILayout.Space(10);

        DrawGUI();


        // 按下按鈕後，才將選取素材的資料輸出到 Unity Console。
        GUIButtonAction("InputObjectData", () => 
        {

            // 沒有選取素材時，不繼續執行後面的資料讀取。
            if (selectedAssets.Count == 0)
            {
                UnityEngine.Debug.LogWarning("尚未選取素材。");
                return;
            }
            // 取得素材在 Unity 專案中的相對路徑，例如 Assets/Images/Icon.png。
            foreach (var assetData in selectedAssets)
            {
                // ObjectField 被清空後可能留下 null，因此先略過空項目。
                if (assetData == null)
                {
                    continue;
                }

                string assetPath = AssetDatabase.GetAssetPath(assetData);
                // GetType().Name 取得實際 Unity 類型，例如 Texture2D、Prefab 或 MonoScript。
                UnityEngine.Debug.Log(
                    $"素材名稱：{assetData.name}\n素材路徑：{assetPath}\n素材類別：{assetData.GetType().Name}");
            }   

        });
        GUIButtonAction("CheckGitStatus", () => ProcessGit(gitStatusShort, isCreateWindow, !isCreateWindow, !isCreateWindow));
        GUIButtonAction("CheckGitBranch", () => ProcessGit(gitBranch, isCreateWindow, !isCreateWindow, !isCreateWindow));
        GUIButtonAction("DeleteDateAll", () => selectedAssets.Clear());
       
    }

    private void ProcessGit(string arguments,bool useShellExecute,bool redirectStandardOutput,bool createNoWindow) 
    {
        if (useShellExecute)
        {
            var info = new ProcessStartInfo
            {
                FileName = windowCMDName,

                // 命令列參數
                Arguments = $"{cmdKeepOpenOption} \"{gitName} {arguments}\"",

                // 執行時所在的位址
                WorkingDirectory = projectPath,

                // 不使用作業系統 Shell，讓程式直接啟動 Git。
                UseShellExecute = useShellExecute,

                // 將 Git 的標準輸出導回 C#，之後可以讀取執行結果。
                RedirectStandardOutput = redirectStandardOutput,

                // 不另外開啟命令提示字元視窗。
                CreateNoWindow = createNoWindow
            };
            // /k 會讓 cmd.exe 保持開啟；不能使用 using 或 WaitForExit，
            // 否則 Unity 會等待使用者關閉黑色命令提示字元視窗。
            Process.Start(info);
        }
        else 
        {
            var info = new ProcessStartInfo
            {
                FileName = gitName,

                // 命令列參數
                Arguments = arguments,

                // 執行時所在的位址
                WorkingDirectory = projectPath,

                // 不使用作業系統 Shell，讓程式直接啟動 Git。
                UseShellExecute = useShellExecute,

                // 將 Git 的標準輸出導回 C#，之後可以讀取執行結果。
                RedirectStandardOutput = redirectStandardOutput,

                // 不另外開啟命令提示字元視窗。
                CreateNoWindow = createNoWindow
            };
            using (Process process = Process.Start(info))
            {
                // 讀取 Git 從標準輸出回傳的文字內容。
                string result = process.StandardOutput.ReadToEnd();
                // 等待 Git 程序執行完成，確保 ExitCode 已經可以取得。
                process.WaitForExit();
                // 將 Git 的執行結果與輸出內容寫入 Unity Console。
                UnityEngine.Debug.Log(
                    $"Git exit code: {process.ExitCode}\n{result}");
            }
        }
        
    }
    private void DrawGUI() 
    {

        // 顯示素材選取欄位，使用者可以將 Project 視窗中的素材拖曳到這裡。
        // typeof(UnityEngine.Object) 代表目前允許選取任何 Unity 素材類型。
        // false 代表不允許拖曳場景中的 GameObject，只接受 Project 素材。
        //selectedAssets.Add(EditorGUILayout.ObjectField("素材", selectedAssets[0], typeof(UnityEngine.Object), false));
        EditorGUILayout.LabelField(
        "將多個素材拖曳到這裡",
        EditorStyles.boldLabel);

        // GUILayoutUtility.GetRect 依照目前視窗寬度配置一個 60 像素高的矩形，
        // 這個矩形就是拖曳素材的有效區域。
        Rect dropArea = GUILayoutUtility.GetRect(
            0,
            60,
            GUILayout.ExpandWidth(true));

        // GUI.Box 只負責繪製可視化框線與提示文字，不會自動接收拖曳資料。
        GUI.Box(
            dropArea,
            "拖曳多個素材到這個區域");

        // Event.current 取得目前這一次 OnGUI 正在處理的滑鼠/拖曳事件。
        Event currentEvent = Event.current;

        // DragUpdated：素材正在拖曳經過區域。
        // DragPerform：使用者在區域內放開滑鼠，準備正式加入素材。
        if ((currentEvent.type == EventType.DragUpdated ||
             currentEvent.type == EventType.DragPerform) &&
            dropArea.Contains(currentEvent.mousePosition))
        {
            // Copy 讓 Unity 顯示「可以複製/加入」的拖曳游標。
            DragAndDrop.visualMode =
                DragAndDropVisualMode.Copy;

            if (currentEvent.type == EventType.DragPerform)
            {
                // AcceptDrag 通知 Unity 已接受這次拖曳，之後才能安全讀取拖曳物件。
                DragAndDrop.AcceptDrag();

                // objectReferences 是這次拖曳進來的所有 Unity 物件，支援一次多個素材。
                foreach (UnityEngine.Object draggedAsset
                         in DragAndDrop.objectReferences)
                {
                    if (draggedAsset == null)
                    {
                        continue;
                    }

                    // 只接受 Project 視窗中的素材。
                    if (!AssetDatabase.Contains(draggedAsset))
                    {
                        continue;
                    }

                    // List.Contains 判斷清單中是否已存在相同物件，避免重複加入。
                    if (selectedAssets.Contains(draggedAsset))
                    {
                        continue;
                    }

                    selectedAssets.Add(draggedAsset);
                }

                // GUI.changed 告訴 Unity 這次 GUI 有資料變更，需要重新繪製視窗。
                GUI.changed = true;
            }

            // Use 會標記事件已處理，避免同一個拖曳事件繼續被其他控制項使用。
            currentEvent.Use();
        }

        EditorGUILayout.Space(10);


        // ObjectField 讓使用者可以檢視或替換清單中的單一素材。
        // 不額外加入 null，避免按下輸出按鈕時出現空項目。
        for (int i = 0; i < selectedAssets.Count; i++)
        {
            selectedAssets[i] = EditorGUILayout.ObjectField(
                $"素材 {i + 1}",
                selectedAssets[i],
                typeof(UnityEngine.Object),
                false);
        }

        isCreateWindow = EditorGUILayout.Toggle("建立視窗", isCreateWindow);


    }
    private bool GUICreateButton(string buttonName, Action act = null) 
    {
        return GUILayout.Button(buttonName);
    }
    private void GUIButtonAction(string buttonName, Action act = null)
    {
        if(GUILayout.Button(buttonName))
        {
            act?.Invoke();
        }
    }
}
