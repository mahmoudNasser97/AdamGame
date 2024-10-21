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
using UnityEngine.AI;
using UnityEditor.SceneManagement;

namespace AISystem.Manager
{
    public class AIOverviewManager : EditorWindow
    {
        bool checkedCivilGroups = false;
        Vector2 windowScrollPosition = Vector2.zero;
        float timer = 0f;


        [MenuItem("Window/AI/CIVIL-AI-SYSTEM/Overview Manager")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            AIOverviewManager window = (AIOverviewManager)EditorWindow.GetWindow(typeof(AIOverviewManager), false, "AI Overview Manager");
            window.Show();
        }


        // Start is called before the first frame update
        public void OnGUI()
        {
            if (!checkedCivilGroups)
            {
                createNodeSelectionWidget();
            }

            if(settings == null)
            {
                settings = Resources.Load<Settings>("System/Settings");
            }
            
            windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
            Info();
            Perform();
            EditorGUILayout.EndScrollView();


            if (timer > 5f)
            {
                checkedCivilGroups = false;
                timer = 0f;
            }

            timer += Time.deltaTime;
        }

        #region Info

        string wikiDesc = "The Civil Sytem and the rest of RPG-AI-SYSTEM has an online active Wiki which will explain all parts of the system. Below are some keys links";
        string moreInfoDesc = "More details on this can be found here:";

        void Info()
        {
            EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            WikiSection();
            WorkplaceSection();
            HouseSection();
            RegionBlurbSection();
            EditorGUI.indentLevel--;
        }

        void WikiSection()
        {
            EditorGUILayout.LabelField("Wiki", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(wikiDesc, EditorStyles.wordWrappedLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Core", EditorStyles.wordWrappedLabel);
            if (EditorGUILayout.LinkButton("Overview"))
            {
                Application.OpenURL("https://github.com/IsaacMulcahy/RPG-AI-SYSTEM-WIKI/wiki#overview");
            }
            if (EditorGUILayout.LinkButton("Civil System"))
            {
                Application.OpenURL("https://github.com/IsaacMulcahy/RPG-AI-SYSTEM-WIKI/wiki/Civil-System");
            }
            if (EditorGUILayout.LinkButton("Job System"))
            {
                Application.OpenURL("https://github.com/IsaacMulcahy/RPG-AI-SYSTEM-WIKI/wiki/Job-System");
            }
            if (EditorGUILayout.LinkButton("Discord"))
            {
                Application.OpenURL("https://discord.gg/WNJAjaCmAh");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        void WorkplaceSection()
        {
            EditorGUILayout.LabelField("Workplaces (Must be on Layer Civil)", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(moreInfoDesc, EditorStyles.wordWrappedLabel);
            if (EditorGUILayout.LinkButton("Wiki"))
            {
                Application.OpenURL("https://github.com/IsaacMulcahy/RPG-AI-SYSTEM-WIKI/wiki/Civil-System#workplaces");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        void HouseSection()
        {
            EditorGUILayout.LabelField("Houses (Must be on Layer Civil)", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(moreInfoDesc, EditorStyles.wordWrappedLabel);
            if (EditorGUILayout.LinkButton("Wiki"))
            {
                Application.OpenURL("https://github.com/IsaacMulcahy/RPG-AI-SYSTEM-WIKI/wiki/Civil-System#houses");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        #endregion

        #region Action Text

        string templateTooltip = "Select a scene object and all settings and components required to setup the type will be added to it";
        string fixSceneTooltip = "Used to make sure objects are on the correct layers and that the layers exist";
        string generatePrePopTooltip = "Run the population code and setup within the editor, this can lower start up resource usage and allow for more consistency between runs";
        string removePrePopTooltip = "Removes all NPCs from the scene and reset buildings and areas to be repopulated";

        void AddInTemplateAI()
        {
            EditorGUILayout.LabelField("NPC Template", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Used for creating new characters", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(new GUIContent("Add in NPC Template", templateTooltip)))
            {
                AddInNPCTemplate();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        void AddInTemplateItem()
        {
            EditorGUILayout.LabelField("Item Template", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Used for creating new Item", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(new GUIContent("Add in Item Template", templateTooltip)))
            {
                AddInItemTemplate();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        void AddInTemplateWorkplace()
        {
            EditorGUILayout.LabelField("Workplace Template", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Used for creating new Workplaces", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(new GUIContent("Add in Workplace Template", templateTooltip)))
            {
                
                AddInWorkplaceTemplate();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        void AddInTemplateHouse()
        {
            EditorGUILayout.LabelField("House Template", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Used for creating new Houses", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(new GUIContent("Add in House Template", templateTooltip)))
            {
                AddInHouseTemplate();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        void PrepopWorld()
        {
            EditorGUILayout.LabelField("Prepopulate World", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Generated in Editor instead of on start", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(new GUIContent("Generate", generatePrePopTooltip)))
            {
                PrePopulateWorld();
            }
            if (GUILayout.Button(new GUIContent("Remove", removePrePopTooltip)))
            {
                ResetWorld();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        void FixSceneBox()
        {
            EditorGUILayout.LabelField("Fix Scene", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Used for setting layers for objects", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(new GUIContent("Fix Scene", fixSceneTooltip)))
            {
                FixScene();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        void RegionBlurbSection()
        {
            EditorGUILayout.LabelField("Region", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(moreInfoDesc, EditorStyles.wordWrappedLabel);
            if (EditorGUILayout.LinkButton("Wiki"))
            {
                Application.OpenURL("https://github.com/IsaacMulcahy/RPG-AI-SYSTEM-WIKI/wiki/Civil-System#regions");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        #endregion

        #region Perform

        string helpboxSetupAI = "Some Items can be excluded from setup to allow for own implentation, these are still needed. Look at documentation to see how";
        string helpboxSetupAISteps = "After using the Setup, Create a Region (Below). The NPC Template can be used to setup your character and create a Prefab which then will need to be added to the region Character Pool";

        string setupSceneTooltip = "Used to add all base objects required to the current scene";

        string[] civilJobGroups;
        string[] civilNeedGroups;
        List<CivilManager> regions = new List<CivilManager>();
        int selectedJobGroup;
        int selectedNeedGroup;
        string newRegionName;
        Vector2 RegionScrollPosition = Vector2.zero;
        bool includeGameManager = true;

        static Settings settings;

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
            EditorGUILayout.LabelField("Performable Actions", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            SetupAISystemSection();
            AddInTemplateAI();
            AddInTemplateHouse();
            AddInTemplateWorkplace();
            AddInTemplateItem();
            PrepopWorld();
            FixSceneBox();
            RegionSection();
            EditorGUI.indentLevel--;
        }

        void SetupAISystemSection()
        {
            EditorGUILayout.LabelField("Setup AI in Scene", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox(helpboxSetupAI, MessageType.Warning);
            EditorGUILayout.BeginHorizontal();
            includeGameManager = EditorGUILayout.Toggle("Game Manager", includeGameManager);
            if (GUILayout.Button(new GUIContent("Create and Add Assets to Scene", setupSceneTooltip)))
            {
                AutoSetupScene();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(helpboxSetupAISteps, EditorStyles.wordWrappedLabel);
            EditorGUI.indentLevel--;
        }

        void RegionSection()
        {
            GetRegions();

            EditorGUILayout.LabelField("Region Management", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            RegionScrollPosition = EditorGUILayout.BeginScrollView(RegionScrollPosition, GUILayout.Width(position.width), GUILayout.Height(Mathf.Min(86 * regions.Count(), 340)));
            foreach (var region in regions)
            {
                DisplayRegion(region);
            }
            EditorGUILayout.EndScrollView();
            DisplayCreateRegion();


            EditorGUI.indentLevel--;
        }


        #endregion

        #region Region
        void CreateRegion()
        {
            string civilManagerRef = "System/Objects/Civil Manager";

            Quaternion rotation = new Quaternion();
            GameObject newCivilManager = Instantiate(Resources.Load<GameObject>(civilManagerRef), Vector3.zero, rotation);
            newCivilManager.name = newRegionName + " Civil Manager";
        }

        void DisplayCreateRegion()
        {
            EditorGUILayout.BeginHorizontal();

            newRegionName = EditorGUILayout.TextField(newRegionName);
            if (GUILayout.Button("Create Region"))
            {
                newRegionName = newRegionName.Replace(" Civil Manager", "");
                CreateRegion();
            }
            EditorGUILayout.EndHorizontal();
        }

        void DisplayRegion(CivilManager currentCivilManager)
        {
            GameObject civilManagerObject = currentCivilManager.gameObject;

            EditorGUILayout.LabelField(civilManagerObject.name, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            civilManagerObject.transform.transform.position = EditorGUILayout.Vector3Field("Location", civilManagerObject.transform.transform.position);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size:");
            currentCivilManager.areaSize = EditorGUILayout.FloatField(currentCivilManager.areaSize);
            if (GUILayout.Button("focus"))
            {
                Selection.activeGameObject = civilManagerObject;
                SceneView.FrameLastActiveSceneView();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        void GetRegions()
        {
            regions = FindObjectsOfType<CivilManager>().ToList();
        }

        #endregion

        #region Auto Setup Scene

        void AutoSetupScene()
        {
            // Game Manager
            if (includeGameManager)
            {
                var gameManager = Resources.Load<GameManager>("System/Objects/Game Manager");
                var instanceGameManager = Instantiate(gameManager, Vector3.zero, new Quaternion());

                instanceGameManager.name = instanceGameManager.name.Replace("(Clone)", "");
            }

            ObjectLayerHelper.GetLayer("Civil");
            ObjectLayerHelper.GetLayer("Resource");

            var orchestrator = Resources.Load<AIOrchestrator>("System/Objects/AIOrchestrator");
            var instanceOrchestrator = Instantiate(orchestrator, Vector3.zero, new Quaternion());

            instanceOrchestrator.name = instanceOrchestrator.name.Replace("(Clone)", "");
        }

        void AddInNPCTemplate()
        {
            GameObject selected = Selection.activeGameObject;

            if (selected != null)
            {
                selected.AddComponent<NavMeshAgent>();
                selected.AddComponent<AIController>();
                selected.AddComponent<AIDataBoard>();
                Animator animator = selected.AddComponent<Animator>();
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("System/Objects/AI");
            }
            else
            {
                Debug.LogWarning("No active Game Object");
            }
        }

        void AddInHouseTemplate()
        {
            GameObject selected = Selection.activeGameObject;

            if (selected != null && ObjectLayerHelper.GetLayer("Civil") != -1)
            {
                selected.AddComponent<House>();
                selected.layer = ObjectLayerHelper.GetLayer("Civil");
                CreateEntranceObj(selected.transform);
            }
            else
            {
                Debug.LogWarning("No active Game Object");
            }
        }

        void AddInWorkplaceTemplate()
        {
            GameObject selected = Selection.activeGameObject;

            if (selected != null && ObjectLayerHelper.GetLayer("Civil") != -1)
            {
                selected.AddComponent<Workplace>();
                selected.layer = ObjectLayerHelper.GetLayer("Civil");
                CreateEntranceObj(selected.transform);
            }
            else
            {
                Debug.LogWarning("No active Game Object");
            }
        }

        void AddInItemTemplate()
        {
            GameObject selected = Selection.activeGameObject;

            if (selected != null && ObjectLayerHelper.GetLayer("Resource") != -1)
            {
                selected.AddComponent<Item>();
                selected.layer = ObjectLayerHelper.GetLayer("Resource");
            }
            else
            {
                Debug.LogWarning("No active Game Object");
            }
        }

        public static void FixScene()
        {
            int civilLayer = ObjectLayerHelper.GetLayer("Civil");
            int resourceLayer = ObjectLayerHelper.GetLayer("Resource");

            Item[] itemList = GameObject.FindObjectsOfType<Item>();

            foreach (Item candidate in itemList)
            {
                candidate.gameObject.layer = resourceLayer;
            }

            House[] houseList = GameObject.FindObjectsOfType<House>();
            Workplace[] workplaceList = GameObject.FindObjectsOfType<Workplace>();

            foreach (House candidate in houseList)
            {
                candidate.gameObject.layer = civilLayer;
            }

            foreach (Workplace candidate in workplaceList)
            {
                candidate.gameObject.layer = civilLayer;
            }
            Debug.ClearDeveloperConsole();
        }

        void PrePopulateWorld()
        {

            CivilJobData.GetInstance();
            CivilNeedSystem.GetInstance();

            ResetWorld();

            foreach (CivilManager region in regions)
            {
                if (!region.HasCharacterPool())
                {
                    Debug.LogWarning("No Characters in character pool so was skipped", region);
                }
                else
                {
                    region.CreateRegion(true);
                }
            }

            EditorSceneManager.SaveOpenScenes();
        }

        public static void ResetWorld()
        {
            // Remove Npc's
            List<AIController> npcs = FindObjectsOfType<AIController>().ToList();

            foreach (AIController npc in npcs)
            {
                DestroyImmediate(npc.gameObject);
            }

            // Clear Houses
            List<House> houses = FindObjectsOfType<House>().ToList();

            foreach (House house in houses)
            {
                house.ResetHouse();
            }

            // Clear Workplaces
            List<Workplace> workplaces = FindObjectsOfType<Workplace>().ToList();

            foreach (Workplace workplace in workplaces)
            {
                workplace.ResetWorkplace();
            }

            // Clear Civil Managers
            List<CivilManager> civilManagers = FindObjectsOfType<CivilManager>().ToList();

            foreach(CivilManager manager in civilManagers)
            {
                manager.ResetState();
            }
        }

        void CreateEntranceObj(Transform selectedObj)
        {
            GameObject entranceObj = new GameObject();
            entranceObj.name = "entrance";
            entranceObj.transform.position = selectedObj.position;
            entranceObj.transform.SetParent(selectedObj);
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
    }
}

#endif