using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class PhotoCaptureManager : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private Button takePhotoButton;
    [SerializeField] private Canvas uiCanvas;
    private string screenshotFolder;

    void Start()
    {
        screenshotFolder = Path.Combine(Application.persistentDataPath, "Screenshots");
        if (!Directory.Exists(screenshotFolder))
        {
            Directory.CreateDirectory(screenshotFolder);
        }

        if (takePhotoButton != null)
        {
            takePhotoButton.onClick.AddListener(TakePhoto);
        }
    }

    public void TakePhoto()
    {
        StartCoroutine(CapturePhoto());
    }

    private IEnumerator CapturePhoto()
    {
        if (uiCanvas != null)
        {
            uiCanvas.enabled = false;
        }

        yield return new WaitForEndOfFrame();

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        arCamera.targetTexture = renderTexture;
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        arCamera.Render();
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        arCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        if (uiCanvas != null)
        {
            uiCanvas.enabled = true;
        }

        string fileName = $"Screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string screenshotPath = Path.Combine(screenshotFolder, fileName);
        File.WriteAllBytes(screenshotPath, screenshot.EncodeToPNG());

        NativeGallery.SaveImageToGallery(screenshot, "ARAppScreenshots", fileName);

        Debug.Log($"Screenshot saved to: {screenshotPath}");
        ShowNotification("Фото сохранено в галерее!");

        Destroy(screenshot);
    }

    private void ShowNotification(string message)
    {
        Debug.Log(message);
    }
}
