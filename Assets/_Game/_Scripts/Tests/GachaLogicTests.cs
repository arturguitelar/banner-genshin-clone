using NUnit.Framework;
using Game.Core;

public class GachaLogicTests
{
    // --- TESTE 1: O Teto de 90 (5 Estrelas) ---
    [Test]
    public void HardPity_Guarantees_FiveStar_At_90()
    {
        // 1. Arrange
        GachaSystem gacha = new GachaSystem();

        // Vamos simular 89 tiros de puro azar
        // Nota: O azar do 5* (1.0f) vai acabar acionando o pity de 4* no caminho,
        // mas o foco desse teste é verificar se o contador de 5* chega no 90.
        for (int i = 0; i < 89; i++)
        {
            gacha.Pull(1.0f); // 1.0f = Azar garantido para 5*
        }

        // Verificamos o estado antes do tiro final
        Assert.AreEqual(89, gacha.PityCounter5, "Deveria estar com 89 de Pity 5 Estrelas");

        // 2. Act (O Tiro 90)
        GachaRarity result = gacha.Pull(1.0f);

        // 3. Assert
        Assert.AreEqual(GachaRarity.FiveStar, result, "O tiro 90 deve ser 5 Estrelas garantido");
        Assert.AreEqual(0, gacha.PityCounter5, "O Pity de 5 Estrelas deve resetar após o ganho");
    }

    // --- TESTE 2: O Teto de 10 (4 Estrelas) ---
    [Test]
    public void HardPity_Guarantees_FourStar_At_10()
    {
        // 1. Arrange
        GachaSystem gacha = new GachaSystem();

        // Simulamos 9 tiros de lixo (3 estrelas)
        // Precisamos de um valor que não seja 5* e nem 4*
        // Chance 4* é aprox 5.1% (0.051). Se jogarmos 0.1f (10%), perdemos o 4* e o 5*.
        float badLuckRoll = 0.5f;

        for (int i = 0; i < 9; i++)
        {
            GachaRarity pullResult = gacha.Pull(badLuckRoll);

            // Só pra garantir que nosso teste não está viciado e ganhando sem querer
            Assert.AreEqual(GachaRarity.ThreeStar, pullResult, $"O tiro {i + 1} deveria ser lixo (3*)");
        }

        Assert.AreEqual(9, gacha.PityCounter4, "Deveria estar com 9 de Pity 4 Estrelas");

        // 2. Act (O Tiro 10)
        // Mesmo com dado ruim (0.5f), o sistema deve forçar a vitória
        GachaRarity result = gacha.Pull(badLuckRoll);

        // 3. Assert
        Assert.AreEqual(GachaRarity.FourStar, result, "O tiro 10 deve ser 4 Estrelas garantido");
        Assert.AreEqual(0, gacha.PityCounter4, "O Pity de 4 Estrelas deve resetar");
    }

    [Test]
    public void RateUp_Mechanic_Works_Correctly()
    {
        // 1. Arrange
        GachaSystem gacha = new GachaSystem();

        // O sistema começa "limpo" (sem garantia)
        Assert.IsFalse(gacha.IsNext5StarGuaranteed, "Deve começar sem garantia");

        // --- PASSO 1: PERDER O 50/50 ---
        // Simulamos um dado de 0.6 (60%), que é maior que 0.5. Ou seja, PERDEU.
        bool wonRateUp1 = gacha.ResolveRateUp5Star(0.6f);

        Assert.IsFalse(wonRateUp1, "Deveria ter perdido o 50/50 com dado 0.6");
        Assert.IsTrue(gacha.IsNext5StarGuaranteed, "Agora o sistema deve estar GARANTIDO");

        // --- PASSO 2: USAR O GARANTIDO ---
        // Agora, mesmo se o dado for RUIM (0.9), o sistema tem que ignorar e dar a vitória.
        bool wonRateUp2 = gacha.ResolveRateUp5Star(0.9f);

        Assert.IsTrue(wonRateUp2, "Deveria ganhar o destaque pois estava GARANTIDO");
        Assert.IsFalse(gacha.IsNext5StarGuaranteed, "A garantia deve resetar após o uso");
    }
}