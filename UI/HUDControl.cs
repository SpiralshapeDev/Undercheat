using HarmonyLib;
using Thor;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Undercheat;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;

namespace UnderCheat
{
    public class HUDControl
    {
        // Variables
        static Canvas canvas;
        static GameObject GO;
        static GameObject textGO;
        static RectTransform RT;
        public static bool hidden = false;
        public static bool guiActive = false;
        public static Vector3 rest_position = new Vector3(-938.0f, 272.0f, 0.0f);
        public static Vector3 rest_position_offset = new Vector3(-1250,0,0);
        static float lerp_speed = 4f;
        static int keyAmount = UnderCheatBase.KeyAmountAdd.Value;
        static int bombAmount = UnderCheatBase.BombAmountAdd.Value;
        static int goldAmount = UnderCheatBase.GoldAmountAdd.Value;
        static int thoriumAmount = UnderCheatBase.ThoriumAmountAdd.Value;
        static int netherAmount = UnderCheatBase.NetherAmountAdd.Value;
        public static GameData Data => GameData.Instance;

        public static void updateText()
        {
            if (GO == null) { return; }
            if (textGO == null) { return; }

            TextMeshProUGUI TMP = textGO.GetComponent<TextMeshProUGUI>();

            string next_page_text = $"Next page ({Undercheat.API.next_page()})";
            TMP.text = $"T: Toggle UI<br>F1: {next_page_text}<br>";
            switch (API.current_page)
            {
                case 1:

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
                                    if (extension1.GetResource(GameData.Instance.XPResource) == extension1.GetMaxResource(GameData.Instance.XPResource))
                                    {
                                        petText = "<color=red>Max Pet Level</color>";
                                    }
                                }
                            }
                        }
                    }

                    // Set Text
                    TMP.text += $"F2: {(Cheats.playerInvincible ? "<color=yellow>" : "")}Toggle Player Invulnerability</color> <br>F3: Toggle Closed doors<br>F4: Unlock All Items<br>F5: {petText}";
                    break;

                case 2:

                    // Resource Texts
                    string keyText = (keyAmount.ToString().Contains("-") ? "Removes " : "Adds ") + Mathf.Abs(keyAmount) + (Mathf.Abs(keyAmount) == 1 ? " key" : " keys"); // Ex: Adds 10 keys
                    string bombText = (bombAmount.ToString().Contains("-") ? "Removes " : "Adds ") + Mathf.Abs(bombAmount) + (Mathf.Abs(bombAmount) == 1 ? " bomb" : " bombs");
                    string goldText = (goldAmount.ToString().Contains("-") ? "Removes " : "Adds ") + Mathf.Abs(goldAmount) + " gold";
                    string thoriumText = (thoriumAmount.ToString().Contains("-") ? "Removes " : "Adds ") + Mathf.Abs(thoriumAmount) + " thorium";
                    string netherText = (netherAmount.ToString().Contains("-") ? "Removes " : "Adds ") + Mathf.Abs(netherAmount) + " nether";
                    foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
                    {
                        if ((UnityEngine.Object)player.Avatar != null)
                        {
                            InventoryExt extension2 = player.Avatar.GetExtension<InventoryExt>();

                            // Show max resource quantity
                            keyText += $" ({extension2.GetResource(GameData.Instance.KeyResource)}/{extension2.GetMaxResource(GameData.Instance.KeyResource)})</color>";
                            bombText += $" ({extension2.GetResource(GameData.Instance.BombResource)}/{extension2.GetMaxResource(GameData.Instance.BombResource)})</color>";
                            goldText += $" ({extension2.GetResource(GameData.Instance.GoldResource)}/{extension2.GetMaxResource(GameData.Instance.GoldResource)})</color>";
                            thoriumText += $" ({extension2.GetResource(GameData.Instance.ThoriumResource)}/{extension2.GetMaxResource(GameData.Instance.ThoriumResource)})</color>";
                            netherText += $" ({extension2.GetResource(GameData.Instance.NetherResource)}/{extension2.GetMaxResource(GameData.Instance.NetherResource)})</color>";

                            //* Change text color if maxed
                            // Key Resource
                            int keyOutputQuantity = extension2.GetResource(GameData.Instance.KeyResource) + keyAmount;
                            if (keyOutputQuantity < extension2.GetMinResource(GameData.Instance.KeyResource) || keyOutputQuantity > extension2.GetMaxResource(GameData.Instance.KeyResource))
                            {
                                keyText = "<color=red>" + keyText;
                            }

                            // Bomb Resource
                            int bombOutputQuantity = extension2.GetResource(GameData.Instance.BombResource) + bombAmount;
                            if (bombOutputQuantity < extension2.GetMinResource(GameData.Instance.BombResource) || bombOutputQuantity > extension2.GetMaxResource(GameData.Instance.BombResource))
                            {
                                bombText = "<color=red>" + bombText;
                            }

                            // Gold Resource
                            int goldOutputQuantity = extension2.GetResource(GameData.Instance.GoldResource) + goldAmount;
                            if (goldOutputQuantity < extension2.GetMinResource(GameData.Instance.GoldResource) || goldOutputQuantity > extension2.GetMaxResource(GameData.Instance.GoldResource))
                            {
                                goldText = "<color=red>" + goldText;
                            }

                            // Thorium Resource
                            int thoriumOutputQuantity = extension2.GetResource(GameData.Instance.ThoriumResource) + thoriumAmount;
                            if (thoriumOutputQuantity < extension2.GetMinResource(GameData.Instance.ThoriumResource) || thoriumOutputQuantity > extension2.GetMaxResource(GameData.Instance.ThoriumResource))
                            {
                                thoriumText = "<color=red>" + thoriumText;
                            }

                            // Nether Resource
                            int netherOutputQuantity = extension2.GetResource(GameData.Instance.NetherResource) + netherAmount;
                            if (netherOutputQuantity < extension2.GetMinResource(GameData.Instance.NetherResource) || netherOutputQuantity > extension2.GetMaxResource(GameData.Instance.NetherResource))
                            {
                                netherText = "<color=red>" + netherText;
                            }
                        }
                    }

                    // Set Text
                    string config_text = "<color=#c3c3c3>> Amounts can be edited in mod's configs <</color>";
                    TMP.text += $"F2: {keyText}<br>F3: {bombText}<br>F4: {goldText}<br>F5: {thoriumText}<br>F6: {netherText}<br>{config_text}";
                    break;

                case 3:
                    var item = API.GetItemDataIndex(API.discover_tab_item_index);
                    if (item is ItemData itemData)
                    {
                        var previous_item = API.GetItemDataIndex(API.discover_tab_item_index - 1);
                        var next_item = API.GetItemDataIndex(API.discover_tab_item_index + 1);

                        string previous_text = $"{(previous_item == null ? "" : $"<color=grey>{API.CapitalizedSpace(previous_item.name)}</color> <- ")}";
                        string next_text = $"{(next_item == null ? "" : $" -> <color=grey>{API.CapitalizedSpace(next_item.name)}</color>")}";

                        string selected_item = $"{previous_text}{API.CapitalizedSpace(item.name)}{next_text}<br>";
                        string previous_next = $"F2: Previous item<br>F3: Next item<br><br>";
                        string discover_relic = $"{(itemData.IsDiscovered ? "<color=red>" : "")}F4: Discover {API.CapitalizedSpace(item.name)}</color><br>";
                        string spawn_text = $"Spawn {API.CapitalizedSpace(itemData.name)} on player";
                        string random_spawn_relic = $"Spawn a random relic on player";
                        string spawn_all_relics = $"Spawn all relics on player";
                        string spawn_all_discovered_relics = $"Spawn all discovered relics on player";

                        TMP.text += $"{selected_item}{previous_next}{discover_relic}F5: {spawn_text}<br>F6: {random_spawn_relic}<br>F7: {spawn_all_relics}<br>F8: {spawn_all_discovered_relics}";
                    }
                    break;
            }
        }

        public static void Update()
        {
            if (textGO == null) { return; }
            TextMeshProUGUI TMP = textGO.GetComponent<TextMeshProUGUI>();
            if (TMP == null) { return; }

            if (Mathf.Round(RT.anchoredPosition3D.x) != Mathf.Round(rest_position.x))
            {
                RT.anchoredPosition3D = Vector3.Lerp(
                        RT.anchoredPosition3D,
                        rest_position,
                        Time.deltaTime * lerp_speed
                    );
            }

            if (hidden) 
            {
                TMP.text = ""; 
            }
        }

        static void OnSpawnsAvatar(PlayerEvent playerEvent)
        {
            guiActive = true;
            // Create GUI
            canvas = GameObject.FindObjectOfType<Canvas>();
            GO = new GameObject("HackPanel", typeof(RectTransform), typeof(CanvasGroup));
            GO.name = "MODGUID.HackUI";
            GO.transform.SetParent(canvas.transform);

            RT = GO.GetComponent<RectTransform>();
            RT.sizeDelta = new Vector2(200, 100);
            RT.anchoredPosition3D = rest_position + rest_position_offset;
            RT.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            // Create Text
            textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(GO.transform, false);

            TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.font = UnderCheatBase.fontAsset;
            tmp.fontSize = 16;
            tmp.color = Color.white;
            tmp.alignment = (TextAlignmentOptions)TextAnchor.UpperLeft;

            // Show GUI
            Debug.Log($"{UnderCheatBase.modGUID}: Showing GUI");
            API.current_page = 1;
            API.discover_tab_item_index = 0;
            updateText();

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
            RT = null;
            API.current_page = 1;
            guiActive = false;
            hidden = false;
            Debug.Log($"{UnderCheatBase.modGUID}: Hiding GUI");
        }

        [HarmonyPatch(typeof(HUD))]
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        public static void Awake()
        {
            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                Debug.Log($"{UnderCheatBase.modGUID}: Creating UI Events");
                player.RegisterEvent(PlayerEvent.EventType.SpawnsAvatar, OnSpawnsAvatar);
                player.RegisterEvent(PlayerEvent.EventType.DestroysAvatar, OnDestroysAvatar);
                Debug.Log($"{UnderCheatBase.modGUID}: Done Creating UI Events");
            }
        }
    }
}
