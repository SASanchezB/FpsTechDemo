using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX = 100f;
    public float sensY = 100f;

    public Transform orientation;

    private float xRotation;
    private float yRotation;

    //
    private float recoilOffset;
    private float recoilSpeed = 10f;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Cargamos los valores de sensibilidad si existen
        if (PlayerPrefs.HasKey("ValorSliderxSen"))
            sensX = PlayerPrefs.GetFloat("ValorSliderxSen");

        if (PlayerPrefs.HasKey("ValorSliderySen"))
            sensY = PlayerPrefs.GetFloat("ValorSliderySen");
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        // Aplicar recoil progresivamente
        recoilOffset = Mathf.Lerp(recoilOffset, 0f, Time.deltaTime * recoilSpeed);
        xRotation -= mouseY;
        xRotation += recoilOffset;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }

    public void ApplyRecoil(float amount)
    {
        recoilOffset += amount;
    }
}
