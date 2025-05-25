using CesiumForUnity;
using UnityEngine;

public class OverviewCameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 50f;
    public float fastMultiplier = 3f;

    private float currentYaw = 0f;

    public CesiumCameraController cesiumCameraController;

    void Update()
    {
        // Solo mover si Ctrl está presionado
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
        {
            cesiumCameraController.enabled = true;
            return;
        }

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            speed *= fastMultiplier;

        Vector3 move = Vector3.zero;

        cesiumCameraController.enabled = false;

        // Creamos los ejes manuales con rotación horizontal aplicada (en Y)
        float yawRad = currentYaw * Mathf.Deg2Rad;
        Vector3 forwardXZ = new Vector3(Mathf.Sin(yawRad), 0, Mathf.Cos(yawRad));
        Vector3 rightXZ   = new Vector3(forwardXZ.z, 0, -forwardXZ.x); // 90 grados a la derecha

        // Movimiento relativo a la rotación horizontal
        if (Input.GetKey(KeyCode.W)) move += forwardXZ;
        if (Input.GetKey(KeyCode.S)) move -= forwardXZ;
        if (Input.GetKey(KeyCode.D)) move += rightXZ;
        if (Input.GetKey(KeyCode.A)) move -= rightXZ;

        // Movimiento vertical directo
        if (Input.GetKey(KeyCode.Z)) move += Vector3.up;
        if (Input.GetKey(KeyCode.X)) move -= Vector3.up;

        transform.position += move * speed * Time.deltaTime;

        // Rotación horizontal (solo en Y)
        float yawInput = 0f;
        if (Input.GetKey(KeyCode.Q)) yawInput -= 1f;
        if (Input.GetKey(KeyCode.E)) yawInput += 1f;

        currentYaw += yawInput * rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(90f, currentYaw, 0f); // Siempre mirando hacia abajo
    }
}
