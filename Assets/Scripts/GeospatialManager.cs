using System.Collections;
using Google.XR.ARCoreExtensions;
using TMPro;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
public class GeospatialManager : MonoBehaviour
{
    [Header("Core Features")]
    [SerializeField]
    private TextMeshProUGUI geospatialStatusText;

    [SerializeField]
    private AREarthManager earthManager;

    [SerializeField] private GameObject session;

    [SerializeField]
    private ARCoreExtensions arcoreExtensions;

    private bool waitingForLocationService = false;

    private Coroutine locationServiceLauncher;


    [SerializeField] private GameObject completePanel;
    public static GeospatialManager Instance { private set; get; }
    private void Awake()
    {
        // Enable geospatial sample to target 60fps camera capture frame rate
        // on supported devices.
        // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
        if (Instance == null)
        {
            Instance = this;
        }
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        //if (!Debug.isDebugBuild || earthManager == null)
        //{
        //    return;
        //}
        if (ARSession.state != ARSessionState.SessionInitializing &&
               ARSession.state != ARSessionState.SessionTracking)
        {
            return;
        }

        // Check feature support and enable Geospatial API when it's supported.
        var featureSupport = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch (featureSupport)
        {
            case FeatureSupported.Unknown:
                break;
            case FeatureSupported.Unsupported:
                Debug.Log("The Geospatial API is not supported by this device.");
                break;
            case FeatureSupported.Supported:
                if (arcoreExtensions.ARCoreExtensionsConfig.GeospatialMode == GeospatialMode.Disabled)
                {
                    arcoreExtensions.ARCoreExtensionsConfig.GeospatialMode =
                        GeospatialMode.Enabled;
                    arcoreExtensions.ARCoreExtensionsConfig.StreetscapeGeometryMode =
                        StreetscapeGeometryMode.Enabled;
                }
                break;
        }

        var pose = earthManager.EarthState == EarthState.Enabled &&
            earthManager.EarthTrackingState == TrackingState.Tracking ?
            earthManager.CameraGeospatialPose : new GeospatialPose();
        var supported = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);

        if (geospatialStatusText != null)
        {
            geospatialStatusText.text =
                $"SessionState: {ARSession.state}\n" +
                $"LocationServiceStatus: {Input.location.status}\n" +
                $"FeatureSupported: {supported}\n" +
                $"EarthState: {earthManager.EarthState}\n" +
                $"EarthTrackingState: {earthManager.EarthTrackingState}\n" +
                $"  LAT/LNG: {pose.Latitude:F6}, {pose.Longitude:F6}\n" +
                $"  HorizontalAcc: {pose.HorizontalAccuracy:F6}\n" +
                $"  ALT: {pose.Altitude:F2}\n" +
                $"  VerticalAcc: {pose.VerticalAccuracy:F2}\n" +
                $"  EunRotation: {pose.EunRotation:F2}\n" +
                $"  OrientationYawAcc: {pose.OrientationYawAccuracy:F2}";
        }
    }
    private IEnumerator AvailabilityCheck()
    {
        if (ARSession.state == ARSessionState.None)
        {
            yield return ARSession.CheckAvailability();
        }

        // Waiting for ARSessionState.CheckingAvailability.
        yield return null;

        if (ARSession.state == ARSessionState.NeedsInstall)
        {
            yield return ARSession.Install();
        }

        // Waiting for ARSessionState.Installing.
        yield return null;
#if UNITY_ANDROID

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.Log("Requesting camera permission.");
            Permission.RequestUserPermission(Permission.Camera);
            yield return new WaitForSeconds(3.0f);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            // User has denied the request.
            Debug.LogWarning(
                "Failed to get the camera permission. VPS availability check isn't available.");
            yield break;
        }
#endif


        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogWarning(
                "Location services aren't running. VPS availability check is not available.");
            yield break;
        }
    }
    private void OnEnable()
    {
        locationServiceLauncher = StartCoroutine(StartLocationService());
        StartCoroutine(AvailabilityCheck());
    }

    private void OnDisable()
    {
        StopCoroutine(locationServiceLauncher);
        locationServiceLauncher = null;
        Debug.Log("Stopping location services.");
        Input.location.Stop();
    }

    private IEnumerator StartLocationService()
    {
        waitingForLocationService = true;

#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.Log("Requesting the fine location permission.");
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(3.0f);
        }
#endif

        if (!Input.location.isEnabledByUser)
        {
            waitingForLocationService = false;
            yield break;
        }

        Debug.Log("Starting location service.");
        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return null;
        }

        waitingForLocationService = false;
        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.Log($"Location service ended with {0} status {Input.location.status}");
            Input.location.Stop();
        }
    }

    public void OnCoinClick()
    {
        if (!completePanel.activeInHierarchy)
            completePanel.SetActive(true);
    }

    public void OnBack()
    {
        SceneManager.LoadScene(0);
    }
}