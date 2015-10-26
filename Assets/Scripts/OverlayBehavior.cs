using UnityEngine;

public enum Phrase
{
    Five,
    Four,
    Three,
    Two,
    One,
    Go,
    Time,
    Ready,
    Game,
    FreeForAll,
    TeamBattle,
    Round1,
    Round2,
    Round3,
    Round4,
    FinalRound,
}

public class OverlayBehavior : MonoBehaviour
{
    public Sprite Five, Four, Three, Two, One, Go, Time, Ready,
                  Game, Round1, Round2, Round3, Round4, FinalRound;

    public AudioClip FiveAudio, FourAudio, ThreeAudio, TwoAudio, OneAudio,
                     GoAudio, TimeAudio, ReadyAudio, GameAudio, FreeForAllAudio,
                     TeamBattleAudio, Round1Audio, Round2Audio, Round3Audio,
                     Round4Audio, FinalRoundAudio;

    public SpriteRenderer Renderer;
    public AudioSource Audio;

    private Vector3 fullSize = new Vector3(50, 50, 1);
    private bool _lerping, _fading, _longFade;

    public void Speak(Phrase phrase)
    {
        Audio.Stop();
        _fading = false;
        _longFade = false;
        switch (phrase)
        {
            case Phrase.Five:
                Renderer.sprite = Five;
                Audio.clip = FiveAudio;
                break;
            case Phrase.Four:
                Renderer.sprite = Four;
                Audio.clip = FourAudio;
                break;
            case Phrase.Three:
                Renderer.sprite = Three;
                Audio.clip = ThreeAudio;
                break;
            case Phrase.Two:
                Renderer.sprite = Two;
                Audio.clip = TwoAudio;
                break;
            case Phrase.One:
                Renderer.sprite = One;
                Audio.clip = OneAudio;
                break;
            case Phrase.Go:
                Renderer.sprite = Go;
                Audio.clip = GoAudio;
                _longFade = true;
                break;
            case Phrase.Time:
                Renderer.sprite = Time;
                Audio.clip = TimeAudio;
                _longFade = true;
                break;
            case Phrase.Ready:
                Renderer.sprite = Ready;
                Audio.clip = ReadyAudio;
                _longFade = true;
                break;
            case Phrase.Game:
                Renderer.sprite = Game;
                Audio.clip = GameAudio;
                _longFade = true;
                break;
            case Phrase.FreeForAll:
                Renderer.sprite = null;
                Audio.clip = FreeForAllAudio;
                break;
            case Phrase.TeamBattle:
                Renderer.sprite = null;
                Audio.clip = TeamBattleAudio;
                break;
            case Phrase.Round1:
                Renderer.sprite = Round1;
                Audio.clip = Round1Audio;
                _longFade = true;
                break;
            case Phrase.Round2:
                Renderer.sprite = Round2;
                Audio.clip = Round2Audio;
                _longFade = true;
                break;
            case Phrase.Round3:
                Renderer.sprite = Round3;
                Audio.clip = Round3Audio;
                _longFade = true;
                break;
            case Phrase.Round4:
                Renderer.sprite = Round4;
                Audio.clip = Round4Audio;
                _longFade = true;
                break;
            case Phrase.FinalRound:
                Renderer.sprite = FinalRound;
                Audio.clip = FinalRoundAudio;
                _longFade = true;
                break;
            default:
                Renderer.sprite = null;
                Audio.clip = null;
                break;
        }
        if (Renderer.sprite != null)
        {
            transform.localScale = new Vector3(100, 100, 1);
            Renderer.color = Color.white;
            _lerping = true;
            _fading = true;
        }
        if (Audio.clip != null)
            Audio.Play();
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
                Renderer.color = Color.Lerp(Renderer.color, Color.clear, 0.25f);
                float alpha = Renderer.color.a - Color.clear.a;
                if (alpha >= 0.99f)
                {
                    Renderer.color = Color.clear;
                    _fading = false;
                }
            }
        }
    }
}
