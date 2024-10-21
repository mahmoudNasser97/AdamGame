﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading;

namespace MeshDeformer
{
    [System.Serializable]
    public class DeformChunk
    {
        //list of indexes to currentVertices
        public List<int> vertexIndexes = new List<int>();

        // the bound sof the chunk used for center, position and intersects
        public Bounds chunkBounds;
    }

    public class MeshDeformer : MonoBehaviour
    {
        [SerializeField, Tooltip("If this is not set, we will try to use one attached to this Gameobject")]
        MeshFilter meshToDeform;

        [Space]
        [SerializeField, Tooltip("Use collisions as a source of deformation")]
        bool useCollisions = true;

#if UNITY_2019_1_OR_NEWER
        [Space]
        [SerializeField, Tooltip("Shjould the mesh collider be updated. This runs on a separate thread but can be expensive")]
        bool updateMeshCollider = true;
        [SerializeField, Tooltip("If UpdateMeshCollider is true, this is the mesh collider that should be updated. If this is not set, we will try to use one attached to this Gameobject")]
        MeshCollider meshColliderToUpdate;
#endif
        [Space]
        [SerializeField, Tooltip("Game objects that will not cause deformation via collision")]
        List<GameObject> ignoreObjects = new List<GameObject>();
        List<GameObject> tempIgnoreObjects = new List<GameObject>();

        [Space]
        [SerializeField, Tooltip("Enable to allow for automated chunking to reduce runtime compute times")]
        bool useChunking = true;

        [SerializeField, Tooltip("The maximum vertices per chunk, overpopulated chunks will be split in half"), Range(10, 10000)]
        private int maxVertsPerChunk = 100;

        [Space]
        [SerializeField]
        float minCollisionVelocity = 3f;
        [SerializeField, Tooltip("Maximum clamp for collision velocity")]
        float maxCollisionVelocity = 30f;
        [SerializeField, Tooltip("A range which the collision velocity will be scaled to")]
        float deformationMagnitudeScale = 1;

        [Space]
        [SerializeField, Tooltip("Threading may not be available on all platforms")]
        bool useThreading = true;

        List<DeformChunk> deformChunks = new List<DeformChunk>();

        Mesh baseMesh;
        Mesh deformingMesh;
        List<Vector3> currentVertices = new List<Vector3>();
        List<int> currentTriangles = new List<int>();

#if UNITY_2019_1_OR_NEWER
        UnityEngine.Rendering.SubMeshDescriptor[] submeshes;
#endif
        Thread thread;
        Thread chunkThread;

        void Start()
        {
            if (meshToDeform == null)
            {
                meshToDeform = GetComponent<MeshFilter>();
            }

            baseMesh = meshToDeform.sharedMesh;
            deformingMesh = meshToDeform.mesh;

            currentVertices = deformingMesh.vertices.ToList();
            currentTriangles = deformingMesh.triangles.ToList();

#if UNITY_2019_1_OR_NEWER
            submeshes = new UnityEngine.Rendering.SubMeshDescriptor[meshToDeform.mesh.subMeshCount];
            for(int submeshIndex = 0; submeshIndex < meshToDeform.mesh.subMeshCount; submeshIndex++)
            {
                submeshes[submeshIndex] = meshToDeform.mesh.GetSubMesh(submeshIndex);
            }
#endif
            BeginChunkSetUp();

#if UNITY_2019_1_OR_NEWER
            if (useCollisions && meshColliderToUpdate == null)
            {
                meshColliderToUpdate = GetComponent<MeshCollider>();
            }
#endif
        }

        void BeginChunkSetUp()
        {
            Bounds meshBounds = deformingMesh.bounds;

            if (useThreading)
            {
                if (chunkThread != null && chunkThread.IsAlive)
                {
                    chunkThread.Abort();
                }

                chunkThread = new Thread(() => ChunkSetUp(meshBounds));
                chunkThread.Start();
            }
            else
            {
                ChunkSetUp(meshBounds);
            }
        }

        void ChunkSetUp(Bounds _meshBounds)
        {
            List<DeformChunk> newChunks = new List<DeformChunk>();
            if (useChunking)
            {
                // Prep for bounds
                _meshBounds.size *= 1.01f; // increase size to ensure we include all verts

                Vector3 chunkSize = _meshBounds.size / 2;
                Vector3 chunkStartPos = _meshBounds.center + (_meshBounds.size / 2) - (chunkSize / 2);

                Vector3 curChunkPos = chunkStartPos;
                Bounds curBounds = new Bounds(curChunkPos, chunkSize);

                // populate minimum number of chunks
                List<int> availableVertIndexes = new List<int>();

                for (int i = 0; i < currentVertices.Count; i++)
                {
                    availableVertIndexes.Add(i);
                }

                for (int i_x = 0; i_x < 2; i_x++)
                {
                    for (int i_y = 0; i_y < 2; i_y++)
                    {
                        for (int i_z = 0; i_z < 2; i_z++)
                        {
                            curBounds.center = curChunkPos;

                            DeformChunk newChunk = new DeformChunk();
                            newChunk.chunkBounds = new Bounds(curChunkPos, curBounds.size);

                            bool chunkUsed = false;

                            for (int vertIndex = 0; vertIndex < availableVertIndexes.Count; vertIndex++)
                            {
                                if (curBounds.Contains(currentVertices[availableVertIndexes[vertIndex]]))
                                {
                                    newChunk.vertexIndexes.Add(availableVertIndexes[vertIndex]);
                                    chunkUsed = true;
                                }
                            }

                            if (chunkUsed)
                            {
                                newChunks.Add(newChunk);

                                for (int usedIndex = 0; usedIndex < newChunk.vertexIndexes.Count; usedIndex++)
                                {
                                    availableVertIndexes.Remove(newChunk.vertexIndexes[usedIndex]);
                                }
                            }

                            curChunkPos -= new Vector3(0, 0, chunkSize.z);
                        }
                        curChunkPos -= new Vector3(0, chunkSize.y, -(chunkSize.z * 2));
                    }
                    curChunkPos -= new Vector3(chunkSize.x, -(chunkSize.y * 2), 0);
                }

                List<DeformChunk> newSplitChunks = new List<DeformChunk>();
                List<DeformChunk> oldChunksToRemove = new List<DeformChunk>();

                // check for overpopulated chunks
                foreach (DeformChunk chunk in newChunks)
                {
                    if (chunk.vertexIndexes.Count <= maxVertsPerChunk)
                    {
                        continue;
                    }

                    oldChunksToRemove.Add(chunk);
                    newSplitChunks.AddRange(SplitChunk(chunk));
                }

                foreach (DeformChunk chunk in oldChunksToRemove)
                {
                    newChunks.Remove(chunk);
                }

                newChunks.AddRange(newSplitChunks);
            }
            else
            {
                // Chunks will not be used
                DeformChunk newChunk = new DeformChunk();
                newChunk.chunkBounds = baseMesh.bounds;

                for (int vertIndex = 0; vertIndex < currentVertices.Count; vertIndex++)
                {
                    newChunk.vertexIndexes.Add(vertIndex);
                }

                newChunks.Add(newChunk);
            }

            deformChunks = newChunks;
        }

        List<DeformChunk> SplitChunk(DeformChunk _chunkToSplit)
        {
            List<DeformChunk> newChunks = new List<DeformChunk>();
            // Find the axis to split the chunk along.
            float largestAxis = -1f;
            int largestAxisIndex = 1;

            for (int i = 0; i < 3; i++)
            {
                if (Mathf.Abs(_chunkToSplit.chunkBounds.size[i]) > largestAxis)
                {
                    largestAxisIndex = i;
                    largestAxis = _chunkToSplit.chunkBounds.size[i];
                }
            }

            Vector3 tempSize = _chunkToSplit.chunkBounds.size;
            tempSize[largestAxisIndex] = largestAxis / 1.8f;

            Vector3 tempPos = _chunkToSplit.chunkBounds.center;
            tempPos[largestAxisIndex] = tempPos[largestAxisIndex] + largestAxis / 4;

            DeformChunk newChunk1 = new DeformChunk();
            newChunk1.chunkBounds = new Bounds(tempPos, tempSize);
            newChunks.Add(newChunk1);

            DeformChunk newChunk2 = new DeformChunk();
            newChunk2.chunkBounds = new Bounds(tempPos, tempSize);
            tempPos[largestAxisIndex] = tempPos[largestAxisIndex] - largestAxis / 2;
            newChunk2.chunkBounds.center = tempPos;
            newChunks.Add(newChunk2);

            foreach (int vertIndex in _chunkToSplit.vertexIndexes)
            {
                if (newChunk1.chunkBounds.Contains(currentVertices[vertIndex]))
                {
                    newChunks[0].vertexIndexes.Add(vertIndex);
                }
                else
                {
                    newChunks[1].vertexIndexes.Add(vertIndex);
                }
            }

            bool removeChunk1 = false;
            bool removeChunk2 = false;

            if (newChunks[0].vertexIndexes.Count > maxVertsPerChunk)
            {
                newChunks.AddRange(SplitChunk(newChunks[0]));
                removeChunk1 = true;
            }

            if (newChunks[1].vertexIndexes.Count > maxVertsPerChunk)
            {
                newChunks.AddRange(SplitChunk(newChunks[1]));
                removeChunk2 = true;
            }

            if (removeChunk1)
            {
                newChunks.Remove(newChunk1);
            }
            if (removeChunk2)
            {
                newChunks.Remove(newChunk2);
            }

            List<DeformChunk> emptyChunks = new List<DeformChunk>();
            foreach (DeformChunk chunk in newChunks)
            {
                if (chunk.vertexIndexes.Count == 0)
                {
                    emptyChunks.Add(chunk);
                }
            }

            foreach (DeformChunk chunk in emptyChunks)
            {
                newChunks.Remove(chunk);
            }

            return newChunks;
        }

        void UpdateMesh()
        {
            if (deformingMesh != null)
            {
                deformingMesh.vertices = currentVertices.ToArray();
                deformingMesh.triangles = currentTriangles.ToArray();

#if UNITY_2019_1_OR_NEWER
                deformingMesh.subMeshCount = submeshes.Length;

                for (int submeshIndex = 0; submeshIndex < submeshes.Length; submeshIndex++)
                {
                    deformingMesh.SetSubMesh(submeshIndex, submeshes[submeshIndex]);
                }
#endif
                deformingMesh.RecalculateNormals();
                deformingMesh.RecalculateBounds();
                deformingMesh.RecalculateTangents();

                BeginChunkSetUp();

#if UNITY_2019_1_OR_NEWER
                if (meshColliderToUpdate != null)
                {
                    int id = deformingMesh.GetInstanceID();
                    bool isConvex = meshColliderToUpdate.convex;

                    if (useThreading)
                    {
                        if (thread != null && thread.IsAlive)
                        {
                            thread.Abort();
                        }
                        thread = new Thread(() => UpdateCollisionMesh(id, isConvex));
                        thread.Start();
                    }
                    else
                    {
                        UpdateCollisionMesh(id, isConvex);
                    }
                }
#endif
            }
        }

        bool collisionMeshReady = false;

        private void Update()
        {

#if UNITY_2019_1_OR_NEWER
            if (collisionMeshReady && meshColliderToUpdate != null)
            {
                meshColliderToUpdate.sharedMesh = deformingMesh;
                collisionMeshReady = false;
            }
#endif
            if (deformationsRunning <= 0)
            {
                deformationsRunning = 0;

                if (tempIgnoreObjects.Count > 0)
                {
                    tempIgnoreObjects.Clear();
                }
            }
        }

#if UNITY_2019_1_OR_NEWER
        void UpdateCollisionMesh(int _meshID, bool _convex = true)
        {
            Physics.BakeMesh(_meshID, _convex);
            collisionMeshReady = true;
        }
#endif
        public void DeformMesh(Vector3 _deformPoint, Vector3 _relativeVelocity, int _tempIgnoreIndex)
        {
            _relativeVelocity = Vector3.ClampMagnitude(_relativeVelocity, maxCollisionVelocity);
            _relativeVelocity = RemapVector3(_relativeVelocity, 0, maxCollisionVelocity, 0, deformationMagnitudeScale);
            _relativeVelocity = meshToDeform.transform.InverseTransformVector(_relativeVelocity);

            _deformPoint = meshToDeform.transform.InverseTransformPoint(_deformPoint);

            List<int> inRangeChunkIndexes = new List<int>();

            Bounds impactBounds = new Bounds(_deformPoint,
                new Vector3(_relativeVelocity.magnitude, _relativeVelocity.magnitude, _relativeVelocity.magnitude) * 2.1f);

            for (int i = 0; i < deformChunks.Count; i++)
            {
                if (impactBounds.Intersects(deformChunks[i].chunkBounds))
                {
                    inRangeChunkIndexes.Add(i);
                }
            }

            if (!useChunking && inRangeChunkIndexes.Count <= 0)
            {
                inRangeChunkIndexes.Add(0);
            }

            StartCoroutine(RunDeformationOverFrames(inRangeChunkIndexes, _deformPoint, _relativeVelocity, _tempIgnoreIndex));
        }

        int deformationsRunning = 0;
        IEnumerator RunDeformationOverFrames(List<int> _inRangeChunkIndexes, Vector3 _deformPoint, Vector3 _relativeVelocity, int _tempIgnoreIndex)
        {
            deformationsRunning++;

            // Only allow each frame of deformation to take up 1/2 of a 60fps app
            float newFrameTime = Time.realtimeSinceStartup + (1f / 120f);
            bool changedMesh = false;

            foreach (int index in _inRangeChunkIndexes)
            {
                foreach (int vertindex in deformChunks[index].vertexIndexes)
                {
                    if (Time.realtimeSinceStartup >= newFrameTime)
                    {
                        newFrameTime = Time.realtimeSinceStartup + (1f / 120f);
                        yield return null;
                    }

                    if (Vector3.Distance(_deformPoint, currentVertices[vertindex]) >= _relativeVelocity.magnitude)
                    {
                        continue;
                    }

                    Vector3 changeValue = _relativeVelocity - (_relativeVelocity * Vector3.Distance(_deformPoint, currentVertices[vertindex]) / _relativeVelocity.magnitude);
                    currentVertices[vertindex] += changeValue;
                    changedMesh = true;
                }
            }

            if (changedMesh)
            {
                UpdateMesh();
            }

            tempIgnoreObjects[_tempIgnoreIndex] = null;
            deformationsRunning--;
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.relativeVelocity.magnitude < minCollisionVelocity)
            {
                return;
            }

            // also ignore collision if it is with one of our ignore objects
            foreach (GameObject ignoreObject in ignoreObjects)
            {
                if (col.gameObject == ignoreObject)
                {
                    return;
                }

                foreach (ContactPoint Contact in col.contacts)
                {
                    if (Contact.otherCollider.gameObject == ignoreObject)
                    {
                        return;
                    }
                }
            }

            foreach (GameObject ignoreObject in tempIgnoreObjects)
            {
                if (col.gameObject == ignoreObject)
                {
                    return;
                }

                foreach (ContactPoint Contact in col.contacts)
                {
                    if (Contact.otherCollider.gameObject == ignoreObject)
                    {
                        return;
                    }
                }
            }

            foreach (var contact in col.contacts)
            {
                tempIgnoreObjects.Add(col.gameObject);
                DeformMesh(contact.point, col.relativeVelocity, tempIgnoreObjects.Count - 1);
            }
        }

        #region Utility

        public static Vector3 RemapVector3(Vector3 value, float fromMagnitude1, float toMagnitude1, float fromMagnitude2, float toMagnitude2)
        {
            Vector3 from1 = value.normalized * fromMagnitude1;
            Vector3 to1 = value.normalized * toMagnitude1;
            Vector3 from2 = value.normalized * fromMagnitude2;
            Vector3 to2 = value.normalized * toMagnitude2;

            Vector3 fromAbs = value - from1;
            Vector3 fromMaxAbs = to1 - from1;

            Vector3 normal = new Vector3(fromAbs.x / fromMaxAbs.x, fromAbs.y / fromMaxAbs.y, fromAbs.z / fromMaxAbs.z);

            if (float.IsNaN(normal.x))
                normal = new Vector3(0, normal.y, normal.z);
            if (float.IsNaN(normal.y))
                normal = new Vector3(normal.x, 0, normal.z);
            if (float.IsNaN(normal.z))
                normal = new Vector3(normal.x, normal.y, 0);

            Vector3 toMaxAbs = to2 - from2;
            Vector3 toAbs = new Vector3(toMaxAbs.x * normal.x, toMaxAbs.y * normal.y, toMaxAbs.z * normal.z);
            Vector3 result = toAbs + from2;

            return result;
        }

        public static float RemapFloat(float value, float from1, float to1, float from2, float to2)
        {
            return from2 + (value - from1) * (to2 - from2) / (to1 - from1);
        }

        #endregion

        #region Gizmos

        public enum MG_GizmoType
        {
            None,
            Bounds,
            Chunk
        }
        public class MG_GizmoCube
        {
            public Vector3 m_pos;
            public Vector3 m_size;
            public MG_GizmoType m_gizType;

            public MG_GizmoCube(Vector3 _pos, Vector3 _size, MG_GizmoType _type)
            {
                m_pos = _pos;
                m_size = _size;
                m_gizType = _type;
            }
        }

        List<MG_GizmoCube> gizmoCubes = new List<MG_GizmoCube>();

        List<Vector3> gizmoSpheres = new List<Vector3>();

        int specificChunk = -1;

        void OnDrawGizmosSelected()
        {
            if (specificChunk == -1)
            {
                foreach (MG_GizmoCube giz in gizmoCubes)
                {
                    Gizmos.color = GetGizmoColour(giz.m_gizType);
                    Gizmos.DrawCube(giz.m_pos, giz.m_size);
                }
            }
            else if (specificChunk < deformChunks.Count)
            {
                Gizmos.color = GetGizmoColour(MG_GizmoType.Bounds);
                Gizmos.DrawCube(deformChunks[specificChunk].chunkBounds.center, deformChunks[specificChunk].chunkBounds.size);

                Gizmos.color = GetGizmoColour(MG_GizmoType.Chunk);

                foreach (int index in deformChunks[specificChunk].vertexIndexes)
                {
                    Gizmos.DrawSphere(currentVertices[index], 0.01f);
                }
            }

            Gizmos.color = GetGizmoColour(MG_GizmoType.Chunk);

            foreach (Vector3 pos in gizmoSpheres)
            {
                Gizmos.DrawSphere(pos, 0.01f);
            }
        }


        Color GetGizmoColour(MG_GizmoType _colour)
        {
            switch (_colour)
            {
                case MG_GizmoType.Bounds:
                    return Color.yellow - new Color(0, 0, 0, 0.5f);
                case MG_GizmoType.Chunk:
                    return Color.cyan - new Color(0, 0, 0, 0.5f);
                default:
                    return Color.white - new Color(0, 0, 0, 0.5f);
            }
        }

        #endregion
    }
}