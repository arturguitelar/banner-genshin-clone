using NUnit.Framework;
using Game.Core; // Vamos criar essa classe já já

public class WalletLogicTests
{
    [Test]
    public void Wallet_Spending_Reduces_Balance_Correctly()
    {
        // 1. Arrange: Começa com 1600 Gemas (10 tiros)
        var wallet = new WalletSystem(1600);

        // 2. Act: Tenta gastar 160
        bool transactionSuccess = wallet.TrySpend(160);

        // 3. Assert
        Assert.IsTrue(transactionSuccess, "A transação deveria ser aprovada.");
        Assert.AreEqual(1440, wallet.CurrentPrimogems, "O saldo deveria cair para 1440.");
    }

    [Test]
    public void Wallet_Blocks_Purchase_If_Insufficient_Funds()
    {
        // 1. Arrange: Pobre (só tem 150 gemas)
        var wallet = new WalletSystem(150);

        // 2. Act: Tenta gastar 160 (O custo do banner)
        bool transactionSuccess = wallet.TrySpend(160);

        // 3. Assert
        Assert.IsFalse(transactionSuccess, "A transação deveria ser recusada.");
        Assert.AreEqual(150, wallet.CurrentPrimogems, "O saldo NÃO deveria mudar.");
    }
}