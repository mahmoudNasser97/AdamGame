#if UNITY_EDITOR

using AISystem.Civil;
using AISystem.Common;
using AISystem.Civil.CivilAISystem.V2;
using AISystem.Civil.CivilAISystem.V2.Needs;
using AISystem.ItemSystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using AISystem.Menu;
using AISystem.EditorSystems;
using System;

namespace AISystem.Manager
{
    public class SettingsMenu : EditorWindow
    {
        bool checkedCivilGroups = false;
        Vector2 windowScrollPosition = Vector2.zero;
        float timer = 0f;

        ListMenuItem npcDistanceGrid = null;

        string settingsFullPath = "Assets/CIVIL-AI-SYSTEM/Resources/System/Settings.asset";

        [MenuItem("Window/AI/CIVIL-AI-SYSTEM/Settings")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            SettingsMenu window = (SettingsMenu)EditorWindow.GetWindow(typeof(SettingsMenu), false, "AI Settings");
            window.Show();
            window.currentSettings = null;
        }


        // Start is called before the first frame update
        public void OnGUI()
        {
            if (!checkedCivilGroups)
            {
                createNodeSelectionWidget();
            }

            if (currentSettings == null)
            {

                if (!LoadSettings())
                {
                    CreateDefaultSettings();
                }
            }

            windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
            Perform();
            EditorGUILayout.EndScrollView();


            if (timer > 5f)
            {
                checkedCivilGroups = false;
                timer = 0f;
            }

            if (npcDistanceGrid == null)
            {
                CreateNpcGroupingDistance();
            }

            EditorUtility.SetDirty(currentSettings);

            timer += Time.deltaTime;
        }

        #region Perform

        string helpboxWarning = "Select a group of civil data, this will delete any current ones within 'AI/Resources/type' but not in the 'inactive' or 'System'";
        string helpboxNavAgent = "Select NavMeshAgent version to use. To add your own check out the wiki section 'Customizing The System'. Errors will be shown in the console";

        string reimportTooltip = "Used to reload the currently selected node group";

        string[] civilJobGroups;
        string[] civilNeedGroups;
        int selectedJobGroup;
        int selectedNeedGroup;

        Settings currentSettings;

        void createNodeSelectionWidget()
        {
            civilJobGroups = CreateNodeGroups("JobSystem");
            civilNeedGroups = CreateNodeGroups("NeedSystem");
            selectedJobGroup = GetCurrentNodeGroup("JobSystem", civilJobGroups);
            selectedNeedGroup = GetCurrentNodeGroup("NeedSystem", civilNeedGroups);
            checkedCivilGroups = true;
        }

        void Perform()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            SettingSection();
            ImportJobSection();

            if (currentSettings.needSystemEnabled)
            {
                ImportNeedSection();
            }

            GroupingDistanceDisplay();
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        void SettingSection()
        {
            EditorGUILayout.LabelField("Nav Agent", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(helpboxNavAgent, MessageType.Info);

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Nav Mode:");
            NAV_MODE candidateNavMode = (NAV_MODE)EditorGUILayout.EnumPopup(currentSettings.navMode);
            if(candidateNavMode != currentSettings.navMode)
            {
                ChangeNavMode(currentSettings.navMode, candidateNavMode);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Speed:");
            currentSettings.speed = EditorGUILayout.FloatField(currentSettings.speed);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Acceleration:");
            currentSettings.acceleration = EditorGUILayout.FloatField(currentSettings.acceleration);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Angular Speed:");
            currentSettings.angularSpeed = EditorGUILayout.FloatField(currentSettings.angularSpeed);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Stopping Distance:");
            currentSettings.stoppingDistance = EditorGUILayout.FloatField(currentSettings.stoppingDistance);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;


            EditorGUILayout.LabelField("Systems", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Need System Enabled:");
            currentSettings.needSystemEnabled = EditorGUILayout.Toggle(currentSettings.needSystemEnabled);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;


            EditorGUILayout.LabelField("Performance", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Culling Distance:");
            currentSettings.renderDistance = EditorGUILayout.FloatField(currentSettings.renderDistance);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("NPC", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Culling Mode:");
            currentSettings.npcCullingMode = (NPC_CULLING_MODE)EditorGUILayout.EnumPopup(currentSettings.npcCullingMode);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Max Batch:");
            currentSettings.maxBatchSize = EditorGUILayout.IntField(currentSettings.maxBatchSize);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Distance Batching Tick Rate:"); // How often they are grouped into distance groups
            currentSettings.groupingTickRate = EditorGUILayout.FloatField(currentSettings.groupingTickRate);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tick Rate:"); // How often the main loops runs
            currentSettings.tickRate = EditorGUILayout.FloatField(currentSettings.tickRate);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Culling Tick Rate:"); // How often culling is updated
            currentSettings.cullingTickRate = EditorGUILayout.FloatField(currentSettings.cullingTickRate);
            EditorGUILayout.EndHorizontal();
            if (currentSettings.needSystemEnabled)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Need Tick Rate:"); // How often the need update is run
                currentSettings.needSystemTickRate = EditorGUILayout.FloatField(currentSettings.needSystemTickRate);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;

        }

        void ImportJobSection()
        {
            EditorGUILayout.LabelField("Import Job Node Group", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox(helpboxWarning, MessageType.Info);
            if (checkedCivilGroups)
            {
                int changedGroup;

                EditorGUILayout.BeginHorizontal();
                changedGroup = EditorGUILayout.Popup(selectedJobGroup, civilJobGroups);
                if (GUILayout.Button(new GUIContent("Reimport", reimportTooltip)))
                {
                    AIOverviewManager.ResetWorld();
                    CopyNodeGroup("JobSystem", "CIVIL_JOBS", civilJobGroups[changedGroup]);
                    CivilJobData.Reset();
                }
                EditorGUILayout.EndHorizontal();

                if (changedGroup != selectedJobGroup)
                {
                    if (changedGroup != 0)
                    {
                        AIOverviewManager.ResetWorld();
                        if (CopyNodeGroup("JobSystem", "CIVIL_JOBS", civilJobGroups[changedGroup]))
                        {
                            selectedJobGroup = changedGroup;
                        }
                    }
                    else
                    {
                        AIOverviewManager.ResetWorld();
                        string fullPath = Application.dataPath + "/Resources/JobSystem";
                        FileUtil.DeleteFileOrDirectory(fullPath + "/JobList.asset");
                        FileUtil.DeleteFileOrDirectory(fullPath + "/DutyList.asset");
                        FileUtil.DeleteFileOrDirectory(fullPath + "/ActionList.asset");
                        FileUtil.DeleteFileOrDirectory(fullPath + "/TaskMethodList.asset");
                        FileUtil.DeleteFileOrDirectory(fullPath + "/nodes.aiflowchart");
                    }
                }
            }
            EditorGUI.indentLevel--;
        }

        void ImportNeedSection()
        {
            EditorGUILayout.LabelField("Import Need Node Group", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox(helpboxWarning, MessageType.Info);
            if (checkedCivilGroups)
            {
                int changedGroup;

                EditorGUILayout.BeginHorizontal();
                changedGroup = EditorGUILayout.Popup(selectedNeedGroup, civilNeedGroups);

                if (GUILayout.Button(new GUIContent("Reimport", reimportTooltip)))
                {
                    AIOverviewManager.ResetWorld();
                    CopyNodeGroup("NeedSystem", "CIVIL_NEEDS", civilNeedGroups[changedGroup]);
                    CivilNeedSystem.Reset();
                }
                EditorGUILayout.EndHorizontal();

                if (changedGroup != selectedNeedGroup)
                {
                    if (changedGroup != 0)
                    {
                        AIOverviewManager.ResetWorld();
                        if (CopyNodeGroup("NeedSystem", "CIVIL_NEEDS", civilNeedGroups[changedGroup]))
                        {
                            selectedNeedGroup = changedGroup;
                        }
                    }
                    else
                    {
                        AIOverviewManager.ResetWorld();
                        string fullPath = Application.dataPath + "/Resources/NeedSystem";
                        FileUtil.DeleteFileOrDirectory(fullPath + "/NeedList.asset");
                        FileUtil.DeleteFileOrDirectory(fullPath + "/MethodList.asset");
                        FileUtil.DeleteFileOrDirectory(fullPath + "/ActionList.asset");
                        FileUtil.DeleteFileOrDirectory(fullPath + "/nodes.aiflowchart");
                    }
                }
            }
            EditorGUI.indentLevel--;
        }

        #endregion

        #region List

        void GroupingDistanceDisplay()
        {
            EditorGUILayout.LabelField("NPC Grouping Distance", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if (npcDistanceGrid != null)
            {
                Vector2 npcDistanceZone = new Vector2(position.width, 250);
                npcDistanceGrid.UpdateRendering(npcDistanceZone);
                npcDistanceGrid.Render();
                currentSettings.npcGroupingDistance = npcDistanceGrid.GetValues();
            }
            EditorGUI.indentLevel--;
        }

        void CreateNpcGroupingDistance()
        {
            Vector2 npcDistanceZone = new Vector2(position.width, 250);
            string[] columnHeaders = { "Group Id", "Distance < X", "" };
            npcDistanceGrid = new ListMenuItem(npcDistanceZone, 25, columnHeaders, currentSettings.npcGroupingDistance);
        }

        #endregion

        #region Node Group Control
        string[] CreateNodeGroups(string file)
        {
            List<string> nodeGroups = new List<string>();
            string fullPath = Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/" + file + "/Inactive";

            string[] groups = System.IO.Directory.GetFiles(fullPath);
            nodeGroups.Add("None");

            foreach (string group in groups)
            {
                string[] section = group.Split('/');

                nodeGroups.Add(section[section.Length - 1].Replace("Inactive\\", "").Replace(".meta", ""));
            }

            return nodeGroups.ToArray();
        }

        int GetCurrentNodeGroup(string system, string[] group)
        {
            int selectedGroup = 0;

            string fullPath = Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/" + system;

            string[] fileEntries = System.IO.Directory.GetFiles(fullPath);

            foreach (string fileName in fileEntries)
            {
                if (fileName.Contains(".aiflowchart"))
                {
                    string fileData = System.IO.File.ReadAllText(fileName);

                    List<string> lines = new List<string>(fileData.Split("\n"[0]));

                    for (int i = 0; i < group.Count(); i++)
                    {
                        if (group[i] == lines[0].Replace("\r", ""))
                        {
                            selectedGroup = i;
                            break;
                        }
                    }

                    if (lines[0].Length == 0)
                    {
                        selectedGroup = 0;
                    }

                    break;
                }
            }

            return selectedGroup;


        }

        void CreateEnum(string name, string path, string[] entires)
        {
            string relativePath = path + name + ".cs";
            string fullPath = Application.dataPath.Replace("/Assets/CIVIL-AI-SYSTEM", "") + "/" + relativePath;

            if (File.Exists(fullPath))
            {
                Debug.Log("Deleted old version of " + name + " file");
                File.Delete(fullPath);
            }

            using (StreamWriter streamWriter = new StreamWriter(relativePath))
            {
                streamWriter.WriteLine("namespace AISystem.Civil");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\t public enum " + name);
                streamWriter.WriteLine("\t {");
                for (int i = 0; i < entires.Length; i++)
                {
                    streamWriter.WriteLine("\t\t" + entires[i].Replace(" ", "_").ToUpper() + ",");
                }
                streamWriter.WriteLine("\t }");
                streamWriter.WriteLine("}");
            }
            AssetDatabase.Refresh();
        }

        string[] GetJobNames()
        {
            List<string> jobNames = new List<string>();

            jobNames.Add("NONE");

            JobList jobList = Resources.Load<JobList>("JobSystem/JobList");

            jobNames.AddRange(jobList.GetJobNames());

            return jobNames.ToArray();
        }

        string[] GetNeedNames()
        {
            List<string> names = new List<string>();

            names.Add("NONE");

            NeedList list = Resources.Load<NeedList>("NeedSystem/NeedList");

            names.AddRange(list.GetNames());

            return names.ToArray();
        }

        public bool CopyNodeGroup(string file, string enumName, string groupName)
        {
            string fullPath = Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/" + file;
            string DirectoryToCopy = fullPath + "/Inactive/" + groupName;

            string[] fileEntries = System.IO.Directory.GetFiles(DirectoryToCopy);

            foreach (string fileAddress in fileEntries)
            {
                string fileAddressCleaned = fileAddress.Replace('\\', '/');

                string[] addressBroken = fileAddressCleaned.Split('/');

                string filename = addressBroken[addressBroken.Length - 1];

                FileUtil.DeleteFileOrDirectory(fullPath + "/" + filename);

                FileUtil.CopyFileOrDirectory(fileAddressCleaned, fullPath + "/" + filename);

                AssetDatabase.Refresh();
            }

            Debug.Log("Updated Civil Groups - " + groupName);

            string[] names = null;

            if (enumName == "CIVIL_JOBS")
            {
                names = GetJobNames();
            }
            else
            {
                names = GetNeedNames();
            }

            if (names.Length > 0)
            {
                CreateEnum(enumName, "Assets/CIVIL-AI-SYSTEM/Script/Enums/", names);
                Debug.Log("Updated " + enumName + " Enum");
            }

            return true;
        }

        #endregion

        #region Setting Object Creation

        bool LoadSettings()
        {
            currentSettings = Resources.Load<Settings>("System/Settings");

            if(currentSettings == null)
            {
                return false;
            }
            
            CreateNpcGroupingDistance();

            return true;
        }

        void CreateDefaultSettings()
        {
            Settings newSettings = ScriptableObject.CreateInstance<Settings>();

            AssetDatabase.CreateAsset(newSettings, settingsFullPath);
            AssetDatabase.SaveAssets();

            currentSettings = newSettings;
        }

        #endregion

        #region Nav Mode Change

        void ChangeNavMode(NAV_MODE old, NAV_MODE candidate)
        {
            SymbolHandler symbolHandler = new SymbolHandler();
            bool cannotUpdate = false;


            if (old == NAV_MODE.ASTAR)
            {
                symbolHandler.RemoveSymbol("CAS_ASTAR_EXISTS");
            }

            if(old == NAV_MODE.AGENT_NAVIGATION)
            {
                symbolHandler.RemoveSymbol("CAS_AGENTNAV_EXISTS");
            }

            switch (candidate)
            {
                case NAV_MODE.ASTAR:
                    if (symbolHandler.CheckForDomain("Pathfinding"))
                    {
                        symbolHandler.AddSymbol("CAS_ASTAR_EXISTS");
                    }
                    else
                    {
                        cannotUpdate = true;
                    }
                    break;
                case NAV_MODE.AGENT_NAVIGATION:
                    if (symbolHandler.CheckForDomain("ProjectDawn.Navigation"))
                    {
                        symbolHandler.AddSymbol("CAS_AGENTNAV_EXISTS");
                    }
                    else
                    {
                        cannotUpdate = true;
                    }
                    break;
            }

            symbolHandler.UpdateSymbols();

            if(cannotUpdate)
            {
                var errorboxNavAgent = "Nav Mode " + candidate.ToString() + " not set to prevent error, have you imported the package?";
                Debug.LogWarning(errorboxNavAgent);
                return;
            }

            if (candidate == NAV_MODE.NONE)
            {
                return;
            }

            currentSettings.navMode = candidate;
        }

        #endregion

        
    }
}

#endif