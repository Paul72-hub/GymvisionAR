using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class ImageTrackingObjectManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject avatarPrefab; // Ton avatar 3D (le sportif)
    
    private ARTrackedImageManager _trackedImageManager;
    private GameObject _spawnedAvatar;

    void Awake()
    {
        // On récupère automatiquement le composant de tracking
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        // On s'abonne à l'événement de détection d'image
        _trackedImageManager.trackedImagesChanged += OnChanged;
    }

    void OnDisable()
    {
        // On se désabonne pour éviter les erreurs quand on quitte l'app
        _trackedImageManager.trackedImagesChanged -= OnChanged;
    }

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 1. Quand une image est détectée pour la première fois
        foreach (var trackedImage in eventArgs.added)
        {
            SpawnAvatar(trackedImage);
        }

        // 2. Quand l'image bouge (suivi en temps réel)
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateAvatarPosition(trackedImage);
        }
    }

    void SpawnAvatar(ARTrackedImage trackedImage)
    {
        // On crée l'avatar à la position du QR Code
        _spawnedAvatar = Instantiate(avatarPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
        // On le met enfant de l'image pour qu'il la suive naturellement
        _spawnedAvatar.transform.parent = trackedImage.transform;
    }

    void UpdateAvatarPosition(ARTrackedImage trackedImage)
    {
        // Si l'image est bien visible, on affiche l'avatar, sinon on le cache
        if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            _spawnedAvatar.SetActive(true);
        }
        else
        {
            _spawnedAvatar.SetActive(false);
        }
    }
}