using UnityEngine;

public class MobileCamera : MonoBehaviour
{
    [SerializeField]
    private float _mainSpeed = 10.0f; //regular speed
    [SerializeField]
    private float _shiftAdd = 25.0f; //multiplied by how long shift is held.  Basically running
    [SerializeField]
    private float _maxShift = 100.0f; //Maximum speed when holdin gshift
    [SerializeField]
    private float _camSens = 0.1f; //How sensitive it with mouse
    
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;

    void Update()
    {
        // mouse angle camera
        lastMouse = Input.mousePosition - lastMouse;
        lastMouse = new Vector3(-lastMouse.y * _camSens, lastMouse.x * _camSens, 0);
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
        transform.eulerAngles = lastMouse;
        lastMouse = Input.mousePosition;

        // keyboard
        Vector3 p = GetBaseInput();
        p = p * _mainSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position;
        //if (Input.GetKey(KeyCode.Space))
        //{ //If player wants to move on X and Z axis only
        //    transform.Translate(p);
        //    newPosition.x = transform.position.x;
        //    newPosition.z = transform.position.z;
        //    transform.position = newPosition;
        //}
        //else
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