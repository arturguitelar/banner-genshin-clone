using UnityEngine;
using Game.Data;

namespace Game.View
{
    public class GachaResultsView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cardsContainer; // O painel horizontal onde as cartas vão ficar
        [SerializeField] private GachaCardDisplay cardPrefab; // O prefab que acabamos de arrumar

        public void ShowResults(GachaItemSO[] items)
        {
            Clear();

            // Cria uma carta para cada item sorteado
            foreach (var item in items)
            {
                GachaCardDisplay newCard = Instantiate(cardPrefab, cardsContainer);
                newCard.Setup(item);
            }
        }

        public void Clear()
        {
            // Destrói todas as cartas filhas
            foreach (Transform child in cardsContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}