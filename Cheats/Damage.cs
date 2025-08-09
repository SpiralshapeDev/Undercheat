using HarmonyLib;
using Thor;
using UnityEngine;

namespace UnderCheat.Cheats
{
    [HarmonyPatch(typeof(HealthExt), nameof(HealthExt.ChangeHP))]
    public class Damage
    {
        static void Prefix(HealthExt __instance, ref HealthExt.ChangeHPArgs args)
        {
            bool isPlayer = false;
            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                if (!(UnityEngine.Object)player.Avatar) break;
                if (player.Avatar != __instance.Entity) break;
                
                isPlayer = true;
            }

            if (isPlayer)
            {
                if (CheatManager.playerReducingDamage)
                {
                    float percentageMultiplier = (100 - UnderCheatBase.DamageReduceHackPercentage.Value) / 100;
                    float result = args.delta * percentageMultiplier;
                    int deltaOut = (int)Mathf.Round(result);

                    Debug.Log($"{UnderCheatBase.modGUID}: Reduced player incoming damage by {args.delta - deltaOut}");

                    if (args.delta < 0)
                    {
                        args.delta = deltaOut;
                    }
                }

            }
        }
    }
}
