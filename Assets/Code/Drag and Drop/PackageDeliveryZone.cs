using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class PackageDeliveryZone : MonoBehaviour, IDataPersistence
{
    [Description("When the player touches the trigger collider attached to this Game Object, whatever package the player is holding will be delivered.")]

    [SerializeField] private string id;
    [SerializeField] private GameObject objectToEnableOnDelivery;
    private bool isPrefab = true;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] bool collectedPackage = false;

    public UnityEvent PackageDelivered;

    private void Start()
    {
        isPrefab = false;

        if (id.Equals(string.Empty))
            Debug.LogError("GUID for this object is null. Please assign a GUID for saving data.");

        if (PackageDelivered == null)
            PackageDelivered = new UnityEvent();
    }

    public void LoadData(GameData data)
    {
        if (isPrefab) return;
        data.collectionZones.TryGetValue(id, out collectedPackage);
    }

    public void SaveData(ref GameData data)
    {
        if (isPrefab) return;

        if (data.collectionZones.ContainsKey(id))
        {
            data.collectionZones.Remove(id);
        }
        data.collectionZones.Add(id, collectedPackage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController) && !collectedPackage && playerController.HasPackage())
        {
            playerController.RemovePackage();
            collectedPackage = true;
            if (objectToEnableOnDelivery) objectToEnableOnDelivery.SetActive(true);
            PackageDelivered?.Invoke();
        }
    }
}
