using UnityEditor;
using UnityEngine;
using AISystem.Civil.Objects.V2;
using AISystem.Civil.Iterators;
using AISystem.Civil.Ownership;

#if UNITY_EDITOR
using AISystem.Flowchart.V1;

namespace AISystem.Flowchart.JobSystem
{
    public class ActionNode : Node
    {
        Action action;
        float width = 360;
        float height = 265;


        public ActionNode(Vector2 mousePosition) : base(mousePosition, "Action", out System.Guid entry_id)
        {
            action = new Action(ACTIONS_TYPES.IDLE, ITEMS.NULL, ITEMS_TYPE.NULL, null, LOOKING_TYPES.NONE, OWNERSHIP.ALL, false, false);
            Setup(mousePosition);
        }

        public ActionNode(Vector2 mousePosition, Action actionRef) : base(mousePosition, "Action", out System.Guid entry_id)
        {
            action = actionRef;
            Setup(mousePosition);
        }

        void Setup(Vector2 mousePosition)
        {
            node = new Rect(mousePosition.x, mousePosition.y, width, height);
            itemId = System.Guid.Parse(action.id);
            lowestNodeType = true;

            SetupLocalWeightingState();
        }

        public override void DisplayData()
        {
            EditorGUILayout.TextField(action.id.ToString(), textStyle);

            action.ActionType = (ACTIONS_TYPES)EditorGUILayout.EnumPopup("Action", action.ActionType, buttonStyle);
            action.itemsNeeded = (ITEMS)EditorGUILayout.EnumPopup("Item Needed", action.itemsNeeded, buttonStyle);
            action.itemType = (ITEMS_TYPE)EditorGUILayout.EnumPopup("Item Type Needed", action.itemType, buttonStyle);
            action.itemOutput = (GameObject)EditorGUILayout.ObjectField("Item Output", action.itemOutput, typeof(GameObject), false);
            action.ownershipMode = (OWNERSHIP)EditorGUILayout.EnumPopup("Item Ownership", action.ownershipMode, buttonStyle);
            action.LookAt = (LOOKING_TYPES)EditorGUILayout.EnumPopup("Look", action.LookAt, buttonStyle);
            EditorGUILayout.BeginHorizontal();
            action.SetItemInUse = EditorGUILayout.Toggle("Set Item in Use", action.SetItemInUse);
            action.NoResetItemOnEnd = EditorGUILayout.Toggle("No Reset of Item in Use", action.NoResetItemOnEnd);
            EditorGUILayout.EndHorizontal();
            action.UpdateEachLoop = EditorGUILayout.Toggle("Update Each Loop", action.UpdateEachLoop);

            DisplayWeighting(action);
        }

        public override bool CheckConnectionAllowed(string childType)
        {

            return false;
        }

        public override string getName()
        {
            return null;
        }

        public override NODE_ITERATOR GetIterator()
        {
            return action.iterator;
        }

        public override void SetIterator(NODE_ITERATOR iterator)
        {
            action.iterator = iterator;
        }

        public override BaseNode GetNode()
        {
            return action;
        }

        public Action GetAction()
        {
            return action;
        }
    }
}
#endif