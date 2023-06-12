using UnityEngine.UI;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public RawImage CompassImageLeft;
    public RawImage CompassImageRight;
    public Transform Player;
    public TMPro.TextMeshProUGUI CompassDirectionText;

    public void Update()
    {
        int displayangle;

        Vector3 forward = Player.transform.forward;
        forward.y = 0;

        float headingAngle = 0f;
        if(forward != Vector3.zero)
           headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;

        headingAngle = 5 * (Mathf.RoundToInt(headingAngle / 5.0f));
        displayangle = Mathf.RoundToInt(headingAngle);

        if(forward != Vector3.zero)
        {
            CompassImageLeft.uvRect = new Rect(Quaternion.LookRotation(forward).eulerAngles.y / 360, 0, 1, 1);
            CompassImageRight.uvRect = new Rect(Quaternion.LookRotation(forward).eulerAngles.y / 360, 0, 1, 1);
        }

        switch (displayangle)
        {
            case 0:
                CompassDirectionText.text = "N";
                break;
            case 360:
                CompassDirectionText.text = "N";
                break;
            case 45:
                CompassDirectionText.text = "NE";
                break;
            case 90:
                CompassDirectionText.text = "E";
                break;
            case 130:
                CompassDirectionText.text = "SE";
                break;
            case 180:
                CompassDirectionText.text = "S";
                break;
            case 225:
                CompassDirectionText.text = "SW";
                break;
            case 270:
                CompassDirectionText.text = "W";
                break;
            default:
                CompassDirectionText.text = headingAngle.ToString();
                break;
        }
    }
}