using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using Newtonsoft.Json;

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

                mesh.vertices = verticies;
                mesh.triangles = triangles;
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
        [SerializeField] public SerializedType ObjectType { get; set; } = new SerializedType();

        //The mesh data for the object.
        [SerializeField] public ProBuilderMeshData ProBuilderMeshData { get; set; } = new ProBuilderMeshData();

        //Information about the object's GameObject.
        [SerializeField] public GameObjectData GameObjectData { get; set; } = new GameObjectData();
    }

    [System.Serializable]
    public class ProBuilderMeshData
    {
        [SerializeField] public Vector2[] uv { get; set; }
        [SerializeField] public Vector3[] vertices { get; set; }
        [SerializeField] public int[] triangles { get; set; }
    }

    [System.Serializable]
    public class GameObjectData
    {
        [SerializeField] public Vector3 position { get; set; }
        [SerializeField] public Quaternion rotation { get; set; }
        [SerializeField] public Vector3 localScale { get; set; }
    }

    [System.Serializable]
    public enum SerializedType
    {
        PlayerMeshObject,
        PlayerEmptyObject
    }
}