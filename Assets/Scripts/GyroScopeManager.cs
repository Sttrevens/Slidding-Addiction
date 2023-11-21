using UnityEngine;

public class GyroscopeManager : MonoBehaviour
{
    public VideoManager videoManager;

    void Start()
    {
        // Check if the device supports the gyroscope
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else
        {
            Debug.LogError("Gyroscope not supported on this device.");
        }
    }

    void Update()
    {
        if (SystemInfo.supportsGyroscope)
        {
            // Get the gyroscope attitude (rotation)
            Quaternion gyroAttitude = Input.gyro.attitude;

            // Check if the device is lying flat
            // We check if the z component of the gyroscope's rotation is close to 0
            if (Mathf.Abs(gyroAttitude.x) < 0.1f)
            {
                OnDeviceLyingFlat();
            }
            else
            {
                BrowsingMode();
            }
        }
    }

    void OnDeviceLyingFlat()
    {
        // Trigger actions when the device is lying flat
        Debug.Log("Device is lying flat on the table");
        videoManager.isOnTable = true;
    }

    void BrowsingMode()
    {
        Debug.Log("Device is in hand!");
        videoManager.isOnTable = false;
    }
}