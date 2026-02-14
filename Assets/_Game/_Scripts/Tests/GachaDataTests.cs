using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Game.Data; // Precisamos acessar seus ScriptableObjects

public class GachaDataTests
{
    // Variáveis que vamos usar em todos os testes
    private GachaPoolSO _mockPool;
    private GachaBannerSO _mockBanner;
    private GachaItemSO _itemDiluc;  // Representa o Padrão
    private GachaItemSO _itemNahida; // Representa o Destaque

    // [SetUp] roda ANTES de cada teste. Prepara o terreno.
    [SetUp]
    public void Setup()
    {
        // 1. Criamos os Itens em memória (Fake items)
        _itemDiluc = ScriptableObject.CreateInstance<GachaItemSO>();
        _itemDiluc.idName = "diluc";
        _itemDiluc.displayName = "Diluc (Standard)";

        _itemNahida = ScriptableObject.CreateInstance<GachaItemSO>();
        _itemNahida.idName = "nahida";
        _itemNahida.displayName = "Nahida (Featured)";

        // 2. Criamos o Pool (Mochileiro) e colocamos o Diluc lá
        _mockPool = ScriptableObject.CreateInstance<GachaPoolSO>();
        _mockPool.characters5Star = new List<GachaItemSO> { _itemDiluc }; // Importante inicializar a lista!

        // 3. Criamos o Banner e ligamos tudo
        _mockBanner = ScriptableObject.CreateInstance<GachaBannerSO>();
        _mockBanner.bannerName = "Banner Teste";
        _mockBanner.featuredUnit5Star = _itemNahida; // Destaque
        _mockBanner.standardPool = _mockPool;        // Ligação com o Pool <--- O PULO DO GATO
    }

    [Test]
    public void Banner_Can_Access_StandardPool_Items()
    {
        // O teste: Pedimos ao Banner para buscar um item "Padrão" (Loss).
        // Ele deve ir até o Pool e trazer o Diluc.

        GachaItemSO result = _mockBanner.GetStandard5Star();

        Assert.IsNotNull(result, "O Banner retornou nulo ao buscar no Pool Padrão.");
        Assert.AreEqual(_itemDiluc, result, "O Banner deveria ter trazido o Diluc do Pool.");
        Assert.AreEqual("Diluc (Standard)", result.displayName);
    }

    [Test]
    public void Banner_Has_Correct_Featured_Unit()
    {
        // Teste simples para garantir que configuramos o destaque certo
        Assert.AreEqual(_itemNahida, _mockBanner.featuredUnit5Star);
    }

    // Opcional: Limpeza após o teste (TearDown)
    // Na Unity o GC cuida disso em EditMode, mas é boa prática saber que existe.
    [TearDown]
    public void Teardown()
    {
        // Destrói os objetos da memória para não vazar
        Object.DestroyImmediate(_mockBanner);
        Object.DestroyImmediate(_mockPool);
        Object.DestroyImmediate(_itemDiluc);
        Object.DestroyImmediate(_itemNahida);
    }
}