using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private List<GameObject> prefabs;
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    private GameObject selectedPrefab;
    private GameObject selectedObject;
    private Camera arCamera;

    void Start()
    {
        arCamera = Camera.main;
        if (prefabs.Count > 0)
        {
            selectedPrefab = prefabs[0];
        }
        else
        {
            Debug.LogError("Prefab list is empty! Please assign prefabs in the inspector.");
        }

        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager is not assigned!");
        }
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                HandleTouch(touch.position, touch.phase);
            }
        }
    }

    private void HandleTouch(Vector2 position, TouchPhase phase)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Touch ignored because it's over a UI element.");
            return;
        }

        if (phase == TouchPhase.Began)
        {
            SpawnOrSelectObject(position);
        }
        else if (phase == TouchPhase.Moved && selectedObject != null)
        {
            MoveObject(position);
        }
    }

    private void SpawnOrSelectObject(Vector2 screenPosition)
    {
        if (raycastManager == null || selectedPrefab == null)
        {
            Debug.LogError("RaycastManager or selectedPrefab is null!");
            return;
        }

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;

            if (spawnedObjects.TryGetValue(selectedPrefab.name, out GameObject existingObject))
            {
                selectedObject = existingObject;
                Debug.Log("Existing object selected.");
            }
            else
            {
                GameObject newObject = Instantiate(selectedPrefab, hitPose.position, hitPose.rotation);

                OrientObjectToSurface(newObject, hitPose.rotation);

                spawnedObjects[selectedPrefab.name] = newObject;
                selectedObject = newObject;
                Debug.Log($"Spawned new object: {selectedPrefab.name}");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any plane.");
        }
    }

    private void MoveObject(Vector2 screenPosition)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;
            selectedObject.transform.position = hitPose.position;

            OrientObjectToSurface(selectedObject, hitPose.rotation);
        }
        else
        {
            Debug.Log("Raycast did not hit any plane during object movement.");
        }
    }

    private void OrientObjectToSurface(GameObject obj, Quaternion surfaceRotation)
    {
        obj.transform.rotation = surfaceRotation;
    }

    public void SelectPrefab(int prefabIndex)
    {
        if (prefabIndex < 0 || prefabIndex >= prefabs.Count)
        {
            Debug.LogError("Invalid prefab index!");
            return;
        }
        selectedPrefab = prefabs[prefabIndex];
        selectedObject = null;
    }

    public int GetSelectedPrefabIndex()
    {
        if (selectedPrefab != null)
        {
            return prefabs.IndexOf(selectedPrefab);
        }
        return -1;
    }
}
