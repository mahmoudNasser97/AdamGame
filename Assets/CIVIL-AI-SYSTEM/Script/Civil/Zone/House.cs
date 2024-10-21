using UnityEngine;
using System.Collections.Generic;
using AISystem.Civil.Ownership;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AISystem.Civil
{
    public class House : MonoBehaviour
    {
        [SerializeField] int resident;
        [SerializeField] int maxResidents = 5;
        [SerializeField] Vector3 entrance;
        [SerializeField] bool useRoomSystem = false;
        [SerializeField] List<Room> rooms = new List<Room>();


        public House(int maxResidents, Vector3 entrance)
        {
            this.maxResidents = maxResidents;
            this.entrance = entrance;
        }

        public void Start()
        {
            Transform entranceObj = gameObject.transform.Find("entrance");

            if (entranceObj != null && entranceObj.gameObject.activeSelf)
            {
                entrance = gameObject.transform.Find("entrance").position;
            }
        }

        public int getMax()
        {
            return maxResidents;
        }

        public bool isFull()
        {
            if (resident < maxResidents)
            {
                return false;
            }

            return true;
        }

        public bool addResident(AIDataBoard databoard)
        {
            if (resident < maxResidents)
            {
                if (useRoomSystem)
                {
                    foreach (Room room in rooms)
                    {
                        if (room.assigned == null)
                        {
                            room.assigned = databoard;
                            databoard.AddRoom(room);
                            resident++;
                            return true;
                        }
                    }

                    Debug.LogWarning("Resident missing bedroom", databoard);

                }

                resident++;
                return true;
            }

            return false;
        }

        public Vector3 GetEntance()
        {
            return entrance;
        }

        public void ResetHouse()
        {
            resident = 0;
        }

        public void SetMax(int value)
        {
            maxResidents = value;
        }

#if UNITY_EDITOR

        #region Editor

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(entrance, 0.2f);

            Transform entranceObj = gameObject.transform.Find("entrance");

            if (entranceObj != null && entranceObj.gameObject.activeSelf)
            {
                entrance = gameObject.transform.Find("entrance").position;
            }
        }

        [CustomEditor(typeof(House))]
        public class HouseInspector : Editor
        {
            float timer = 0f;
            char targetIcon = '⊗';

            public override void OnInspectorGUI()
            {
                House house = (House)target;


                if (timer > 5f)
                {
                    house.rooms = new List<Room>(house.transform.GetComponentsInChildren<Room>());
                    timer = 0f;
                }

                timer += Time.deltaTime;


                CIVIL_ROOM roomCandidate = CIVIL_ROOM.BEDROOM;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.IntField("Resident (current/max)", house.resident);
                house.SetMax(EditorGUILayout.IntField(house.maxResidents));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Rooms");
                house.useRoomSystem = EditorGUILayout.Toggle("Enable Room System", house.useRoomSystem);

                if (house.useRoomSystem)
                {
                    EditorGUI.indentLevel++;

                    foreach (var room in house.rooms)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(targetIcon.ToString()))
                        {
                            Selection.activeGameObject = room.gameObject;
                        }

                        room.type = (CIVIL_ROOM)EditorGUILayout.EnumPopup(room.type);
                        EditorGUILayout.IntField(room.items.Count);
                        if (GUILayout.Button("Gather Item"))
                        {
                            room.CheckForItems();
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    roomCandidate = (CIVIL_ROOM)EditorGUILayout.EnumPopup("Room Type", roomCandidate);
                    if (GUILayout.Button("Create Room"))
                    {
                        CreateRoom(roomCandidate, house);
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }


            void CreateRoom(CIVIL_ROOM roomType, House house)
            {
                GameObject candidate = new GameObject("Room - " + roomType.ToString());
                candidate.transform.parent = house.transform;
                candidate.transform.localPosition = Vector3.zero;
                Room room = candidate.AddComponent<Room>();
                room.bounds = candidate.AddComponent<BoxCollider>();
                room.bounds.isTrigger = true;

                house.rooms.Add(room);
            }
        }

        #endregion
#endif
    }
}