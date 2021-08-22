namespace VigorXR.Utilities.UNSTABLE
{
    using UnityEngine;
    using UnityEngine.ProBuilder;
    using System.Collections.Generic;
    using System.Linq;

    public class SerializedObject : MonoBehaviour
    {
        Vector2[] uv;
        Vector3[] verticies;
        int[] triangles;
        Vector3[] normals;

        public Dictionary<int, int> materialIndexes = new Dictionary<int, int>();

        private ObjectData GatheredObjectData = new ObjectData();
        public SerializedType ObjectSerializationType = SerializedType.PlayerEmptyObject;

        

        public void Serialize()
        {
            var handler = FindObjectOfType<BuildingUtilityNew>();

            GatheredObjectData.ObjectType = ObjectSerializationType;

            GatheredObjectData.GameObjectData.position = transform.position;
            GatheredObjectData.GameObjectData.rotation = transform.rotation;
            GatheredObjectData.GameObjectData.localScale = transform.localScale;

            if (ObjectSerializationType == SerializedType.PlayerMeshObject)
            {
                var mesh = GetComponent<MeshFilter>().sharedMesh;
                var renderer = GetComponent<MeshRenderer>();
                var pb = GetComponent<ProBuilderMesh>();

                GatheredObjectData.ProBuilderMeshData.uv = mesh.uv;
                GatheredObjectData.ProBuilderMeshData.vertices = mesh.vertices;
                GatheredObjectData.ProBuilderMeshData.triangles = mesh.triangles;
                GatheredObjectData.ProBuilderMeshData.normals = mesh.normals;

                GatheredObjectData.ProBuilderMeshData._SmaterialIndexes = materialIndexes;
            }
        }

        public void Rebuild(ObjectData data)
        {
            var handler = FindObjectOfType<BuildingUtilityNew>();

            GatheredObjectData = data;

            transform.position = GatheredObjectData.GameObjectData.position;
            transform.rotation = GatheredObjectData.GameObjectData.rotation;
            transform.localScale = GatheredObjectData.GameObjectData.localScale;

            if (GatheredObjectData.ObjectType == SerializedType.PlayerMeshObject)
            {
                var mesh = GetComponent<MeshFilter>();
                var renderer = GetComponent<MeshRenderer>();
                var pb = GetComponent<ProBuilderMesh>();
                mesh.sharedMesh = new Mesh();

                

                triangles = GatheredObjectData.ProBuilderMeshData.triangles;
                verticies = GatheredObjectData.ProBuilderMeshData.vertices;
                uv = GatheredObjectData.ProBuilderMeshData.uv;
                normals = GatheredObjectData.ProBuilderMeshData.normals;

                materialIndexes = GatheredObjectData.ProBuilderMeshData._SmaterialIndexes;

                mesh.sharedMesh.vertices = verticies;
                mesh.sharedMesh.triangles = triangles;
                mesh.sharedMesh.uv = uv;
                mesh.sharedMesh.normals = normals;

                
                pb.sharedVertices = new List<SharedVertex>();

                mesh.sharedMesh.RecalculateNormals();
                mesh.sharedMesh.RecalculateBounds();

                pb.positions = verticies;

                
                
                List<Face> faces = new List<Face>();
                for (int i = 0; i < triangles.Length / 3; i++)
                {
                    int[] indices =
                    {
                        triangles[(i * 3) + 0],
                        triangles[(i * 3) + 1],
                        triangles[(i * 3) + 2],
                    };
                    var face = new Face(indices);
                    faces.Add(face);
                }

                pb.faces = faces;

                foreach(var item in pb.faces)
                {
                    try
                    {
                        pb.SetMaterial(new Face[] { item }, handler.ObjectMaterials[materialIndexes[pb.faces.IndexOf(item)]]);
                    }
                    catch
                    {
                        this.SetFaceMaterial(new Face[] { item }, 0);
                    }
                }

                pb.ToMesh();
                
                this.GetComponent<ProBuilderMesh>().Refresh();

                this.GetComponent<MeshCollider>().sharedMesh = null;
                this.GetComponent<MeshCollider>().sharedMesh = mesh.sharedMesh;

                var _materials = renderer.materials.ToList();

                _materials.RemoveAt(0);

                renderer.materials = _materials.ToArray();
            }


        }

        public void SetFaceMaterial(Face[] faces, int materialID)
        {
            var handler = FindObjectOfType<BuildingUtilityNew>();
            if (ObjectSerializationType != SerializedType.PlayerMeshObject)
                return;

            var pb = this.GetComponent<ProBuilderMesh>();

            var material = handler.ObjectMaterials[materialID];

            pb.SetMaterial(faces, material);

            foreach(var item in faces)
            {
                var index = pb.faces.IndexOf(item);
                try
                {
                    materialIndexes[index] = materialID;
                }
                catch
                {
                    materialIndexes.Add(index, materialID);
                }
            }

            pb.ToMesh();
            pb.Refresh();
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
        [SerializeField] public Vector3[] normals { get; set; }

        [SerializeField] public Dictionary<int, int> _SmaterialIndexes { get; set; }
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