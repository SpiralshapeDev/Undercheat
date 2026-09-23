using System.Text.RegularExpressions;
using HarmonyLib;
using Thor;
using TMPro;
using UnityEngine;

namespace UnderCheat.Helpers
{
    public static class HudManager
    {
        // Variables
        private static SimulationPlayer player => Game.Instance.Simulation.PrimaryPlayer;
        private static Canvas _canvas;
        private static GameObject _panelGameObject;
        private static RectTransform _rectPanel;
        private static TextMeshProUGUI _textPanel;
        public static bool Hidden;
        private const float LerpSpeed = 4f;
        private static int lastKeyAmount = UnderCheatBase.KeyAmountAdd.Value;
        private static int lastBombAmount = UnderCheatBase.BombAmountAdd.Value;
        private static int lastGoldAmount = UnderCheatBase.GoldAmountAdd.Value;
        private static int lastThoriumAmount = UnderCheatBase.ThoriumAmountAdd.Value;
        private static int lastNetherAmount = UnderCheatBase.NetherAmountAdd.Value;
        
        private static readonly Vector3 RestPosition = new Vector3(-938.0f, 272.0f, 0.0f);
        private static readonly Vector3 RestPositionOffset = new Vector3(-1250,0,0);

        public static void UpdateText()
        {
            if (!_panelGameObject) { return; }
            if (!_textPanel) { return; }
            if (!_rectPanel) { return; }

            string nextPageText = $"Next page ({PageManager.nextPage})";
            string configText;
            lastKeyAmount = UnderCheatBase.KeyAmountAdd.Value;
            lastBombAmount = UnderCheatBase.BombAmountAdd.Value;
            lastGoldAmount = UnderCheatBase.GoldAmountAdd.Value;
            lastThoriumAmount = UnderCheatBase.ThoriumAmountAdd.Value;
            lastNetherAmount = UnderCheatBase.NetherAmountAdd.Value;
            bool hideHints = UnderCheatBase.HideConfigHints.Value;
            _textPanel.text = $"T: Toggle UI<br>F1: {nextPageText}<br>";
            switch (PageManager.currentPage)
            {
                case 1:
                {
                    string petText = "Max Pet Level";
                    if (!player.Avatar) break;

                    bool damageBoost = player.Avatar.HasModifier("UnderCheat.meleeDamageBoost");
                    foreach (PetOwnerExt.PetSlot petSlot in player.Avatar.GetExtension<PetOwnerExt>().PetSlots)
                    {
                        if (!petSlot.pet) continue;

                        InventoryExt extension1 = petSlot.pet.GetExtension<InventoryExt>();
                        if (!extension1) continue;

                        if (extension1.GetResource(GameData.Instance.XPResource) != extension1.GetMaxResource(GameData.Instance.XPResource)) continue;
                        
                        petText = $"<color=red>{petText}</color>";
                    }

                    string damageReducer = $"F2: {(CheatManager.playerImmunity ? "<color=yellow>" : "")}Toggle Damage Reducer ({UnderCheatBase.DamageReduceHackPercentage.Value}%)</color>";
                    float damageBoostAmount = UnderCheatBase.DamageBoostAmount.Value;
                    string damageBoostText = $"F3: {(damageBoost ? "<color=yellow>" : "")}Toggle Attack Damage Booster ({(damageBoostAmount < 0 ? $"{damageBoostAmount}" : $"+{damageBoostAmount}")} DMG)</color>";
                    configText = hideHints ? "" : "<br><color=#c3c3c3>> Damage reducer amount can be edited in mod's config <<br>> Hints can also be disabled <</color>";
                    string openConfigHint = hideHints ? "" : "<br><color=#c3c3c3>> Due to sandboxing, config can't be opened when running proton <</color>";
                    string openConfigText = FileManager.isProton ? $"<color=red><s>Open Config File</s></color>{openConfigHint}" : "Open Config File";
                    _textPanel.text += $"{damageReducer}<br>{damageBoostText}{configText}<br>F4: Toggle Closed doors<br>F5: Unlock All Items<br>F6: {petText}<br>F7: Refresh Config<br>F8: {openConfigText}";
                    break;
                }

                case 2:
                    // Resource Texts
                    string keyText = (lastKeyAmount < 0 ? "Removes " : "Adds ") + Mathf.Abs(lastKeyAmount) + (Mathf.Abs(lastKeyAmount) == 1 ? " key" : " keys");
                    string bombText = (lastBombAmount < 0 ? "Removes " : "Adds ") + Mathf.Abs(lastBombAmount) + (Mathf.Abs(lastBombAmount) == 1 ? " bomb" : " bombs");
                    string goldText = (lastGoldAmount < 0? "Removes " : "Adds ") + Mathf.Abs(lastGoldAmount) + " gold";
                    string thoriumText = (lastThoriumAmount < 0 ? "Removes " : "Adds ") + Mathf.Abs(lastThoriumAmount) + " thorium";
                    string netherText = (lastNetherAmount < 0 ? "Removes " : "Adds ") + Mathf.Abs(lastNetherAmount) + " nether";

                    if (!player.Avatar) break;
                    InventoryExt extension2 = player.Avatar.GetExtension<InventoryExt>();

                    // Show max resource quantity
                    keyText += $" ({extension2.GetResource(GameData.Instance.KeyResource)}/{extension2.GetMaxResource(GameData.Instance.KeyResource)})</color>";
                    bombText += $" ({extension2.GetResource(GameData.Instance.BombResource)}/{extension2.GetMaxResource(GameData.Instance.BombResource)})</color>";
                    goldText += $" ({extension2.GetResource(GameData.Instance.GoldResource)}/{extension2.GetMaxResource(GameData.Instance.GoldResource)})</color>";
                    thoriumText += $" ({extension2.GetResource(GameData.Instance.ThoriumResource)}/{extension2.GetMaxResource(GameData.Instance.ThoriumResource)})</color>";
                    netherText += $" ({extension2.GetResource(GameData.Instance.NetherResource)}/{extension2.GetMaxResource(GameData.Instance.NetherResource)})</color>";

                    //* Change text color if maxed
                    bool IsMaxed(ResourceData resource, int currentAmount)
                    {
                        int futureQuantity = (extension2.GetResource(resource) + 1) * (currentAmount > 0 ? 1 : -1);
                        return futureQuantity < extension2.GetMinResource(resource) || futureQuantity > extension2.GetMaxResource(resource);
                    }
                    keyText = IsMaxed(GameData.Instance.KeyResource,lastKeyAmount) ? $"<color=red>{keyText}</color>" : keyText;
                    bombText = IsMaxed(GameData.Instance.BombResource,lastBombAmount) ? $"<color=red>{bombText}</color>" : bombText;
                    goldText = IsMaxed(GameData.Instance.GoldResource,lastGoldAmount) ? $"<color=red>{goldText}</color>" : goldText;
                    thoriumText = IsMaxed(GameData.Instance.ThoriumResource,lastThoriumAmount) ? $"<color=red>{thoriumText}</color>" : thoriumText;
                    netherText = IsMaxed(GameData.Instance.NetherResource,lastNetherAmount) ? $"<color=red>{netherText}</color>" : netherText;

                    // Set Text
                    configText = hideHints ? "" : "<br><color=#c3c3c3>> Amounts can be edited in mod's config <<br>> Hints can also be disabled <</color>";
                    _textPanel.text += $"F2: {keyText}<br>F3: {bombText}<br>F4: {goldText}<br>F5: {thoriumText}<br>F6: {netherText}{configText}";
                    break;

                case 3:
                    var item = CheatManager.GetItemDataIndex(PageManager.discoverPageItemIndex);
                    if (!(item is ItemData itemData)) { break; }
                    
                    var previousItem = CheatManager.GetItemDataIndex(PageManager.discoverPageItemIndex - 1);
                    var nextItem = CheatManager.GetItemDataIndex(PageManager.discoverPageItemIndex + 1);
                    
                    if (!previousItem) { break; }
                    if (!nextItem) { break; }
                    
                    string CapitalizeSpace(string text)
                    {
                        bool firstMatch = true;

                        string result = Regex.Replace(text, @"[A-Z]", match =>
                        {
                            if (firstMatch)
                            {
                                firstMatch = false;
                                return $"{match.Value}";
                            }
                            else
                            {
                                return $" {match.Value}";
                            }
                        });
                        return result;
                    }

                    string previousText = $"<color=grey>{CapitalizeSpace(previousItem.name)}</color> <-";
                    string nextText = $" -> <color=grey>{CapitalizeSpace(nextItem.name)}</color>";

                    string selectedItem = $"{previousText}{CapitalizeSpace(item.name)}{nextText}<br>";
                    string previousNext = $"F2: Previous item<br>F3: Next item<br><br>";
                    string discoverRelic = $"{(itemData.IsDiscovered ? "<color=red>" : "")}F4: Discover {CapitalizeSpace(item.name)}</color><br>";
                    string spawnText = $"Spawn {CapitalizeSpace(itemData.name)} on player";
                    string randomSpawnRelic = $"Spawn a random relic on player";
                    string spawnAllRelics = $"Spawn all relics on player";
                    string spawnAllDiscoveredRelics = $"Spawn all unlocked relics on player";

                    _textPanel.text += $"{selectedItem}{previousNext}{discoverRelic}F5: {spawnText}<br>F6: {randomSpawnRelic}<br>F7: {spawnAllRelics}<br>F8: {spawnAllDiscoveredRelics}";

                    break;

                case 4:
                    _textPanel.text += $"F2: Add random Minor curse<br>F3: Remove random Minor curse<br>F4: Add random Major curse<br>F5: Remove random Major curse";
                    break;
            }
        }
        
        [HarmonyPatch(typeof(Simulation))]
        [HarmonyPatch("Process")]
        [HarmonyPostfix]
        public static void Update()
        {
            if (!player.Avatar) return; // GUI shouldn't have appeared until player avatar spawn.

            if (_panelGameObject == null || _textPanel == null || _rectPanel == null)
                return;

            if (!Mathf.Approximately(Mathf.Round(_rectPanel.anchoredPosition3D.x), Mathf.Round(RestPosition.x)))
            {
                _rectPanel.anchoredPosition3D = Vector3.Lerp(
                        _rectPanel.anchoredPosition3D,
                        RestPosition,
                        Time.deltaTime * LerpSpeed
                    );
            }
            if (Hidden)
            {
                _textPanel.text = "";
                return;
            }
            
            InventoryExt inventoryExt = player.Avatar.GetExtension<InventoryExt>();
            if (inventoryExt == null) return;

            bool keyMismatched = inventoryExt.GetResource(GameData.Instance.KeyResource) != lastKeyAmount;
            bool bombMismatched = inventoryExt.GetResource(GameData.Instance.BombResource) != lastBombAmount;
            bool goldMismatched = inventoryExt.GetResource(GameData.Instance.GoldResource) != lastGoldAmount;
            bool thoriumMismatched = inventoryExt.GetResource(GameData.Instance.ThoriumResource) != lastThoriumAmount;
            bool netherMismatched = inventoryExt.GetResource(GameData.Instance.NetherResource) != lastNetherAmount;
            bool amountMismatched = keyMismatched || bombMismatched || goldMismatched || thoriumMismatched || netherMismatched;

            if (amountMismatched) { UpdateText(); }
        }

        static void OnSpawnsAvatar(PlayerEvent playerEvent)
        {
            // Create GUI
            _canvas = Object.FindObjectOfType<Canvas>();
            _panelGameObject = new GameObject("UndercheatPanel", typeof(RectTransform), typeof(CanvasGroup))
            {
                name = $"{UnderCheatBase.ModGuid}.TextPanel"
            };
            _panelGameObject.transform.SetParent(_canvas.transform);

            _rectPanel = _panelGameObject.GetComponent<RectTransform>();
            _rectPanel.sizeDelta = new Vector2(200, 100);
            _rectPanel.anchoredPosition3D = RestPosition + RestPositionOffset;
            _rectPanel.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            // Create Text
            GameObject textGameObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGameObject.transform.SetParent(_panelGameObject.transform, false);

            _textPanel = textGameObject.GetComponent<TextMeshProUGUI>();
            _textPanel.text = "";
            _textPanel.font = UnderCheatBase.FontAsset;
            _textPanel.fontSize = 16;
            _textPanel.color = Color.white;
            _textPanel.alignment = (TextAlignmentOptions)TextAnchor.UpperLeft;

            // Show GUI
            Debug.Log($"{UnderCheatBase.ModGuid}: Showing GUI");
            PageManager.Reset();
            UpdateText();
        }

        private static void OnDestroysAvatar(PlayerEvent playerEvent)
        {
            // Delete GUI
            Object.Destroy(_panelGameObject);
            _panelGameObject = null;
            _rectPanel = null;
            PageManager.currentPage = 1;
            Debug.Log($"{UnderCheatBase.ModGuid}: Hiding GUI");
        }

        [HarmonyPatch(typeof(HUD))]
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Debug.Log($"{UnderCheatBase.ModGuid}: Creating UI Events");
            player.RegisterEvent(PlayerEvent.EventType.SpawnsAvatar, OnSpawnsAvatar);
            player.RegisterEvent(PlayerEvent.EventType.DestroysAvatar, OnDestroysAvatar);
            Debug.Log($"{UnderCheatBase.ModGuid}: Done Creating UI Events");
        }
    }
}
