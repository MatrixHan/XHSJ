using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor.Callbacks;
using Hugula.Editor;
using Hugula.Utils;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using Seven;

public class ProjectBuild : Editor
{

    #region console par
    ///<summary>
    /// setting
    /// release 发布版本
    /// obb Android apk extions files
    ///</summary>
    public static string setting
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("setting"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }

    ///<summary>
    /// 编译宏
    ///</summary>
    public static string defineSymbols
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("define"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }

    ///<summary>
    /// BundleId 包名
    ///</summary>
    public static string bundleId
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("bundle"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }

    ///<summary>
    /// 签名team id
    ///</summary>
    public static string signingTeamId
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("sign"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }

    ///<summary>
    /// appName only android
    ///</summary>
    public static string productName
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("product"))
                {
                    return arg.Split(":"[0])[1];
                }
            }
            return string.Empty;
        }
    }

    #endregion

    private static StringBuilder environmentVariable = new StringBuilder();

    private static bool exportingAndroidProject = false;
    /// <summary>
    /// 在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
    /// </summary>
    static string[] GetBuildScenes()
    {
        var ve = setting;//发布release或者dev版本

        List<string> names = new List<string>();
#if HUGULA_RELEASE
        names.Add("Assets/Scene/s_first.unity");
#else
        if (setting.Contains("dev_scene"))
            names.Add("Assets/Scene/s_first.unity");
        else
            names.Add("Assets/Scene/s_first.unity");
#endif
        return names.ToArray();
    }

    static void BuildSlua()
    {
        SLua.LuaCodeGen.GenerateAll();
    }

    #region 构建分级


    ///<summary>
    /// 设置编译命令
    ///</summary>
    static void ScriptingDefineSymbols()
    {
        Debug.Log(defineSymbols);
        var _defineSymbols = defineSymbols.Replace(",", ";");
        Debug.Log(_defineSymbols);

#if UNITY_IOS
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, _defineSymbols);
#elif UNITY_ANDROID
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _defineSymbols);
#elif UNITY_STANDALONE
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, _defineSymbols);
#endif

    }

    static void ExportRes()
    {
        CUtils.DebugCastTime("Time ExportRes Begin");
        ExportResources.exportPublish();//资源
        CUtils.DebugCastTime("Time exportPublish End");
    }

    static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
    {
        AssetDatabase.Refresh();
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);

        if (EditorUserBuildSettings.development)
            build_options |= BuildOptions.Development;
        if (EditorUserBuildSettings.connectProfiler)
            build_options |= BuildOptions.ConnectWithProfiler;
        if (EditorUserBuildSettings.allowDebugging)
            build_options |= BuildOptions.AllowDebugging;

        string res = "";// BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);

        if (res.Length != 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
    }
    static void WriteAppVerion(string folder = "apk", string appExtension = "il2cpp")
    {
        System.Environment.SetEnvironmentVariable("APP_VERSION", Hugula.CodeVersion.APP_VERSION); //app version
        environmentVariable.AppendFormat("APP_VERSION={0}\n", Hugula.CodeVersion.APP_VERSION);
        environmentVariable.AppendFormat("CODE_VERSION={0}\n", Hugula.CodeVersion.CODE_VERSION);
        environmentVariable.AppendFormat("APP_NUMBER={0}\n", Hugula.CodeVersion.APP_NUMBER);

        var fileName = Hugula.CodeVersion.APP_VERSION;
#if HUGULA_RELEASE
        environmentVariable.AppendFormat("APP_STATE={0}\n", "release");
        fileName += "_release_";
#else
        environmentVariable.AppendFormat("APP_STATE={0}\n", "dev");
        fileName += "_dev_";
#endif
        fileName += appExtension;
        environmentVariable.AppendFormat("APP_FILENAME={0}\n", fileName);
        WriteShell();
    }

    static void WriteShell()
    {
        string path = Path.GetFullPath("shell");
        EditorUtils.DirectoryDelete(path);
        EditorUtils.CheckDirectory(path);
        string shellPath = Path.Combine(path, "ftp_release_shell.sh");
        StringBuilder ftpTampleta = new StringBuilder();
        ftpTampleta.Append("function foo()\n{\nlocal r\nlocal a\nr=\"$@\"\nwhile [[ \"$r\" != \"$a\" ]] ; do\na=${r%%/*}\necho \"mkdir $a\"\necho \"cd $a\"\nr=${r#*/}\ndone\n}");
        ftpTampleta.Append("\nfunction upload_ftp()\n{\necho \"current folder \"$1\necho \"upload to \"$FTP_ROOT$2\nftp -niv <<- EOF\nopen $FTP_IP\nuser $FTP_USER $FTP_PWD\nlcd $1\n$(foo \"$FTP_ROOT$2\")\ncd $FTP_ROOT$2\nbin\nhash\npwd\nprompt off\nmput *.*\nclose\nbye\nEOF\n}");
        var codeVersion = "";
        if (Hugula.HugulaSetting.instance.appendCrcToFile)
            codeVersion = Hugula.CodeVersion.CODE_VERSION.ToString();
        else
            codeVersion = Hugula.CodeVersion.APP_NUMBER.ToString(); //文件crc 不变路径需要改变
        string firstPackagePath = SplitPackage.UpdateOutVersionPath;// release/android/v9000 
        string ftpToPath = CUtils.platform + "/v" + codeVersion;
        StringBuilder content1 = new StringBuilder();
        content1.AppendFormat("\nupload_ftp {0} {1}", firstPackagePath, ftpToPath);//eg release/android/v9000 to android/v9001
        content1.AppendFormat("\nupload_ftp {0} {1}", firstPackagePath, string.Format("{0}/v{1}", CUtils.platform, Hugula.CodeVersion.CODE_VERSION));//eg release/android/v9000 to android/v9000
        content1.AppendFormat("\nupload_ftp {0} {1}", Path.Combine(firstPackagePath, "res"), Path.Combine(ftpToPath, "res"));//eg release/android/v9000/res to android/v9001/res
        content1.AppendFormat("\nupload_ftp {0} {1}", Path.Combine(firstPackagePath, "res/battle"), Path.Combine(ftpToPath, "res/battle"));//eg release/android/v9000/res to android/v9001/res

        using (StreamWriter sw = new StreamWriter(shellPath, false, new UTF8Encoding(false)))
        {
            sw.Write(ftpTampleta.ToString());
            sw.Write(content1.ToString());
        }
        Debug.Log("Create shell success " + shellPath);

        string ftpVerPath = Path.Combine(path, "ftp_dev_shell.sh");
        firstPackagePath = SplitPackage.UpdateOutVersionDevelopPath;
        content1 = new StringBuilder();
        content1.AppendFormat("\nupload_ftp {0} {1}", firstPackagePath, ftpToPath);//eg dev/android/v9000 to android/v9001
        content1.AppendFormat("\nupload_ftp {0} {1}", firstPackagePath, string.Format("{0}/v{1}", CUtils.platform, Hugula.CodeVersion.CODE_VERSION));//eg dev/android/v9000 to android/v9000

        using (StreamWriter sw = new StreamWriter(ftpVerPath, false, new UTF8Encoding(false)))
        {
            sw.Write(ftpTampleta.ToString());
            sw.Write(content1.ToString());
        }
        Debug.Log("Create shell success " + ftpVerPath);

        //environment_shell.sh
        string environmentPath = Path.Combine(path, "environment_shell.sh");

        using (StreamWriter sw = new StreamWriter(environmentPath, false, new UTF8Encoding(false)))
        {
            sw.Write(environmentVariable.ToString());
        }

        environmentVariable = new StringBuilder();
        Debug.Log("Create shell success " + environmentPath);
    }

    static void AndroidSettings(string appExtension = "il2cpp")
    {
        if (!string.IsNullOrEmpty(productName))
            PlayerSettings.productName = productName;

#if UNITY_2017
        if (!string.IsNullOrEmpty(bundleId))
            PlayerSettings.applicationIdentifier = bundleId;
#else
        if (!string.IsNullOrEmpty(bundleId))
            PlayerSettings.applicationIdentifier = bundleId;
#endif

        PlayerSettings.Android.bundleVersionCode = Hugula.CodeVersion.APP_NUMBER;
        var ve = setting;//发布release或者dev版本
        if (ve.ToLower().Contains("obb") || ve.ToLower().Contains("apkexpansionfiles"))
        {
            PlayerSettings.Android.useAPKExpansionFiles = true;
        }
        else
        {
            PlayerSettings.Android.useAPKExpansionFiles = false;
        }

        //write app version
        WriteAppVerion("apk", appExtension);

    }

    static void IOSSettings()
    {
        if (!string.IsNullOrEmpty(productName))
            PlayerSettings.productName = productName;
        PlayerSettings.iOS.buildNumber = Hugula.CodeVersion.APP_NUMBER.ToString();
        if (!string.IsNullOrEmpty(signingTeamId))
            PlayerSettings.iOS.appleDeveloperTeamID = signingTeamId;

        //write app version
        WriteAppVerion("ipa");
    }


    #endregion

     [MenuItem("Hugula/Android publish ", false, 16)]
    static void BuildForAndroid()
    {
//		ExportRes ();
        string path = "七煞神将.apk";
        path = Path.GetFullPath(path);
        exportingAndroidProject = false;
        //copy android plugins
        AndroidSettings("mono");
        PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.Mono2x, BuildTargetGroup.Android);
        if (setting.ToLower().Contains("development"))
            GenericBuild(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.Development);
        else
            GenericBuild(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
        CUtils.DebugCastTime("Time BuildForAndroid End");

    }


    // [MenuItem("Hugula/Android iL2CPP publish ", false, 16)]
    static void BuildForAndroidIL2CPP()
    {
        string path = "hugula.apk";
        path = Path.GetFullPath(path);
        exportingAndroidProject = false;
        //copy android plugins
        AndroidSettings();
        PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.IL2CPP, BuildTargetGroup.Android);
        if (setting.ToLower().Contains("development"))
            GenericBuild(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.Development);
        else
            GenericBuild(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
        PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.Mono2x, BuildTargetGroup.Android);
        CUtils.DebugCastTime("Time BuildForAndroid iL2CPP End");

    }


    // [MenuItem("Hugula/Android Project publish ", false, 16)]
    static void BuildForAndroidProject()
    {
        string path = "../../release/android";
        path = Path.GetFullPath(path);
        exportingAndroidProject = true;
        AndroidSettings("mono");
        EditorUtils.DirectoryDelete(path);
        EditorUtils.CheckDirectory(path);
        if (setting.ToLower().Contains("development"))
            GenericBuild(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.Development | BuildOptions.AcceptExternalModificationsToPlayer);
        else
            GenericBuild(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);

        CUtils.DebugCastTime("Time BuildForAndroidProject End");

    }

    // [MenuItem("Hugula/Android IL2CPP Project publish ", false, 16)]
    static void BuildForAndroidProjectIL2CPP()
    {
        string path = "../../release/android_il2cpp";
        path = Path.GetFullPath(path);
        exportingAndroidProject = true;
        AndroidSettings();
        EditorUtils.DirectoryDelete(path);
        EditorUtils.CheckDirectory(path);
        PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.IL2CPP, BuildTargetGroup.Android);
        if (setting.ToLower().Contains("development"))
            GenericBuild(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.Development | BuildOptions.AcceptExternalModificationsToPlayer);
        else
            GenericBuild(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
        PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.Mono2x, BuildTargetGroup.Android);
        CUtils.DebugCastTime("Time BuildForAndroidProjectIL2CPP End");


    }

    // [MenuItem("Hugula/IOS Publish  ", false, 16)]
    static void BuildForIOS()
    {
        CUtils.DebugCastTime("Time BuildForIOS Begin");

        string path = "../../release/ios";
        path = Path.GetFullPath(path);
        IOSSettings();
        EditorUtils.DirectoryDelete(path);
        EditorUtils.CheckDirectory(path);
        if (setting.ToLower().Contains("development"))
            GenericBuild(GetBuildScenes(), path, BuildTarget.iOS, BuildOptions.Development);
        else
            GenericBuild(GetBuildScenes(), path, BuildTarget.iOS, BuildOptions.None);
        CUtils.DebugCastTime("Time BuildForIOS End");
    }

    //[MenuItem("Hugula/project  StandaloneWindows ", false, 16)]
    static void BuildForWindows()
    {
        string path = "../../release/pc/";
        EditorUtils.DirectoryDelete(path);
        EditorUtils.CheckDirectory(path);
        WriteAppVerion();
        path = path + "hugula.exe";
        BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.StandaloneWindows, BuildOptions.None);
    }



    //发布成功后copy 资源到项目
    [PostProcessBuild(1000)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        Debug.LogFormat("BuildTarget = {0} , path = {1}", target, pathToBuiltProject);
    }

	// 导出rgba分离ui和图片
	public static void ExportRGBAndA()
	{
		TexturePackerBuild.BuildTextruePT (TexturePackerBuild.GetInputPath());
		TexturePackerBuild.BuildTextruePT (TexturePackerBuild.GetEffectImgInputPath());
		SplitAlphaAndRGB.SplitRgbAndAlphaChannel ();
		SplitAlphaAndRGB.ChangeUI ();
		EffectTools.Run ();
	}

	public static void CopyDirectory(string sourcePath, string destinationPath, bool copyMeta = false)
    {
        if (!Directory.Exists(sourcePath)) return;

        DirectoryInfo info = new DirectoryInfo(sourcePath);
        Directory.CreateDirectory(destinationPath);
        foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
        {
            string destName = Path.Combine(destinationPath, fsi.Name);
            if (fsi is System.IO.FileInfo)
            {
				if (copyMeta || !fsi.Extension.Equals(".meta"))
                    File.Copy(fsi.FullName, destName, true);
            }
            else
            {
                Directory.CreateDirectory(destName);
				CopyDirectory(fsi.FullName, destName, copyMeta);
            }
        }
    }

    static void DeleteDirectory(string targetDir)
    {
        string[] files = Directory.GetFiles(targetDir);
        string[] dirs = Directory.GetDirectories(targetDir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }
    }
}
