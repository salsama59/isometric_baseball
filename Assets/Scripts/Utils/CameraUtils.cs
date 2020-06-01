using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtils : MonoBehaviour
{
    public static CameraController FetchCameraController()
    {
        GameObject cameraGameObject = GameObject.FindGameObjectWithTag(TagsConstants.MAIN_CAMERA);
        CameraController cameraController = cameraGameObject.GetComponent<CameraController>();
        return cameraController;
    }

    public static Camera FetchMainCamera()
    {
        return Camera.main;
    }

}
