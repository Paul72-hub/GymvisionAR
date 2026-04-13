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

        Camera cam = Camera.main;

        Vector3 spawnPosition = cam.transform.position + cam.transform.forward * 1.2f;

        Quaternion spawnRotation = Quaternion.identity;

        _spawnedAvatar = Instantiate(avatarPrefab, spawnPosition, spawnRotation);

        // Très important : on NE le met PAS en enfant du QR code
        _spawnedAvatar.transform.SetParent(null);

        _hasSpawned = true;
    }
}