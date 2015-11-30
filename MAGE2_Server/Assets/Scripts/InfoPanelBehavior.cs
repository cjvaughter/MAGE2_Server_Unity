using UnityEngine;
using UnityEngine.UI;

public class InfoPanelBehavior : MonoBehaviour
{
    public Text LevelText, LevelValues;
    public Text AttribText, AttribValues;
    public Text HMText, HMValues;
    public Text KDText, KDValues;

    private string _lv = "", _av = "", _hv = "", _kv = "";
    private Player _player;

    void Update()
    {
        if (_player != null)
        {
            _lv = _player.Level + "\n\n" + _player.XP;
            _av = _player.Strength + "\n" + _player.Defense + "\n" + _player.Luck;
            _hv = _player.Hits + "\n" + _player.Misses + "\n" + _player.HM.ToString("0.000");
            _kv = _player.Kills + "\n" + _player.Deaths + "\n" + _player.KD.ToString("0.000");
        }

        LevelValues.text = _lv;
        AttribValues.text = _av;
        HMValues.text = _hv;
        KDValues.text = _kv;
    }

    public void SetPlayer(Player p)
    {
        _player = p;
        if(_player == null)
        {
            LevelText.color = Color.gray;
            _lv = "";
            AttribText.color = Color.gray;
            _av = "";
            HMText.color = Color.gray;
            _hv = "";
            KDText.color = Color.gray;
            _kv = "";
        }
        else
        {
            LevelText.color = Color.white;
            AttribText.color = Color.white;
            HMText.color = Color.white;
            KDText.color = Color.white;
        }
    }
}
