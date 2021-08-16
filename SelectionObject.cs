namespace VigorXR.Utilities.UNSTABLE
{
    using UnityEngine;
    [RequireComponent(typeof(SerializedObject))]
    public class SelectionObject : MonoBehaviour
    {
        public SerializedObject GetSerializedObject;
        void Awake() => GetSerializedObject = this.gameObject.GetComponent<SerializedObject>();
    }
}
