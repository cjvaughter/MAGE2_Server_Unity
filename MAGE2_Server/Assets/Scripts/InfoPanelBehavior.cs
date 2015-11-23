using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfoPanelBehavior : MonoBehaviour
{
    public Text LevelText, LevelValues;
    public Text AttribText, AttribValues;
    public Text HMText, HMValues;
    public Text KDText, KDValues;

    private string lv = "", av = "", hv = "", kv = "";
    private Player player;

    void Update()
    {
        LevelValues.text = lv;
        AttribValues.text = av;
        HMValues.text = hv;
        KDValues.text = kv;
    }

    public void UpdateInfo()
    {
        if (player != null)
        {
            lv = player.Level + "\n\n" + player.XP;
            av = player.Strength + "\n" + player.Defense + "\n" + player.Luck;   
            hv = player.Hits + "\n" + player.Misses + "\n" + player.HM.ToString("0.000");
            kv = player.Kills + "\n" + player.Deaths + "\n" + player.KD.ToString("0.000");
        }
    }

    public void SetPlayer(Player p)
    {
        player = p;
        if(player == null)
        {
            LevelText.color = Color.gray;
            lv = "";
            AttribText.color = Color.gray;
            av = "";
            HMText.color = Color.gray;
            hv = "";
            KDText.color = Color.gray;
            kv = "";
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
