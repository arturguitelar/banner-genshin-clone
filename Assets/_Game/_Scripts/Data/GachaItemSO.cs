using UnityEngine;
using Game.Core;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Gacha Item", menuName = "Genshin/Gacha Item")]
    public class GachaItemSO : ScriptableObject
    {
        [Header("Identity")]
        public string idName;       // ID único (ex: "char_fischl")
        public string displayName;  // Nome na tela (ex: "Fischl")

        [Header("Visuals - List View")]
        public Sprite icon;         // A imagem pequena/vertical

        [Header("Visuals - Splash View")]
        public Sprite splashArt;    // A arte full body (Ref 2)
        public Sprite elementIcon;  // O ícone do elemento (Pyro, Electro, etc)

        [Header("Data")]
        public GachaRarity rarity;  // Define a cor do brilho e a lógica de drop
    }
}