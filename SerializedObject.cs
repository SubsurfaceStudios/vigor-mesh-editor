using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace VigorXR.Utilities.UNSTABLE
{
    public class SerializedObject : MonoBehaviour
    {
        [HideInInspector] [SerializeField] Vector2[] uv;
        [HideInInspector] [SerializeField] Vector3[] verticies;
        [HideInInspector] [SerializeField] int[] triangles;

        private ObjectData GatheredObjectData = new ObjectData();
        public SerializedType ObjectSerializationType = SerializedType.PlayerEmptyObject;


        public void Serialize()
        {
            GatheredObjectData.ObjectType = ObjectSerializationType;

            GatheredObjectData.GameObjectData.position = transform.position;
            GatheredObjectData.GameObjectData.rotation = transform.rotation;
            GatheredObjectData.GameObjectData.localScale = transform.localScale;

            if (ObjectSerializationType == SerializedType.PlayerMeshObject)
            {
                var mesh = GetComponent<MeshFilter>().mesh;

                GatheredObjectData.ProBuilderMeshData.uv = mesh.uv;
                GatheredObjectData.ProBuilderMeshData.vertices = mesh.vertices;
                GatheredObjectData.ProBuilderMeshData.triangles = mesh.triangles;
            }
        }

        public void Rebuild(ObjectData data)
        {
            GatheredObjectData = data;

            transform.position = GatheredObjectData.GameObjectData.position;
            transform.rotation = GatheredObjectData.GameObjectData.rotation;
            transform.localScale = GatheredObjectData.GameObjectData.localScale;

            if (GatheredObjectData.ObjectType == SerializedType.PlayerMeshObject)
            {
                var mesh = GetComponent<MeshFilter>().mesh;

                triangles = GatheredObjectData.ProBuilderMeshData.triangles;
                verticies = GatheredObjectData.ProBuilderMeshData.vertices;
                uv = GatheredObjectData.ProBuilderMeshData.uv;

                mesh.triangles = triangles;
                mesh.vertices = verticies;
                mesh.uv = uv;

                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                this.GetComponent<ProBuilderMesh>().Refresh();

                this.GetComponent<MeshCollider>().sharedMesh = null;
                this.GetComponent<MeshCollider>().sharedMesh = mesh;
            }


        }

        public ObjectData PrepareForSerialization()
        {
            Serialize();
            return GatheredObjectData;
        }
    }

    [System.Serializable]
    public class ObjectData
    {
        //Holds ALL data for a serialized custom room object.

        //The serialization type of the object.
        public SerializedType ObjectType = new SerializedType();

        //The mesh data for the object.
        public ProBuilderMeshData ProBuilderMeshData = new ProBuilderMeshData();

        //Information about the object's GameObject.
        public GameObjectData GameObjectData = new GameObjectData();
    }

    [System.Serializable]
    public class ProBuilderMeshData
    {
        public Vector2[] uv;
        public Vector3[] vertices;
        public int[] triangles;
    }

    [System.Serializable]
    public class GameObjectData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;
    }

    [System.Serializable]
    public enum SerializedType
    {
        PlayerMeshObject,
        PlayerEmptyObject
    }
}