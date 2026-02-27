using UnityEngine;
using UnityEngine.UI;
using Game.Data;
using Game.Core;

namespace Game.View
{
    public class GachaCardDisplay : MonoBehaviour
    {
        [Header("Slots & Visuals")]
        [SerializeField] private Image rarityFilter;     // Aquele filtro por cima do fundo azul
        [SerializeField] private Image artCharacterSlot; // A Amber
        [SerializeField] private Image artWeaponSlot;    // O Arco
        [SerializeField] private Image elementIcon;      // O Fogo

        [Header("Stars")]
        [SerializeField] private Transform starsContainer; // Onde nascem as estrelas
        [SerializeField] private GameObject starPrefab;    // O prefab da estrela (Image)

        [Header("Filter Colors (Config)")]
        // 3 Estrelas: Transparente (Deixa o azul original aparecer)
        [SerializeField] private Color color3Star = new Color(0, 0, 0, 0);
        // 4 Estrelas: Roxo (Mistura com o azul)
        [SerializeField] private Color color4Star = new Color(0.8f, 0f, 1f, 0.6f);
        // 5 Estrelas: Dourado/Laranja
        [SerializeField] private Color color5Star = new Color(1f, 0.6f, 0f, 0.7f);

        public void Setup(GachaItemSO item)
        {
            // 1. Configura Visual baseada no TIPO (Char vs Weapon)
            if (item.itemType == GachaType.Character)
            {
                // Mostra Personagem
                artCharacterSlot.gameObject.SetActive(true);
                artWeaponSlot.gameObject.SetActive(false);

                // Preenche
                artCharacterSlot.sprite = item.splashArt != null ? item.splashArt : item.icon;

                // Mostra Elemento
                if (item.elementIcon != null)
                {
                    elementIcon.gameObject.SetActive(true);
                    elementIcon.sprite = item.elementIcon;
                }
                else
                {
                    elementIcon.gameObject.SetActive(false);
                }
            }
            else // É Weapon
            {
                // Mostra Arma
                artCharacterSlot.gameObject.SetActive(false);
                artWeaponSlot.gameObject.SetActive(true);

                // Preenche
                artWeaponSlot.sprite = item.icon;

                // Esconde Elemento (Armas não mostram elemento na carta)
                elementIcon.gameObject.SetActive(false);
            }

            // 2. Configura a Cor do Filtro (Raridade)
            switch (item.rarity)
            {
                case GachaRarity.ThreeStar: rarityFilter.color = color3Star; break;
                case GachaRarity.FourStar: rarityFilter.color = color4Star; break;
                case GachaRarity.FiveStar: rarityFilter.color = color5Star; break;
            }

            // 3. Spawna as Estrelas
            // Limpa as antigas primeiro
            foreach (Transform child in starsContainer) Destroy(child.gameObject);

            // Enum: 0=3Star, 1=4Star, 2=5Star. Então somamos 3.
            int starCount = 3 + (int)item.rarity;

            for (int i = 0; i < starCount; i++)
            {
                Instantiate(starPrefab, starsContainer);
            }
        }
    }
}