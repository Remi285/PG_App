using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float fastMultiplier = 3f;
    public float lookSpeed = 2f;

    private float rotationX;
    private float rotationY;

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * lookSpeed;
            rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);

            transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= fastMultiplier;

        Vector3 dir = new Vector3(
            Input.GetAxis("Horizontal"),
            (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0),
            Input.GetAxis("Vertical")
        );

        transform.Translate(dir * speed * Time.deltaTime, Space.Self);
    }
}
