using UnityEngine;
using System.Collections;

public class OverlayBehavior : MonoBehaviour
{
    public Sprite five, four, three, two, one, go, time, ready;

    public SpriteRenderer sr;
    private Vector3 fullSize = new Vector3(50, 50, 1);
    private Color transparent = new Color(1f, 1f, 1f, 0f);
    private bool lerping = false, fading = false;

	IEnumerator Count()
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 5; i >= 0; i--)
        {
            ChangeSprite(i);
            yield return new WaitForSeconds(0.75f);
            fading = true;
            yield return new WaitForSeconds(0.25f);
        }
    }

    void ChangeSprite(int num)
    {
        fading = false;
        switch (num)
        {
            case 6:
                sr.sprite = ready;
                break;
            case 5:
                sr.sprite = five;
                break;
            case 4:
                sr.sprite = four;
                break;
            case 3:
                sr.sprite = three;
                break;
            case 2:
                sr.sprite = two;
                break;
            case 1:
                sr.sprite = one;
                break;
            case 0:
                sr.sprite = go;
                break;
            default:
                sr.sprite = null;
                break;
        }
        transform.localScale = new Vector3(100, 100, 1);
        sr.color = Color.white;
        lerping = true;
    }

    void Start()
    {
        StartCoroutine("Count");
    }

    void FixedUpdate()
    {
        if (lerping)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, fullSize, 0.10f);
            Vector3 delta = transform.localScale - fullSize;
            if (delta.x < 0.01 && delta.y < 0.01)
            {
                transform.localScale = fullSize;
                lerping = false;
            }
        }

        if(fading)
        {
            sr.color = Color.Lerp(sr.color, transparent, 0.25f);
            float alpha = sr.color.a - transparent.a;
            if (alpha >= 0.99f)
            {
                sr.color = transparent;
                fading = false;
            }
        }
    }
}
