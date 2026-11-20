using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CheckGitformLogWindow : EditorWindow
{
    [SerializeField]
    public bool isCreateWindow;

    //private List<UnityEngine.Object> _selectedAssets = new List<UnityEngine.Object>();
    private UnityEngine.Object newSelectedObject = new UnityEngine.Object();
    private List<UnityEngine.Object> selectedAssets = new List<UnityEngine.Object>();
    [SerializeField]
    private List<Type> assetsType = new List<Type>();
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
                // 將 Git 的執行結果與輸出內容寫入 Unity Console。
                UnityEngine.Debug.Log(
                    $"Git exit code: {process.ExitCode}\n{result}");
            }
        }
        
    }
    private void DrawGUI() 
    {

        EditorGUILayout.Space(10);

        // 所有清單操作按鈕都放在參考欄位上方。
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        UnityEngine.Object newObject = EditorGUILayout.ObjectField("NewObject",newSelectedObject,typeof(UnityEngine.Object),false,GUILayout.Width(480));
        newSelectedObject = newObject;
        // +：新增一筆值為 null 的資料。
        if (GUILayout.Button("+", GUILayout.Width(30)))
        {
            selectedAssets.Add(newSelectedObject);
            newSelectedObject = null;
        }

        // -：刪除清單最末位的資料。
        if (GUILayout.Button("-", GUILayout.Width(30)))
        {
            if (selectedAssets.Count > 0)
            {
                selectedAssets.RemoveAt(selectedAssets.Count - 1);
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUIButtonAction("Delete All", () => selectedAssets.Clear());
        GUIButtonAction("Refresh", RefreshSelectedAssets);

        // ObjectField 讓使用者可以檢視或替換清單中的單一素材。
        // 不額外加入 null，避免按下輸出按鈕時出現空項目。
        //檢查selectedAssets 欄位是否異動
        EditorGUI.BeginChangeCheck();
        // 每個欄位個別檢查，才能同時知道變更的是哪一列。
        for (int i = 0; i < selectedAssets.Count; i++)
        {
            // 在繪製 ObjectField 前保存目前事件與拖曳物件。
            Event currentEvent = Event.current;
            bool isDragPerform = currentEvent.type == EventType.DragPerform;
            List<UnityEngine.Object> draggedObjects = isDragPerform ? DragAndDrop.objectReferences.ToList() : null;

            string assetTypeName = selectedAssets[i] == null ? "NullObject": selectedAssets[i].GetType().Name;

            EditorGUI.BeginChangeCheck();
            UnityEngine.Object newAsset = EditorGUILayout.ObjectField($"{assetTypeName} {i + 1}", selectedAssets[i] , typeof(UnityEngine.Object) , false);

            if (EditorGUI.EndChangeCheck())
            {
                // newAsset 是這一列 ObjectField 變更後的值。
                selectedAssets[i] = newAsset;
                draggedObjects.Remove(newAsset);
                selectedAssets.AddRange(draggedObjects);
                if (isDragPerform && draggedObjects != null)
                {
                    UnityEngine.Debug.Log(
                        $"第 {i + 1} 列被拖曳填入，當下拖曳 {draggedObjects.Count} 個 Object：");

                    foreach (UnityEngine.Object draggedObject in draggedObjects)
                    {
                        if (draggedObject != null)
                        {
                            UnityEngine.Debug.Log(
                                $"- {draggedObject.name} ({draggedObject.GetType().Name})");
                        }
                    }
                }
                else if (newAsset != null)
                {
                    UnityEngine.Debug.Log($"第 {i + 1} 列已變更為：{newAsset.name}");
                }
                else
                {
                    UnityEngine.Debug.Log($"第 {i + 1} 列已清空。");
                }
            }
        }

        isCreateWindow = EditorGUILayout.Toggle("建立視窗", isCreateWindow);

    }
    private void RefreshSelectedAssets()
    {
        // 直接從目前清單分組，不依賴 assetsType。
        // assetsType 沒有同步更新時，排序結果會是空的；接著 Clear() 就會讓所有物件消失。
        List<UnityEngine.Object> sortedAssets = selectedAssets
            .Where(asset => asset != null)
            .GroupBy(asset => asset.GetType())
            .OrderByDescending(group => group.Count())
            .SelectMany(group => group)
            .ToList();

        // 不把 null 加回清單；按下 Refresh 後，空欄位會被清除。
        selectedAssets.Clear();
        selectedAssets.AddRange(sortedAssets);
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
