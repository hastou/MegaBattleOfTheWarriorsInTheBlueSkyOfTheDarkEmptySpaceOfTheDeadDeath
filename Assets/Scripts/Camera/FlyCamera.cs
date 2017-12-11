using UnityEngine;
 
public class FlyCamera : MonoBehaviour {
 
    public float movementSpeed = 1.0f;

    public bool rotateOnly = false;
 
    void Update ()
    {
        movementSpeed = Mathf.Max(movementSpeed += Mathf.Pow(2, Input.GetAxis("Mouse ScrollWheel"))*0.01f, 0.0f);
        if (!rotateOnly) transform.position += (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") + transform.up * Input.GetAxis("Depth")) * movementSpeed;
        transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), Input.GetAxis("Rotation"));
    }
}