    using UnityEngine;
    using UnityEngine.XR.ARFoundation;
    using UnityEngine.XR.ARSubsystems;

    public class ImageTrackingObjectManager : MonoBehaviour
    {
        [Header("Configuration")]
        [Header("AR Infos")]
        public GameObject infoPrefab;

        private GameObject _spawnedInfo;
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

            if (_trackedImageManager != null)
            {
                _trackedImageManager.trackedImagesChanged += OnChanged;
            }
            else
            {
                Debug.LogError("ARTrackedImageManager manquant !");
            }
        }

        void OnDisable()
        {

            if (_trackedImageManager != null)
            {
                _trackedImageManager.trackedImagesChanged -= OnChanged;
            }
        }

        void Update()
    {
        if (_spawnedAvatar == null) return;

        // Zoom / dézoom avec 1 doigt (simple mais efficace)
        if (Input.GetMouseButton(0))
        {
            float zoomSpeed = 0.005f; // beaucoup plus doux

            // on lit le mouvement du doigt
            float delta = Input.GetAxis("Mouse Y");

            Vector3 scale = _spawnedAvatar.transform.localScale;

            scale += Vector3.one * delta * zoomSpeed;

            // limites pour éviter bug
            scale = new Vector3(
                Mathf.Clamp(scale.x, 0.3f, 2f),
                Mathf.Clamp(scale.y, 0.3f, 2f),
                Mathf.Clamp(scale.z, 0.3f, 2f)
            );

            _spawnedAvatar.transform.localScale = scale;

            Debug.Log("Scale : " + scale);
        }
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
            Debug.Log("TrySpawnAvatar appelé");

            if (_hasSpawned)
            {
                Debug.Log("Déjà spawn");
                return;
            }

            if (trackedImage.trackingState != TrackingState.Tracking)
            {
                Debug.Log("Tracking pas bon");
                return;
            }

            // Création
            _spawnedAvatar = Instantiate(avatarPrefab);
            // ===== INFO PANEL =====
    if (infoPrefab != null)
    {
        _spawnedInfo = Instantiate(infoPrefab);
        _spawnedInfo.SetActive(true);
        Debug.Log("INFO SPAWN");

        // parent = avatar
        _spawnedInfo.transform.SetParent(_spawnedAvatar.transform);

        // position au-dessus
        _spawnedInfo.transform.localPosition = new Vector3(0, 1.6f, 0);

        // orientation initiale
        _spawnedInfo.transform.localRotation = Quaternion.identity;
        Debug.Log("INFO PANEL SPAWN OK");
    }
    else
    {
        Debug.LogWarning("InfoPrefab non assigné !");
    }

            // Position
            _spawnedAvatar.transform.position =
                trackedImage.transform.position +
                trackedImage.transform.rotation * positionOffset;

            _spawnedAvatar.transform.rotation =
                trackedImage.transform.rotation * Quaternion.Euler(rotationOffset);

            Debug.Log("Avatar spawné à : " + _spawnedAvatar.transform.position);

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
                if (child.name == name)
                    return child;

                Transform result = FindDeepChild(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }
        Transform FindMeshRoot(Transform parent)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<SkinnedMeshRenderer>() != null ||
                child.GetComponent<MeshRenderer>() != null)
            {
                return child;
            }
        }
        return null;
    }
    }