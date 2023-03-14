using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFade : MonoBehaviour
{
    public static IEnumerator FadeImage(bool fadeAway, Image img, Image img2 = null)
    {
        if (fadeAway)
        {
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                img.color = new Color(1, 1, 1, i);

                if(img2 != null)
                    img2.color = new Color(1, 1, 1, i);

                yield return null;
            }
        } else {
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                img.color = new Color(1, 1, 1, i);

                if (img2 != null)
                    img2.color = new Color(1, 1, 1, i);

                yield return null;
            }
        }
    }
}