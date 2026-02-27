using UnityEngine;
using Game.Core; // <--- Importante para ele achar o Enum GachaType

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Gacha Item", menuName = "Genshin/Gacha Item")]
    public class GachaItemSO : ScriptableObject
    {
        [Header("Identity")]
        public string idName;
        public string displayName;

        [Header("Type & Rarity")]
        public GachaType itemType; 
        public GachaRarity rarity;

        [Header("Visuals - List View")]
        public Sprite icon;         // Ícone pequeno (Armas usam este na carta)

        [Header("Visuals - Splash View")]
        public Sprite splashArt;    // Arte de corpo inteiro (Só Characters usam)
        public Sprite elementIcon;  // Fogo, Vento, etc.
    }
}