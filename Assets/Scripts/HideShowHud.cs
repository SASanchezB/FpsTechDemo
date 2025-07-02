using UnityEngine;

public class HideShowHud : MonoBehaviour
{
    [SerializeField] GameObject hud;
    private bool isHidden = false;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.U))
        {
            if (isHidden == false)
            {
                hud.SetActive(false);
                isHidden = true;
            }
            else
            {
                hud.SetActive(true);
                isHidden = false;
            }
        }
    }
}
