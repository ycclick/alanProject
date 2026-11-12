using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class CheckGitformLogWindow : EditorWindow
{
    [SerializeField]
    public bool isCreateWindow;
    [SerializeField]
    private UnityEngine.Object selectedAsset;
    //[SerializeField]
    private Type asset;
    //跟目錄路徑
    private static string projectPath = Directory.GetParent(Application.dataPath).FullName;
    private static string gitStatusShort = "status --short";
    private static string gitBranch = "branch -a";
    private static string gitName = "git";
    private static string windowCMDName = "cmd.exe";
    private static string cmdKeepOpenOption = $"/k";

    [MenuItem("Tools/Git/CheckGit")]
    private static void Open()
    {
        //index = 0;
        GetWindow<CheckGitformLogWindow>("CheckGit");
        var info = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = "status --short",
            WorkingDirectory = projectPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        // 使用上面的設定啟動 Git 外部程序。
        // Process.Start(info) 會執行：git status --short。
        using (Process process = Process.Start(info))
        {
            // 讀取 Git 從標準輸出回傳的文字內容。
            // 例如：修改過的檔案或尚未追蹤的新檔案。
            string result = process.StandardOutput.ReadToEnd();

            // 等待 Git 程序執行完成，確保 ExitCode 已經可以取得。
            process.WaitForExit();

            // 將 Git 的執行結果與輸出內容寫入 Unity Console。
            // ExitCode 為 0 通常代表指令執行成功。
            UnityEngine.Debug.Log(
                $"Git exit code: {process.ExitCode}\n{result}");
        }
        // using 區塊結束後會自動釋放 Process 使用的資源。
    }

    private void OnGUI()
    {

        //isCerateWindow = EditorGUILayout.Foldout("isCerateWindow", isCerateWindow);

        // 在 Index 欄位與素材欄位之間加入垂直間距。
        EditorGUILayout.Space(10);

        DrawGUI();


        // 按下按鈕後，才將選取素材的資料輸出到 Unity Console。
        GUIButtonAction("InputObjectData", () => 
        {
            asset = selectedAsset?.GetType();
            // 沒有選取素材時，不繼續執行後面的資料讀取。
            if (selectedAsset == null)
            {
                UnityEngine.Debug.LogWarning("尚未選取素材。");
                return;
            }
            // 取得素材在 Unity 專案中的相對路徑，例如 Assets/Images/Icon.png。
            string assetPath = AssetDatabase.GetAssetPath(selectedAsset);

            // 將素材名稱與專案路徑輸出到 Unity Console。
            UnityEngine.Debug.Log(
                $"素材名稱：{selectedAsset.name}\n素材路徑：{assetPath}\n素材類別:{asset}");
        });
        GUIButtonAction("CheckGitStatus", () => ProcessGit(gitStatusShort, isCreateWindow, !isCreateWindow, !isCreateWindow));
        GUIButtonAction("CheckGitBranch", () => ProcessGit(gitBranch, isCreateWindow, !isCreateWindow, !isCreateWindow));
        GUIButtonAction("CheckGitBranch", () => ProcessGit(gitBranch, isCreateWindow, !isCreateWindow, !isCreateWindow));
        //GUIButtonAction("CheckGitBranch", () => ProcessGit(gitBranch, true, false, false));
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
            using (Process process = Process.Start(info))
            {
                // 讀取 Git 從標準輸出回傳的文字內容。
                //string result = process.StandardOutput.ReadToEnd();
                // 等待 Git 程序執行完成，確保 ExitCode 已經可以取得。
                process.WaitForExit();
                // 將 Git 的執行結果與輸出內容寫入 Unity Console。
                //UnityEngine.Debug.Log($"Git exit code: {process.ExitCode}\n{result}");
            }
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
        selectedAsset = EditorGUILayout.ObjectField("素材", selectedAsset, typeof(UnityEngine.Object), false);
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
