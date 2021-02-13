using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.rotation = CameraManagement.Instance.CurrentCamera.transform.rotation;
    }
}
