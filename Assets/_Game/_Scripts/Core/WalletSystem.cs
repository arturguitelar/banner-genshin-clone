using UnityEngine;

public class WalletSystem
{
    public int CurrentPrimogems { get; private set; }

    // Construtor para definir saldo inicial (útil pra testes e save game)
    public WalletSystem(int initialAmount)
    {
        CurrentPrimogems = initialAmount;
    }

    public void AddGems(int amount)
    {
        CurrentPrimogems += amount;
    }

    /// <summary>
    /// Tenta realizar uma compra. Retorna true se teve saldo.
    /// </summary>
    public bool TrySpend(int cost)
    {
        if (CurrentPrimogems >= cost)
        {
            CurrentPrimogems -= cost;
            return true; // Compra aprovada
        }

        return false; // Saldo insuficiente
    }
}
