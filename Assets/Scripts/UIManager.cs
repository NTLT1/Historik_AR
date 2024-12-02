using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel; 
    [SerializeField] private TMP_Text titleText; 
    [SerializeField] private TMP_Text infoText; 
    [SerializeField] private Button nextButton; 
    [SerializeField] private Button prevButton; 
    [SerializeField] private Button closeButton; 
    [SerializeField] private SpawnManager spawnManager; 
    [SerializeField] private TextAsset prefabInfoFile;
    [SerializeField] private GameObject[] uiElementsToHide;

    private List<PrefabData> prefabInfoList = new List<PrefabData>();
    private int currentPrefabIndex;

    void Start()
    {
        LoadPrefabInfo();
        currentPrefabIndex = 0;
        UpdateInfo();

        nextButton.onClick.AddListener(ShowNextInfo);
        prevButton.onClick.AddListener(ShowPreviousInfo);
        closeButton.onClick.AddListener(CloseInfoPanel);

        infoPanel.SetActive(false); 
    }


    public void OpenInfoPanel()
    {
        foreach (GameObject uiElement in uiElementsToHide)
        {
            uiElement.SetActive(false);
        }
        currentPrefabIndex = spawnManager.GetSelectedPrefabIndex();
        if (currentPrefabIndex == -1)
        {
            Debug.LogError("No prefab selected!");
            return;
        }

        infoPanel.SetActive(true);
        UpdateInfo();
    }

    private void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
        foreach (GameObject uiElement in uiElementsToHide)
        {
            uiElement.SetActive(true);
        }
    }

    private void ShowNextInfo()
    {
        currentPrefabIndex = (currentPrefabIndex + 1) % prefabInfoList.Count;
        UpdateInfo();
    }

    private void ShowPreviousInfo()
    {
        currentPrefabIndex = (currentPrefabIndex - 1 + prefabInfoList.Count) % prefabInfoList.Count;
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        titleText.text = prefabInfoList[currentPrefabIndex].title;
        infoText.text = prefabInfoList[currentPrefabIndex].description;
        spawnManager.SelectPrefab(currentPrefabIndex);
    }

    [System.Serializable]
    private class ListWrapper
    {
        public List<PrefabData> items; 
    }

    private void LoadPrefabInfo()
    {
        if (prefabInfoFile != null)
        {
            ListWrapper wrapper = JsonUtility.FromJson<ListWrapper>(prefabInfoFile.text);
            if (wrapper != null)
            {
                prefabInfoList = wrapper.items;
            }
            else
            {
                Debug.LogError("Failed to parse prefab info JSON!");
            }
        }
        else
        {
            Debug.LogError("Prefab info file is missing!");
        }
    }


}
