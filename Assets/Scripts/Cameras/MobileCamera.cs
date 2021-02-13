using UnityEngine;

public class MobileCamera : MonoBehaviour
{
    [SerializeField]
    private float _mainSpeed = 10.0f;
    private float _camSensitiveness = 0.1f;
    private bool _mousePressed = false;
    private Vector3 _lastMousePosition;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mousePressed = true;
            _lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _mousePressed = false;
        }
        if (_mousePressed)
        {
            // mouse angle camera
            _lastMousePosition = Input.mousePosition - _lastMousePosition;
            _lastMousePosition = new Vector3(-_lastMousePosition.y * _camSensitiveness, _lastMousePosition.x * _camSensitiveness, 0);
            _lastMousePosition = new Vector3(transform.eulerAngles.x + _lastMousePosition.x, transform.eulerAngles.y + _lastMousePosition.y, 0);
            transform.eulerAngles = _lastMousePosition;
            _lastMousePosition = Input.mousePosition;
        }

        // keyboard
        Vector3 p = GetBaseInput();
        p = p * _mainSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position;
        // If player wants to move on X and Z axis only
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(p);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        else
        {
            transform.Translate(p);
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
}