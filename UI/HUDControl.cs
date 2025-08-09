using HarmonyLib;
using Thor;
using TMPro;
using Undercheat;
using UnderCheat.Cheats;
using UnityEngine;

namespace UnderCheat.UI
{
    public class HUDControl
    {
        // Variables
        static Canvas _canvas;
        static GameObject GO;
        static GameObject textGO;
        static RectTransform RT;
        static TextMeshProUGUI TMP;
        public static bool Hidden = false;
        public static bool GUIActive = false;
        const float LerpSpeed = 4f;
        static int _keyAmount = UnderCheatBase.KeyAmountAdd.Value;
        static int _bombAmount = UnderCheatBase.BombAmountAdd.Value;
        static int _goldAmount = UnderCheatBase.GoldAmountAdd.Value;
        static int _thoriumAmount = UnderCheatBase.ThoriumAmountAdd.Value;
        static int _netherAmount = UnderCheatBase.NetherAmountAdd.Value;
        static float _damageReduce = UnderCheatBase.DamageReduceHackPercentage.Value;
        public static GameData Data => GameData.Instance;
        
        private static Vector3 _restPosition = new Vector3(-938.0f, 272.0f, 0.0f);
        private static Vector3 _restPositionOffset = new Vector3(-1250,0,0);

        public static void UpdateText()
        {
            if (!GO) { return; }
            if (!textGO) { return; }

            string nextPageText = $"Next page ({Undercheat.API.next_page()})";
            string configText;
            _keyAmount = UnderCheatBase.KeyAmountAdd.Value;
            _bombAmount = UnderCheatBase.BombAmountAdd.Value;
            _goldAmount = UnderCheatBase.GoldAmountAdd.Value;
            _thoriumAmount = UnderCheatBase.ThoriumAmountAdd.Value;
            _netherAmount = UnderCheatBase.NetherAmountAdd.Value;
            _damageReduce = UnderCheatBase.DamageReduceHackPercentage.Value;
            bool hideHints = UnderCheatBase.HideConfigHints.Value;
            TMP.text = $"T: Toggle UI<br>F1: {nextPageText}<br>";
            switch (API.current_page)
            {
                case 1:
                    bool damageBoost = false;
                    string petText = "Max Pet Level";
                    foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
                    {
                        if ((UnityEngine.Object)player.Avatar == null) break;

                        damageBoost = player.Avatar.HasModifier("CheatMeleeDamage");
                        foreach (PetOwnerExt.PetSlot petSlot in player.Avatar.GetExtension<PetOwnerExt>().PetSlots)
                        {
                            if (UnityEngine.Object.Equals((UnityEngine.Object)petSlot.pet, (UnityEngine.Object)null)) break;
                            
                            InventoryExt extension1 = petSlot.pet.GetExtension<InventoryExt>();
                            
                            if (UnityEngine.Object.Equals((UnityEngine.Object)extension1, (UnityEngine.Object)null)) break;
                          
                            if (extension1.GetResource(GameData.Instance.XPResource) == extension1.GetMaxResource(GameData.Instance.XPResource))
                            {
                                petText = $"<color=red>{petText}</color>";
                            }
                        }
                    }

                    string damageReducer = $"F2: {(CheatManager.playerReducingDamage ? "<color=yellow>" : "")}Toggle Damage Reducer ({_damageReduce}%)</color>";
                    float damageBoostAmount = UnderCheatBase.DamageBoostAmount.Value;
                    string damageBoostText = $"F3: {(damageBoost ? "<color=yellow>" : "")}Toggle Attack Damage Booster ({(damageBoostAmount.ToString().Contains("-") ? $"{damageBoostAmount}" : $"+{damageBoostAmount}")} DMG)</color>";
                    configText = (hideHints ? "" : "<br><color=#c3c3c3>> Damage reducer amount can be edited in mod's config <<br>> Hints can also be disabled <</color>");
                    TMP.text += $"{damageReducer}<br>{damageBoostText}{configText}<br>F4: Toggle Closed doors<br>F5: Unlock All Items<br>F6: {petText}<br>F7: Refresh Config<br>F8: Open Config File";
                    break;

                case 2:

                    // Resource Texts
                    string keyText = (_keyAmount.ToString().Contains("-") ? "Removes " : "Adds ") + Mathf.Abs(_keyAmount) + (Mathf.Abs(_keyAmount) == 1 ? " key" : " keys"); // Ex: Adds 10 keys
                    string bombText = (_bombAmount.ToString().Contains("-") ? "Removes " : "Adds ") + Mathf.Abs(_bombAmount) + (Mathf.Abs(_bombAmount) == 1 ? " bomb" : " bombs");
                    string goldText = (_goldAmount.ToString().Contains("-") ? "Removes " : "Adds ") + Mathf.Abs(_goldAmount) + " gold";
                    string thoriumText = (_thoriumAmount.ToString().Contains("-") ? "Removes " : "Adds ") + Mathf.Abs(_thoriumAmount) + " thorium";
                    string netherText = (_netherAmount.ToString().Contains("-") ? "Removes " : "Adds ") + Mathf.Abs(_netherAmount) + " nether";
                    foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
                    {
                        if ((UnityEngine.Object)player.Avatar == null) break;
                        
                        InventoryExt extension2 = player.Avatar.GetExtension<InventoryExt>();

                        // Show max resource quantity
                        keyText += $" ({extension2.GetResource(GameData.Instance.KeyResource)}/{extension2.GetMaxResource(GameData.Instance.KeyResource)})</color>";
                        bombText += $" ({extension2.GetResource(GameData.Instance.BombResource)}/{extension2.GetMaxResource(GameData.Instance.BombResource)})</color>";
                        goldText += $" ({extension2.GetResource(GameData.Instance.GoldResource)}/{extension2.GetMaxResource(GameData.Instance.GoldResource)})</color>";
                        thoriumText += $" ({extension2.GetResource(GameData.Instance.ThoriumResource)}/{extension2.GetMaxResource(GameData.Instance.ThoriumResource)})</color>";
                        netherText += $" ({extension2.GetResource(GameData.Instance.NetherResource)}/{extension2.GetMaxResource(GameData.Instance.NetherResource)})</color>";

                        //* Change text color if maxed
                        // Key Resource
                        int keyOutputQuantity = extension2.GetResource(GameData.Instance.KeyResource) + (1 * (_keyAmount > 0 ? 1 : -1));
                        if (keyOutputQuantity < extension2.GetMinResource(GameData.Instance.KeyResource) || keyOutputQuantity > extension2.GetMaxResource(GameData.Instance.KeyResource))
                        {
                            keyText = "<color=red>" + keyText;
                        }

                        // Bomb Resource
                        int bombOutputQuantity = extension2.GetResource(GameData.Instance.BombResource) + (1 * (_bombAmount > 0 ? 1 : -1));
                        if (bombOutputQuantity < extension2.GetMinResource(GameData.Instance.BombResource) || bombOutputQuantity > extension2.GetMaxResource(GameData.Instance.BombResource))
                        {
                            bombText = "<color=red>" + bombText;
                        }

                        // Gold Resource
                        int goldOutputQuantity = extension2.GetResource(GameData.Instance.GoldResource) + (1 * (_goldAmount > 0 ? 1 : -1));
                        if (goldOutputQuantity < extension2.GetMinResource(GameData.Instance.GoldResource) || goldOutputQuantity > extension2.GetMaxResource(GameData.Instance.GoldResource))
                        {
                            goldText = "<color=red>" + goldText;
                        }

                        // Thorium Resource
                        int thoriumOutputQuantity = extension2.GetResource(GameData.Instance.ThoriumResource) + (1 * (_thoriumAmount > 0 ? 1 : -1));
                        if (thoriumOutputQuantity < extension2.GetMinResource(GameData.Instance.ThoriumResource) || thoriumOutputQuantity > extension2.GetMaxResource(GameData.Instance.ThoriumResource))
                        {
                            thoriumText = "<color=red>" + thoriumText;
                        }

                        // Nether Resource
                        int netherOutputQuantity = extension2.GetResource(GameData.Instance.NetherResource) + (1 * (_netherAmount > 0 ? 1 : -1));
                        if (netherOutputQuantity < extension2.GetMinResource(GameData.Instance.NetherResource) || netherOutputQuantity > extension2.GetMaxResource(GameData.Instance.NetherResource))
                        {
                            netherText = "<color=red>" + netherText;
                        }
                    }

                    // Set Text
                    configText = (hideHints ? "" : "<br><color=#c3c3c3>> Amounts can be edited in mod's config <<br>> Hints can also be disabled <</color>");
                    TMP.text += $"F2: {keyText}<br>F3: {bombText}<br>F4: {goldText}<br>F5: {thoriumText}<br>F6: {netherText}{configText}";
                    break;

                case 3:
                    var item = API.GetItemDataIndex(API.discover_tab_item_index);
                    if (item is ItemData itemData)
                    {
                        var previousItem = API.GetItemDataIndex(API.discover_tab_item_index - 1);
                        var nextItem = API.GetItemDataIndex(API.discover_tab_item_index + 1);

                        string previousText = $"<color=grey>{API.CapitalizedSpace(previousItem.name)}</color> <-";
                        string nextText = $" -> <color=grey>{API.CapitalizedSpace(nextItem.name)}</color>";

                        string selectedItem = $"{previousText}{API.CapitalizedSpace(item.name)}{nextText}<br>";
                        string previousNext = $"F2: Previous item<br>F3: Next item<br><br>";
                        string discoverRelic = $"{(itemData.IsDiscovered ? "<color=red>" : "")}F4: Discover {API.CapitalizedSpace(item.name)}</color><br>";
                        string spawnText = $"Spawn {API.CapitalizedSpace(itemData.name)} on player";
                        string randomSpawnRelic = $"Spawn a random relic on player";
                        string spawnAllRelics = $"Spawn all relics on player";
                        string spawnAllDiscoveredRelics = $"Spawn all unlocked relics on player";

                        TMP.text += $"{selectedItem}{previousNext}{discoverRelic}F5: {spawnText}<br>F6: {randomSpawnRelic}<br>F7: {spawnAllRelics}<br>F8: {spawnAllDiscoveredRelics}";
                    }
                    break;

                case 4:
                    TMP.text += $"F2: Add random Minor curse<br>F3: Remove random Minor curse<br>F4: Add random Major curse<br>F5: Remove random Major curse";
                    break;
            }
        }

        public static void Update()
        {
            if (!textGO) { return; }
            if (!TMP) { return; }

            if (!Mathf.Approximately(Mathf.Round(RT.anchoredPosition3D.x), Mathf.Round(_restPosition.x)))
            {
                RT.anchoredPosition3D = Vector3.Lerp(
                        RT.anchoredPosition3D,
                        _restPosition,
                        Time.deltaTime * LerpSpeed
                    );
            }

            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                if (!(UnityEngine.Object)player.Avatar) break;
                InventoryExt extension2 = player.Avatar.GetExtension<InventoryExt>();

                bool keyMismatched = extension2.GetResource(GameData.Instance.KeyResource) != _keyAmount;
                bool bombMismatched = extension2.GetResource(GameData.Instance.KeyResource) != _bombAmount;
                bool goldMismatched = extension2.GetResource(GameData.Instance.KeyResource) != _goldAmount;
                bool thoriumMismatched = extension2.GetResource(GameData.Instance.KeyResource) != _thoriumAmount;
                bool netherMismatched = extension2.GetResource(GameData.Instance.KeyResource) != _netherAmount;
                
                bool amountMismatched = keyMismatched || bombMismatched || goldMismatched || thoriumMismatched || netherMismatched;

                if (amountMismatched) { UpdateText(); }
            }

            if (Hidden) { TMP.text = ""; }
        }

        static void OnSpawnsAvatar(PlayerEvent playerEvent)
        {
            GUIActive = true;
            // Create GUI
            _canvas = Object.FindObjectOfType<Canvas>();
            GO = new GameObject("HackPanel", typeof(RectTransform), typeof(CanvasGroup))
            {
                name = $"{UnderCheatBase.modGUID}.HackUI"
            };
            GO.transform.SetParent(_canvas.transform);

            RT = GO.GetComponent<RectTransform>();
            RT.sizeDelta = new Vector2(200, 100);
            RT.anchoredPosition3D = _restPosition + _restPositionOffset;
            RT.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            // Create Text
            textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(GO.transform, false);

            TMP = textGO.GetComponent<TextMeshProUGUI>();
            TMP.text = "";
            TMP.font = UnderCheatBase.fontAsset;
            TMP.fontSize = 16;
            TMP.color = Color.white;
            TMP.alignment = (TextAlignmentOptions)TextAnchor.UpperLeft;

            // Show GUI
            Debug.Log($"{UnderCheatBase.modGUID}: Showing GUI");
            API.current_page = 1;
            API.discover_tab_item_index = 0;
            UpdateText();

            // Resource Add Amount Variables
            _keyAmount = UnderCheatBase.KeyAmountAdd.Value;
            _bombAmount = UnderCheatBase.BombAmountAdd.Value;
            _goldAmount = UnderCheatBase.GoldAmountAdd.Value;
            _thoriumAmount = UnderCheatBase.ThoriumAmountAdd.Value;
            _netherAmount = UnderCheatBase.NetherAmountAdd.Value;
        }

        private static void OnDestroysAvatar(PlayerEvent playerEvent)
        {
            // Delete GUI
            Object.Destroy(GO);
            GO = null;
            RT = null;
            API.current_page = 1;
            GUIActive = false;
            Hidden = false;
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
