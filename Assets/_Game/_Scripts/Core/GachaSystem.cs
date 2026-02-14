using System;

namespace Game.Core
{
    public class GachaSystem
    {
        // --- CONSTANTES (Regras do Jogo) ---
        // 5 Estrelas
        private const int PITY_5_STAR_HARD = 90;
        private const int PITY_5_STAR_SOFT_START = 74;
        private const float CHANCE_5_STAR_BASE = 0.006f; // 0.6%

        // 4 Estrelas
        private const int PITY_4_STAR_LIMIT = 10;
        private const float CHANCE_4_STAR_BASE = 0.051f; // 5.1%

        // --- ESTADO ---
        public int PityCounter5 { get; private set; } = 0;
        public int PityCounter4 { get; private set; } = 0;

        public GachaRarity LastResult { get; private set; }

        // --- NOVO: SISTEMA DE GARANTIA (50/50) ---

        public bool IsNext5StarGuaranteed { get; private set; } = false;
        public bool IsNext4StarGuaranteed { get; private set; } = false;

        // Chame isso quando o jogador PERDER o 50/50
        public void SetNext5StarGuaranteed(bool guaranteed)
        {
            IsNext5StarGuaranteed = guaranteed;
        }

        // Chame isso quando o jogador PERDER o 50/50 de 4 estrelas
        public void SetNext4StarGuaranteed(bool guaranteed)
        {
            IsNext4StarGuaranteed = guaranteed;
        }

        public GachaRarity Pull(float diceRoll)
        {
            // Incrementamos os contadores antes de checar
            PityCounter5++;
            PityCounter4++;

            // 1. Calculamos a chance ATUAL do 5 Estrelas (considerando soft pity do 74+)
            float currentChance5 = GetCurrentChance5Star(PityCounter5);

            // 2. Calculamos a chance ATUAL do 4 Estrelas
            // Se o contador chegou em 10, a chance vira 100% (ou mais, pra garantir)
            float currentChance4 = (PityCounter4 >= PITY_4_STAR_LIMIT) ? 2.0f : CHANCE_4_STAR_BASE;

            // --- A LÓGICA DO "BALDE" (Probabilidade Cumulativa) ---

            // TENTATIVA 5 ESTRELAS
            // Se o dado (ex: 0.004) for menor que a chance (0.006) -> GANHOU 5★
            if (diceRoll < currentChance5)
            {
                PityCounter5 = 0;
                PityCounter4 = 0; // No Genshin, tirar um 5★ geralmente reseta o contador de 10 também!
                LastResult = GachaRarity.FiveStar;
                return GachaRarity.FiveStar;
            }

            // TENTATIVA 4 ESTRELAS
            // Usamos "else if" porque se já ganhou o 5★, não precisa checar o 4★.
            // Note: Somamos as chances. Se o dado for 0.04 (4%), ele é maior que 0.006 (perdeu 5★)
            // mas é menor que 0.006 + 0.051 (ganhou 4★).
            if (diceRoll < (currentChance5 + currentChance4))
            {
                PityCounter4 = 0; // Reseta só o pity de 4★
                // O Pity de 5★ CONTINUA subindo (tristeza, mas é a regra)
                LastResult = GachaRarity.FourStar;
                return GachaRarity.FourStar;
            }

            // 3 ESTRELAS (Lixo)
            LastResult = GachaRarity.ThreeStar;
            return GachaRarity.ThreeStar;
        }

        private float GetCurrentChance5Star(int pity)
        {
            if (pity >= PITY_5_STAR_HARD) return 2.0f; // 100% garantido

            if (pity >= PITY_5_STAR_SOFT_START)
            {
                int movesIntoSoftPity = pity - PITY_5_STAR_SOFT_START + 1;
                return CHANCE_5_STAR_BASE + (0.06f * movesIntoSoftPity);
            }

            return CHANCE_5_STAR_BASE;
        }

        /// <summary>
        /// Decide se o jogador ganhou o personagem da capa (Rate Up).
        /// </summary>
        /// <param name="diceRoll">Valor de 0.0 a 1.0 (Injeção de dependência para testes)</param>
        /// <returns>True se ganhou o destaque, False se perdeu (padrão)</returns>
        public bool ResolveRateUp5Star(float diceRoll)
        {
            // Cenário 1: Jogador já tinha perdido antes. É Garantido.
            if (IsNext5StarGuaranteed)
            {
                IsNext5StarGuaranteed = false; // Reseta a garantia
                return true; // Ganhou o destaque
            }

            // Cenário 2: 50/50
            // Se o dado for menor que 0.5 (50%), ele ganha.
            if (diceRoll < 0.5f)
            {
                IsNext5StarGuaranteed = false; // Continua sem garantia
                return true; // Ganhou no 50/50!
            }

            // Cenário 3: Perdeu o 50/50
            IsNext5StarGuaranteed = true; // Ativa a garantia para o PRÓXIMO
            return false; // Ganhou um padrão (Loss)
        }
    }
}