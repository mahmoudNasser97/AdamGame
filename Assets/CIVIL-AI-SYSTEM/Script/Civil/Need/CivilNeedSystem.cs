using AISystem.Civil.CivilAISystem.V2.Needs;
using UnityEngine;

namespace AISystem.Civil
{
    public class CivilNeedSystem
    {
        private CivilNeedSystem() { }

        private static CivilNeedSystem _instance;

        public static CivilNeedSystem GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CivilNeedSystem();
                _instance.Setup();
            }

            return _instance;
        }

        public static void Reset()
        {
            _instance = null;
        }

        private NeedList needList;
        private MethodList methodList;
        private ActionList actionList;

        public void Setup()
        {
            needList = Resources.Load<NeedList>("NeedSystem/NeedList");
            Debug.Log(needList.nodes.Count.ToString() + " Need(s) loaded");

            methodList = Resources.Load<MethodList>("NeedSystem/MethodList");
            Debug.Log(methodList.nodes.Count.ToString() + " Method(s) loaded");

            actionList = Resources.Load<ActionList>("NeedSystem/ActionList");
            Debug.Log(actionList.nodes.Count.ToString() + " action(s) loaded");
        }

        public void Setup(NeedList needList, MethodList methodList, ActionList actionList)
        {
            this.needList = needList;
            this.methodList = methodList;
            this.actionList = actionList;
        }

        public NeedList GetNeeds()
        {
            return needList;
        }

        public MethodList GetMethods()
        {
            return methodList;
        }

        public ActionList GetActions()
        {
            return actionList;
        }
    };
}
