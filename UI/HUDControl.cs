using HarmonyLib;
using Thor;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace UnderCheat
{
    public class HUDControl
    {
        // Variables
        static Canvas canvas;
        static GameObject GO;
        static TextMeshProUGUI TMP;
        static RectTransform RT;
        public static bool page1 = true;
        public static bool hidden = false;
        public static bool guiActive = false;
        static int keyAmount = UnderCheatBase.KeyAmountAdd.Value;
        static int bombAmount = UnderCheatBase.BombAmountAdd.Value;
        static int goldAmount = UnderCheatBase.GoldAmountAdd.Value;
        static int thoriumAmount = UnderCheatBase.ThoriumAmountAdd.Value;
        static int netherAmount = UnderCheatBase.NetherAmountAdd.Value;

        public static void Update()
        {
            if (GO && TMP)
            {
                switch (page1)
                {
                    case true:

                        // Pet Text
                        string petText = "Max Pet Level"; 
                        foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
                        {
                            foreach (PetOwnerExt.PetSlot petSlot in player.Avatar.GetExtension<PetOwnerExt>().PetSlots)
                            {
                                if (!UnityEngine.Object.Equals((UnityEngine.Object)petSlot.pet, (UnityEngine.Object)null))
                                {
                                    InventoryExt extension1 = petSlot.pet.GetExtension<InventoryExt>();
                                    if (!UnityEngine.Object.Equals((UnityEngine.Object)extension1, (UnityEngine.Object)null))
                                    {
                                        if (extension1.GetResource(GameData.Instance.XPResource) == extension1.GetMaxResource(GameData.Instance.XPResource)) { petText = "<color=red>Max Pet Level</color>"; }
                                    }
                                }
                            }
                        }

                        // Player Invulnerability Text
                        string invulnerabilityText = "Toggle Player Invulnerability";
                        if (Cheats.playerInvincible) { invulnerabilityText = "<color=yellow>Toggle Player Invulnerability</color>"; }
                        // Set Text
                        ((TMP_Text)TMP).text = "T: Toggle UI<br>F1: Switch To Page 2<br>F2: " + invulnerabilityText + " <br>F3: Toggle Closed doors<br>F4: Unlock All Items<br>F5: " + petText;
                        break;

                    case false:

                        // Resource Texts
                        string keyText = (keyAmount.ToString().Contains("-") ? "Removes " : "Adds ") + keyAmount + (keyAmount == 1 ? " key" : " keys"); // Ex: Adds 10 keys
                        string bombText = (bombAmount.ToString().Contains("-") ? "Removes " : "Adds ") + bombAmount + (bombAmount == 1 ? " bomb" : " bombs");
                        string goldText = (goldAmount.ToString().Contains("-") ? "Removes " : "Adds ") + goldAmount + " gold";
                        string thoriumText = (thoriumAmount.ToString().Contains("-") ? "Removes " : "Adds ") + thoriumAmount + " thorium";
                        string netherText = (netherAmount.ToString().Contains("-") ? "Removes " : "Adds ") + netherAmount + " nether";
                        foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
                        {
                            if ((UnityEngine.Object)player.Avatar != null)
                            {
                                InventoryExt extension2 = player.Avatar.GetExtension<InventoryExt>();
                                
                                // Show max resource quantity
                                keyText += " (" + extension2.GetResource(GameData.Instance.KeyResource) + "/" + extension2.GetMaxResource(GameData.Instance.KeyResource) + ")";
                                bombText += " (" + extension2.GetResource(GameData.Instance.BombResource) + "/" + extension2.GetMaxResource(GameData.Instance.BombResource) + ")";
                                goldText += " (" + extension2.GetResource(GameData.Instance.GoldResource) + "/" + extension2.GetMaxResource(GameData.Instance.GoldResource) + ")";
                                thoriumText += " (" + extension2.GetResource(GameData.Instance.ThoriumResource) + "/" + extension2.GetMaxResource(GameData.Instance.ThoriumResource) + ")";
                                netherText += " (" + extension2.GetResource(GameData.Instance.NetherResource) + "/" + extension2.GetMaxResource(GameData.Instance.NetherResource) + ")";
                                
                                //* Change text color if maxed
                                // Key Resource
                                if (keyAmount.ToString().Contains("-"))
                                {
                                    if (extension2.GetResource(GameData.Instance.KeyResource) + keyAmount < extension2.GetMinResource(GameData.Instance.KeyResource))
                                    {
                                        keyText = "<color=red>" + keyText + "</color>";
                                    }

                                }
                                else
                                {
                                    if (extension2.GetResource(GameData.Instance.KeyResource) + keyAmount > extension2.GetMaxResource(GameData.Instance.KeyResource))
                                    {
                                        keyText = "<color=red>" + keyText + "</color>";
                                    }
                                } 
                                // Bomb Resource
                                if (bombAmount.ToString().Contains("-"))
                                {
                                    if (extension2.GetResource(GameData.Instance.BombResource) + bombAmount < extension2.GetMinResource(GameData.Instance.BombResource))
                                    {
                                        bombText = "<color=red>" + bombText + "</color>";
                                    }

                                }
                                else
                                {
                                    if (extension2.GetResource(GameData.Instance.BombResource) + bombAmount > extension2.GetMaxResource(GameData.Instance.BombResource))
                                    {
                                        bombText = "<color=red>" + bombText + "</color>";
                                    }
                                }
                                
                                // Gold Resource
                                if (goldAmount.ToString().Contains("-"))
                                {
                                    if (extension2.GetResource(GameData.Instance.GoldResource) + goldAmount < extension2.GetMinResource(GameData.Instance.GoldResource))
                                    {
                                        goldText = "<color=red>" + goldText + "</color>";
                                    }

                                }
                                else
                                {
                                    if (extension2.GetResource(GameData.Instance.GoldResource) + goldAmount > extension2.GetMaxResource(GameData.Instance.GoldResource))
                                    {
                                        goldText = "<color=red>" + goldText + "</color>";
                                    }
                                }
                                
                                // Thorium Resource
                                if (thoriumAmount.ToString().Contains("-"))
                                {
                                    if (extension2.GetResource(GameData.Instance.ThoriumResource) + thoriumAmount < extension2.GetMinResource(GameData.Instance.ThoriumResource))
                                    {
                                        thoriumText = "<color=red>" + thoriumText + "</color>";
                                    }

                                }
                                else
                                {
                                    if (extension2.GetResource(GameData.Instance.ThoriumResource) + thoriumAmount > extension2.GetMaxResource(GameData.Instance.ThoriumResource))
                                    {
                                        thoriumText = "<color=red>" + thoriumText + "</color>";
                                    }
                                }
                                
                                // Nether Resource
                                if (netherAmount.ToString().Contains("-"))
                                {
                                    if (extension2.GetResource(GameData.Instance.NetherResource) + netherAmount < extension2.GetMinResource(GameData.Instance.NetherResource))
                                    {
                                        netherText = "<color=red>" + netherText + "</color>";
                                    }

                                }
                                else
                                {
                                    if (extension2.GetResource(GameData.Instance.NetherResource) + netherAmount > extension2.GetMaxResource(GameData.Instance.NetherResource))
                                    {
                                        netherText = "<color=red>" + netherText + "</color>";
                                    }
                                }
                            }
                        }
                        
                        // Set Text
                        ((TMP_Text)TMP).text = "T: Toggle UI<br>F1: Switch To Page 1<br>F2: " + keyText + "<br>F3: " + bombText + "<br>F4: " + goldText + "<br>F5: " + thoriumText + "<br>F6: " + netherText;
                        break;

                }
            }
            // Hide Text
            if (hidden) { ((TMP_Text)TMP).text = string.Empty; }
        }

        private static string needS(int i, string str)
        {
            return i == 1 ? "" : "s";
        }

        static void OnSpawnsAvatar(PlayerEvent playerEvent)
        {
            guiActive = true;
            // Create GUI
            canvas = GameObject.FindObjectOfType<Canvas>();
            GO = new GameObject();
            GO.name = UnderCheatBase.modGUID + ".HackUI";

            GO.transform.SetParent(canvas.transform);
            GO.AddComponent<RectTransform>();
            TMP = GO.AddComponent<TextMeshProUGUI>();
            RT = ((TMP_Text)TMP).rectTransform;

            RT.anchoredPosition3D = new Vector3(-938.0f, 272.0f, 0.0f);
            RT.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            ((Transform)RT).SetParent(TMP.transform);
            ((TMP_Text)TMP).font = TMP_FontAsset.CreateFontAsset(Font.CreateDynamicFontFromOSFont(Font.GetPathsToOSFonts()[0].ToString(), 16));
            ((TMP_Text)TMP).fontSize = 16f;

            ((TMP_Text)TMP).text = "test";
            ((Graphic)TMP).color = Color.white;
            ((TMP_Text)TMP).overflowMode = (TextOverflowModes)0;
            ((Behaviour)TMP).enabled = true;

            // Show GUI
            Debug.Log(UnderCheatBase.modGUID + ": Showing GUI");
            TMP.alpha = 1.0f;

            // Resource Add Amount Variables
            keyAmount = UnderCheatBase.KeyAmountAdd.Value;
            bombAmount = UnderCheatBase.BombAmountAdd.Value;
            goldAmount = UnderCheatBase.GoldAmountAdd.Value;
            thoriumAmount = UnderCheatBase.ThoriumAmountAdd.Value;
            netherAmount = UnderCheatBase.NetherAmountAdd.Value;
        }

        static void OnDestroysAvatar(PlayerEvent playerEvent)
        {
            // Delete GUI
            GameObject.Destroy(GO);
            GO = null;
            TMP = null;
            RT = null;
            page1 = true;
            guiActive = false;
            Debug.Log(UnderCheatBase.modGUID + ": Hiding GUI");
        }

        [HarmonyPatch(typeof(HUD))]
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        public static void Awake()
        {
            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                Debug.Log(UnderCheatBase.modGUID + ": Creating UI Events");
                player.RegisterEvent(PlayerEvent.EventType.SpawnsAvatar, OnSpawnsAvatar);
                player.RegisterEvent(PlayerEvent.EventType.DestroysAvatar, OnDestroysAvatar);
                Debug.Log(UnderCheatBase.modGUID + ": Done Creating UI Events");
            }
        }
    }
}
