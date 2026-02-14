using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "StandardPool", menuName = "Genshin/Standard Pool")]
    public class GachaPoolSO : ScriptableObject
    {
        [Header("Standard 5-Stars (Loss)")]
        public List<GachaItemSO> characters5Star; // Diluc, Jean, etc.
        public List<GachaItemSO> weapons5Star;    // Skyward Harp, etc.

        [Header("Standard 4-Stars")]
        public List<GachaItemSO> characters4Star; // Amber, Kaeya...
        public List<GachaItemSO> weapons4Star;    // The Bell...

        [Header("Trash (3-Star)")]
        public List<GachaItemSO> weapons3Star;

        // Métodos auxiliares para facilitar a vida
        public GachaItemSO GetRandomStandard5Star()
        {
            // Nota: No Banner de Personagem, geralmente só cai Personagem 5* no Loss (verifique a regra atual)
            // Mas vamos simplificar pegando da lista de personagens por enquanto
            if (characters5Star.Count == 0) return null;
            return characters5Star[Random.Range(0, characters5Star.Count)];
        }

        public GachaItemSO GetRandomStandard4Star()
        {
            // Aqui misturamos armas e personagens
            var combined = new List<GachaItemSO>();
            combined.AddRange(characters4Star);
            combined.AddRange(weapons4Star);

            if (combined.Count == 0) return null;
            return combined[Random.Range(0, combined.Count)];
        }

        public GachaItemSO GetRandom3Star()
        {
            if (weapons3Star.Count == 0) return null;
            return weapons3Star[Random.Range(0, weapons3Star.Count)];
        }
    }
}