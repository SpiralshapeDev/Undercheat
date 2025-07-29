using HarmonyLib;
using Thor;
using UnityEngine;
using Undercheat;

namespace UnderCheat
{
    [HarmonyPatch(typeof(HealthExt), nameof(HealthExt.ChangeHP))]
    public class Damage
    {
        static void Prefix(HealthExt __instance, ref HealthExt.ChangeHPArgs args)
        {
            bool is_player = false;
            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                if ((UnityEngine.Object)player.Avatar != (UnityEngine.Object)null)
                {
                    if (player.Avatar == __instance.Entity)
                    {
                        is_player = true;
                    }
                }
            }

            if (is_player)
            {
                if (Cheats.playerReducingDamage)
                {
                    float percentage_multiplier = (100 - UnderCheatBase.DamageReduceHackPercentage.Value) / 100;
                    float result = args.delta * percentage_multiplier;
                    int delta_out = (int)Mathf.Round(result);

                    Debug.Log($"{UnderCheatBase.modGUID}: Reduced player incoming damage by {args.delta - delta_out}");

                    if (args.delta < 0)
                    {
                        args.delta = delta_out;
                    }
                }

            }
        }
    }
}
