using UnityEngine;

public class CameraManagement : MonoBehaviour
{
    public static CameraManagement Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraManagement>();
            }
            return _instance;
        }
    }
    private static CameraManagement _instance;

    public Camera CurrentCamera
    {
        get
        {
            return _currentCamera;
        }
    }
    private Camera _currentCamera;

    [SerializeField]
    private Camera _fixedCamera, _mobileCamera;
    private int _count = 0;

    private void Start()
    {
        ActivateFixed();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (++_count % 2 != 0)
                ActivateMobile();
            else
                ActivateFixed();
        }
    }

    public void ActivateFixed()
    {
        _currentCamera = _fixedCamera;
        _fixedCamera.gameObject.SetActive(true);
        _mobileCamera.gameObject.SetActive(false);
    }
    public void ActivateMobile()
    {
        _currentCamera = _mobileCamera;
        _fixedCamera.gameObject.SetActive(false);
        _mobileCamera.gameObject.SetActive(true);
    }
}
