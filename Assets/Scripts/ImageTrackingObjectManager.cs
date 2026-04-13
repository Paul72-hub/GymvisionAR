using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackingObjectManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject avatarPrefab;

    private ARTrackedImageManager _trackedImageManager;
    private GameObject _spawnedAvatar;
    private bool _hasSpawned = false;

    [Header("Offsets optionnels")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    

    void Awake()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        _trackedImageManager.trackedImagesChanged += OnChanged;
    }

    void OnDisable()
    {
        _trackedImageManager.trackedImagesChanged -= OnChanged;
    }

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            TrySpawnAvatar(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            TrySpawnAvatar(trackedImage);
        }
    }

    void TrySpawnAvatar(ARTrackedImage trackedImage)
    {
        if (_hasSpawned) return;
        if (trackedImage.trackingState != TrackingState.Tracking) return;

        // On crée l'avatar
        _spawnedAvatar = Instantiate(avatarPrefab);
        
        // On le place EXACTEMENT à la position du QR code dans le monde
        _spawnedAvatar.transform.position = trackedImage.transform.position;
        _spawnedAvatar.transform.rotation = trackedImage.transform.rotation * Quaternion.Euler(rotationOffset);
        
        Debug.Log($"[AR] Avatar spawn à : {_spawnedAvatar.transform.position}. QR Code à : {trackedImage.transform.position}");

        // Configuration dynamique du BarFollowHands
        BarFollowHands barFollow = _spawnedAvatar.GetComponentInChildren<BarFollowHands>();
        if (barFollow != null)
        {
            Transform leftHand = FindDeepChild(_spawnedAvatar.transform, "Boy_LeftHand");
            Transform rightHand = FindDeepChild(_spawnedAvatar.transform, "Boy_RightHand");

            if (leftHand != null && rightHand != null)
            {
                barFollow.leftHand = leftHand;
                barFollow.rightHand = rightHand;
            }
        }

        _hasSpawned = true;
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }
}