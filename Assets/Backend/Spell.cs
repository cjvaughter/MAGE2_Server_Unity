using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MAGE2_Server
{
    public enum SpellType : byte
    {
        GenericDamage,
        GenericDamageTeam,
        GenericStun,
        GenericStunTeam,
        GenericHeal,
        GenericHealTeam,
        Fire,
        FireTeam,
        Water,
        WaterTeam,
        Earth,
        EarthTeam,
        Air,
        AirTeam,
        Ice,
        IceTeam,
        Rock,
        RockTeam,
        Electric,
        ElectricTeam,
        Poison,
        PoisonTeam,
        Psychic,
        PsychicTeam,
        Ghost,
        GhostTeam,
        Shadow,
        ShadowTeam,
        Light,
        LightTeam,
        //Add new spells here
        NumberOfSpells
    }


    [SuppressMessage("ReSharper", "RedundantExplicitArraySize")]
    public static class Spell
    {
        public const long SpellTimeout = TimeSpan.TicksPerSecond*2;
        public const int MaxChance = 100;
        static List<IRID> SpellQueue = new List<IRID>();
        static Random Chance = new Random();

        public static readonly int[,] DamageMatrix = new int[,]
        {
           //Nor, Fir, Wat, Ear, Air, Ice, Roc, Ele, Poi, Psy, Gho, Sha, Lig 
  /*Normal*/{100, 100, 100, 100, 100, 100,  50, 100, 100, 100,   0, 100, 100},
    /*Fire*/{100,  50,  50, 100, 100, 200,  50, 100, 100, 100, 100, 100, 100},
   /*Water*/{100, 200,  50, 200, 100, 100, 200, 100, 100, 100, 100, 100, 100},
   /*Earth*/{100, 200, 100, 100,   0, 100, 200, 200, 200, 100, 100, 100, 100},
     /*Air*/{100, 100, 100, 100, 100, 100,  50,  50, 100, 100, 100, 100, 100},
     /*Ice*/{100,  50,  50, 200, 200,  50, 100, 100, 100, 100, 100, 100, 100},
    /*Rock*/{100, 200, 100,  50, 200, 200, 100, 100, 100, 100, 100, 100, 100},
/*Electric*/{100, 100, 200,   0, 200, 100, 100,  50, 100, 100, 100, 100, 100},
  /*Poison*/{100, 100, 100,  50, 100, 100,  50, 100,  50, 100,  50, 100, 100},
 /*Psychic*/{100, 100, 100, 100, 100, 100, 100, 100, 200,  50, 100, 100, 100},
   /*Ghost*/{100, 100, 100, 100, 100, 100, 100, 100, 100, 200, 200, 100, 100},
  /*Shadow*/{100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100},
   /*Light*/{100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100},  
        };

        public static void Process(Player defender, byte[] data)
        {
            //SpellQueue.Add(irid);
        }

        public static void ClearExpired(long time)
        {
            SpellQueue.RemoveAll(irid => time - irid.Timestamp > SpellTimeout);
        }

        /// <summary>
        /// Calculates odds of success, carries out spell effect,
        /// awards XP, and increments hits or misses.
        /// </summary>
        /// <param name="dst">The receiving player</param>
        /// <param name="irid">The received packet</param>
        /// <returns>Whether the spell was successful or not.</returns>
        public static bool DetermineSuccess(Player dst, IRID irid)
        {
            if (!SpellQueue.Contains(irid)) return false;
            SpellQueue.Remove(irid);
            Player src = Players.Get(irid.ID);
            
            float srcChance = ((float)src.Strength / (src.Strength + dst.Defense)) * MaxChance;
            if (Chance.Next(MaxChance) <= src.Luck) srcChance += src.Luck;
            if (Chance.Next(MaxChance) <= dst.Luck) srcChance -= dst.Luck;
            bool success = Chance.Next(MaxChance) <= srcChance;

            if (success)
            {
                src.Hits++;
                src.Misses--;

                //int strength = src.Strength * (float)(DamageMatrix[src.Device.Type, dst.Device.Type])/100;


                return true;
            }

            dst.XP += 5; //XP for resisting
            return false;
        }
    }

    public class SpellEffect
    {
        
    }
}
