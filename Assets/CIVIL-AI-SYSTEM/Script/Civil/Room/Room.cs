using AISystem.Civil.Ownership;
using AISystem.ItemSystem;
using AISystem.Common;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AISystem.Civil
{
    public class Room : MonoBehaviour
    {
        public BoxCollider bounds;
        public CIVIL_ROOM type;
        public List<Item> items = new List<Item>();
        public AIDataBoard assigned;

#if UNITY_EDITOR

        public void CheckForItems()
        {
            if (assigned)
            {
                assigned.GetOwnershipManager().RemoveItems(OWNERSHIP.PERSONAL, items);
            }

            items.Clear();

            LayerMask layer = ObjectLayerHelper.GetLayerMaskExlude("Resource"); // This needs improving

            Collider[] candidates = Physics.OverlapBox(this.transform.position + bounds.center, bounds.size / 2, this.transform.rotation);

            foreach(Collider candidate in candidates)
            {
                Item item = candidate.GetComponent<Item>();

                if (item != null)
                {
                    items.Add(item);
                }
            }

            if (assigned)
            {
                assigned.GetOwnershipManager().Upsert(OWNERSHIP.PERSONAL, items);
            }
        }

        #region Editor

        private void OnDrawGizmosSelected()
        {
            
        }

        [CustomEditor(typeof(Room))]
        public class RoomInspector : Editor
        {
            bool displayItemList = false;

            public override void OnInspectorGUI()
            {
                Room candidate = (Room)target;

                candidate.type = (CIVIL_ROOM)EditorGUILayout.EnumPopup("Type", candidate.type);


                if (GUILayout.Button("Items"))
                {
                    displayItemList = !displayItemList;
                }

                if(displayItemList)
                {
                    for(int i = 0; i < candidate.items.Count; i++)
                    {
                        EditorGUILayout.ObjectField(candidate.items[i], typeof(Item), true);
                    }
                    
                }

                if (GUILayout.Button("Populate Items"))
                {
                    candidate.CheckForItems();
                }
            }

        }

        #endregion
#endif
    }
}
