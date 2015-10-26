using UnityEngine;

public class OverlayBehavior : MonoBehaviour
{
    public Sprite Five, Four, Three, Two, One, Go, Time, Ready, Game;
    public AudioClip FiveAudio, FourAudio, ThreeAudio, TwoAudio, OneAudio, GoAudio, TimeAudio, ReadyAudio, GameAudio;

    public SpriteRenderer Renderer;
    public AudioSource Audio;

    private Vector3 fullSize = new Vector3(50, 50, 1);
    private Color _transparent = new Color(1f, 1f, 1f, 0f);
    private bool _lerping, _fading, _longFade;

    public void ChangeSprite(int num)
    {
        Audio.Stop();
        _fading = false;
        _longFade = false;
        switch (num)
        {
            case 8:
                Renderer.sprite = Game;
                Audio.clip = GameAudio;
                _longFade = true;
                break;
            case 7:
                Renderer.sprite = Time;
                Audio.clip = TimeAudio;
                _longFade = true;
                break;
            case 6:
                Renderer.sprite = Ready;
                Audio.clip = ReadyAudio;
                _longFade = true;
                break;
            case 5:
                Renderer.sprite = Five;
                Audio.clip = FiveAudio;
                break;
            case 4:
                Renderer.sprite = Four;
                Audio.clip = FourAudio;
                break;
            case 3:
                Renderer.sprite = Three;
                Audio.clip = ThreeAudio;
                break;
            case 2:
                Renderer.sprite = Two;
                Audio.clip = TwoAudio;
                break;
            case 1:
                Renderer.sprite = One;
                Audio.clip = OneAudio;
                break;
            case 0:
                Renderer.sprite = Go;
                Audio.clip = GoAudio;
                _longFade = true;
                break;
            default:
                Renderer.sprite = null;
                Audio.clip = null;
                break;
        }
        transform.localScale = new Vector3(100, 100, 1);
        Renderer.color = Color.white;
        Audio.Play();
        _lerping = true;
        _fading = true;
    }

    void FixedUpdate()
    {
        Vector3 delta = new Vector3();
        if (_lerping)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, fullSize, 0.10f);
            delta = transform.localScale - fullSize;
            if (delta.x < 0.01 && delta.y < 0.01)
            {
                transform.localScale = fullSize;
                _lerping = false;
            }
        }

        if(_fading)
        {
            if ((!_longFade && delta.x < 1 && delta.y < 1) || (_longFade && delta.x < 0.1 && delta.y < 0.1))
            {
                Renderer.color = Color.Lerp(Renderer.color, _transparent, 0.25f);
                float alpha = Renderer.color.a - _transparent.a;
                if (alpha >= 0.99f)
                {
                    Renderer.color = _transparent;
                    _fading = false;
                }
            }
        }
    }
}
