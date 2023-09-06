using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor.SceneManagement;


[InitializeOnLoadAttribute]
public static class IT4080Menu {
    private class OsHelper {
        const string MODE_NONE = "none";
        const string MODE_WIN = "win";
        const string MODE_OSX = "osx";

        public string mode = MODE_NONE;
        public string execExtension = "";
        public string defaultBuildPath = "";


        public OsHelper() {
#if UNITY_EDITOR_WIN
            InitWindows();
#elif UNITY_EDITOR_OSX
            InitOsx();
#endif
        }

        private void InitWindows() {
            mode = MODE_WIN;
            execExtension = "exe";
            defaultBuildPath = "c:\\temp\\unity_builds\\TheBuild.exe";
        }

        private void InitOsx() {
            mode = MODE_OSX;
            execExtension = "app";
            defaultBuildPath = "~/temp/unity_builds/TheBuild.app";
        }

        private string RunBuildCmdWin(string path, string log) {
            // In windows, you have to give it the full path to where the log should
            // go, it isn't cool like OSX.  So we have to construct the full path from
            // the path to the executable.  Then it's pretty much the same as OSX.
            string logPath = Path.GetDirectoryName(path);
            string fullLogPath = Path.Join(logPath, log);

            return $"{path} --logfile {fullLogPath}";
        }

        private string RunBuildCmdOsx(string path, string log) {
            // Uses open to launch the app so we do not have to know where the actual
            // executable is inside the .app bundle.  This also sets the logfile to
            // be the <app name>.log or whatever is specified in logfile.  The log
            // will be in the same directory as the applicaiton.  Each new run
            // overwrites the existing log file if it exists.
            return $"open -n {path} --args --logfile {log}";
        }

        public string RunBuildCommand(string path, string log) {
            string toReturn = "";
            if (mode == MODE_WIN) {
                toReturn = RunBuildCmdWin(path, log);
            } else if (mode == MODE_OSX) {
                toReturn = RunBuildCmdOsx(path, log);
            }
            return toReturn;
        }

        private BuildPlayerOptions BuildOptsOsx() {
            BuildPlayerOptions opts = new BuildPlayerOptions();
            opts.target = BuildTarget.StandaloneOSX;
            opts.targetGroup = BuildTargetGroup.Standalone;

            return opts;
        }

        private BuildPlayerOptions BuildOptsWin() {
            BuildPlayerOptions opts = new BuildPlayerOptions();
            opts.target = BuildTarget.StandaloneWindows;
            opts.targetGroup = BuildTargetGroup.Standalone;

            return opts;
        }

        public BuildPlayerOptions BuildOpts() {
            BuildPlayerOptions opts = new BuildPlayerOptions();
            if (mode == MODE_WIN) {
                opts = BuildOptsWin();
            } else if (mode == MODE_OSX) {
                opts = BuildOptsOsx();
            }
            return opts;
        }
    }




    public const string BUILD_PATH_PREF = "it4080hBuildPath";
    private const string NOT_SET = "<No Path Set>";
    private static OsHelper osHelper = new OsHelper();
    public const string VERSION = "0.2.0";

    static IT4080Menu() {
        if (EditorPrefs.HasKey(BUILD_PATH_PREF)) {
            Debug.Log($"IT4080 build to {EditorPrefs.GetString(BUILD_PATH_PREF)}");
        } else {
            EditorPrefs.SetString(BUILD_PATH_PREF, osHelper.defaultBuildPath);
        }
    }


    // ---------------
    // Private Methods
    // ---------------

    /*
     * Adapted from one of the repsonses on
     * https://answers.unity.com/questions/1128694/how-can-i-get-a-list-of-all-scenes-in-the-build.html
     * 
     * This will get all the scenes that have been configured in the build 
     * settings.  If you specify a path for runScenePath then it will put that
     * path at the start of the array instead of its default spot in the list 
     * (if it has one).
     */
    private static string[] getBuildScenes(string runScenePath = "") {
        List<string> scenes = new List<string>();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
            if (scene.enabled && scene.path != runScenePath) {
                scenes.Add(scene.path);
            }
        }

        if (runScenePath != "") {
            scenes.Insert(0, runScenePath);
        }

        return scenes.ToArray();
    }


    /*
     * Adapted from https://docs.unity3d.com/Manual/BuildPlayerPipeline.html
     */
    private static bool BuildTheBuild(string path, bool runCurrentScene = false) {
        BuildPlayerOptions opts = osHelper.BuildOpts();
        opts.locationPathName = path;   // This will make the path if it does not exist.
        if (runCurrentScene) {
            opts.scenes = getBuildScenes(EditorSceneManager.GetActiveScene().path);
        } else {
            opts.scenes = getBuildScenes();
        }

        BuildReport report = BuildPipeline.BuildPlayer(opts.scenes, path, opts.target, BuildOptions.Development);
        //BuildReport report = BuildPipeline.BuildPlayer(opts);

        bool success = false;
        if (report.summary.result == BuildResult.Succeeded) {
            string logmsg = $"Build succeeded: {path}";
            if (runCurrentScene) {
                logmsg += $"\n    Built scene {EditorSceneManager.GetActiveScene().path} instead of default scene";
            }
            Debug.Log(logmsg);
            success = true;
        } else if (report.summary.result == BuildResult.Failed) {
            Debug.Log("Build failed");
            success = false;
        }

        return success;
    }


    /*
     * Prompts the user for a path to a file to be used for the build.  Can
     * optionally provide the current path that is being used.
     *
     * This will set the the BUILD_PATH_PREF editor preference value to what
     * is chosen and also return that value.
     *
     * If the user cancels then the editor preference is not set and the current
     * value of the editor preference is returned.
     */
    private static string getBuildPathFromUser(string startDir = NOT_SET) {
        string fullPath = startDir;
        string curDir = "";
        string curFile = "";
        string ext = osHelper.execExtension;

        if (fullPath == NOT_SET) {
            fullPath = "";
        } else {
            curFile = Path.GetFileName(startDir);
            curDir = Path.GetFullPath(startDir);
        }

        string toReturn = EditorUtility.SaveFilePanel("Set Build Path", curDir, curFile, ext);

        if (toReturn.Length != 0) {
            EditorPrefs.SetString(BUILD_PATH_PREF, toReturn);
            Debug.Log($"Build path changed to {EditorPrefs.GetString(BUILD_PATH_PREF)}");
        } else {
            toReturn = EditorPrefs.GetString(BUILD_PATH_PREF);
            Debug.Log($"Canceled, path is {toReturn}");
        }

        return toReturn;
    }


    /*
     * Gets the build path from the user if they have not set it yet.  Otherwise
     * whatever they set it to is returned
     */
    private static string GetBuildPath() {
        var curPath = EditorPrefs.GetString(BUILD_PATH_PREF);

        if (curPath == NOT_SET) {
            curPath = getBuildPathFromUser(curPath);
        }

        return curPath;
    }


    private static string makeLogNameFromPath(string path, string extra = "") {
        string toReturn = $"{Path.GetFileNameWithoutExtension(path)}{extra}.log";
        return toReturn;
    }


    private static void RunApp(string path, string logfile = null) {
        var log = logfile;
        if (log == null) {
            log = makeLogNameFromPath(path);
        }
        string cmd = osHelper.RunBuildCommand(path, log);
        Debug.Log($"[running]:  {cmd}");
        ShellHelper.ProcessCommand(cmd, "/");
    }


    private static void RunX(int x = 1) {
        string curPath = EditorPrefs.GetString(BUILD_PATH_PREF);
        if (curPath != NOT_SET) {
            for (int i = 0; i < x; i++) {
                RunApp(curPath, makeLogNameFromPath(curPath, $"_{i + 1}"));
            }
        } else {
            Debug.LogWarning("Cannot run, build path not set.");
        }
    }


    private static void BuildThenRunX(int x = 1, bool runCurrentScene = false) {
        string curPath = GetBuildPath();
        if (curPath != NOT_SET) {
            bool result = BuildTheBuild(curPath, runCurrentScene);
            if (result) {
                RunX(x);
            }
        }
    }


    private static void ShowLogs() {
        string curPath = EditorPrefs.GetString(BUILD_PATH_PREF);
        var window = EditorWindow.GetWindow<It4080.LogViewer>();
        window.basePath = Path.Join(Path.GetDirectoryName(curPath), Path.GetFileNameWithoutExtension(curPath));
        window.ShowPopup();
        window.LoadLogs();
    }


    // ------------------------------------------------------------------------
    // Build Menus
    // ------------------------------------------------------------------------
    [MenuItem("IT4080/Set Build Path", false, 100)]
    private static void MnuSetBuildPath() {
        getBuildPathFromUser(EditorPrefs.GetString(BUILD_PATH_PREF));
    }

    [MenuItem("IT4080/Build", false, 100)]
    private static void MnuBuildTheBuild() {
        string curPath = GetBuildPath();
        // This handles the one case where it wasn't set and the user canceled
        // the dialog to set the build path.
        if (curPath != NOT_SET) {
            BuildTheBuild(curPath);
        }
    }

    [MenuItem("IT4080/Build Current Scene", false, 100)]
    private static void MnuBuildCurrentScene() {
        string curPath = GetBuildPath();
        // This handles the one case where it wasn't set and the user canceled
        // the dialog to set the build path.
        if (curPath != NOT_SET) {
            BuildTheBuild(curPath, true);
        }
    }


    //--------------------------------
    // Build and Run X Menus
    //--------------------------------
    [MenuItem("IT4080/Build & Run/1", false, 200)]
    private static void MnuBuildRun1() {
        BuildThenRunX(1);
    }

    [MenuItem("IT4080/Build & Run/2", false, 200)]
    private static void MnuBuildRun2() {
        BuildThenRunX(2);
    }

    [MenuItem("IT4080/Build & Run/3", false, 200)]
    private static void MnuBuildRun3() {
        BuildThenRunX(3);
    }

    [MenuItem("IT4080/Build & Run/4", false, 200)]
    private static void MnuBuildRun4() {
        BuildThenRunX(4);
    }


    //--------------------------------
    // Run X Menus
    //--------------------------------
    [MenuItem("IT4080/Run/1", false, 200)]
    private static void MnuRun1() {
        RunX(1);
    }

    [MenuItem("IT4080/Run/2", false, 200)]
    private static void MnuRun2() {
        RunX(2);
    }

    [MenuItem("IT4080/Run/3", false, 200)]
    private static void MnuRun3() {
        RunX(3);
    }

    [MenuItem("IT4080/Run/4", false, 200)]
    private static void MnuRun4() {
        RunX(4);
    }



    //--------------------------------
    // View Menus
    //--------------------------------
    // Disabling view logs for now since it goes back to it's default state
    // when scripts are reloaded.  I think this could be avoided if this was
    // an actual plugin instead of just files.  Also, it's a bit buggy and slow.
    //[MenuItem("IT4080/View Logs", false , 300)]
    //private static void MnuViewLogs() {
    //    ShowLogs();
    //}

    [MenuItem("IT4080/Show Files", false, 300)]
    private static void MnuViewFiles() {
        string buildFile = GetBuildPath();
        string parentDir = Path.GetDirectoryName(GetBuildPath());
        if (Directory.Exists(parentDir)) {
            EditorUtility.RevealInFinder(buildFile);
        } else {
            Debug.LogError($"Cannot open {parentDir} because it does not exist.  Kicking off a build will create the path.");
        }        
    }


    [MenuItem("IT4080/About")]
    private static void MnuAbout() {
        Debug.Log($"IT4080 Menu Version {VERSION}\n" +
            $"  builds to:  {EditorPrefs.GetString(IT4080Menu.BUILD_PATH_PREF)}");
    }

}
