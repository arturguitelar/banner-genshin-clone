# 🌠 Genshin Impact Gacha Clone (Unity 6)

> Um clone robusto e testável do sistema de Gacha do [Genshin Impact](https://genshin.hoyoverse.com/), desenvolvido com foco em Arquitetura de Software, TDD e Unity 6 (URP).

![License](https://img.shields.io/badge/license-MIT-green)
![Unity](https://img.shields.io/badge/Unity-6000.0.0+-black?logo=unity)
![Status](https://img.shields.io/badge/status-development-orange)

## 📺 Sobre o Projeto

Este projeto faz parte de uma série de vídeos do canal **[Killdragon](https://www.youtube.com/@ArturKilldragon)**, onde exploramos como criar sistemas de jogos complexos utilizando engenharia de software profissional e Inteligência Artificial.

O objetivo não é apenas copiar o visual, mas replicar a **lógica matemática** (Soft Pity, Hard Pity, 50/50) garantindo que tudo funcione através de Testes Unitários antes mesmo de abrir a Unity.

## ✨ Funcionalidades (Core)

- 🎲 **Sistema de Probabilidade Real:** Implementação fiel das taxas de drop (0.6% base, Soft Pity a partir do 74º tiro).
- 🛡️ **Hard Pity & 50/50:** Lógica garantida para drops de 5★ no 90º tiro e sistema de garantia de banner.
- 🧪 **TDD (Test Driven Development):** Mais de 90% da lógica de gacha é coberta por testes automatizados (`NUnit`).
- 🏗️ **Arquitetura Desacoplada:** A lógica do Gacha (`GachaSystem.cs`) desconhece a existência da Unity, facilitando testes e manutenção.
- 🎨 **Unity 6 + URP:** Renderização otimizada para o estilo Anime Cel-Shaded.

## 🛠️ Tecnologias Utilizadas[[1](https://www.google.com/url?sa=E&q=https%3A%2F%2Fvertexaisearch.cloud.google.com%2Fgrounding-api-redirect%2FAUZIYQH2WJGacvRHdo7n5EjGnUFLfhdXhTkLBn_F7XdTXEHS7Mb2nu38kQKYGtxC_Z8Q_iv7of2FWXPJZ0ZEvRPsoh9AO3txWJZWRDqGT1tL9g207AcL9O8VekLWu53gHgmTk08TsJL_)][[2](https://www.google.com/url?sa=E&q=https%3A%2F%2Fvertexaisearch.cloud.google.com%2Fgrounding-api-redirect%2FAUZIYQFKjTn15ta2nvJP2Q2rQBo2OJZeriAoqRdqwtJQLRNfUBFFdyj0H1_-oLnU5Dk80Nmal8e3uMPSnAGuEPp0L1aoMXy_p4LzUFH8yhKMCGL2kn_xDqN93CYzDwKYy4t0Mm91tOzIaPM%3D)][[3](https://www.google.com/url?sa=E&q=https%3A%2F%2Fvertexaisearch.cloud.google.com%2Fgrounding-api-redirect%2FAUZIYQFq6EskxNmBuUysLzTPg240zxxp93p9TQeFrXtC3rx56bo889GTjutQss-Mt2lkQBviSYYthvd6lFqXHgc8IxPgcnNVk_16bxkYw3OlDvg-u8zL6WljQbNIS7pImVhRxwll1lwI2mBd0vFFEGz5FT-iYREt2D2ZDsY%3D)][[4](https://www.google.com/url?sa=E&q=https%3A%2F%2Fvertexaisearch.cloud.google.com%2Fgrounding-api-redirect%2FAUZIYQH76HNzun_KiQB8vsHx4aqiwTzSwSim7ycX-WnUvR3UREjzS4fMaijEVpF91WCSnjvhG4NrUVj9MWPmXzfzVVXqwQcJLK4V7NE41xq2LWy1SdtAMTrXbvcfu9qKo_Xi8KJQDo3puwL6bymunxbIKl_yM9jekA%3D%3D)][[5](https://www.google.com/url?sa=E&q=https%3A%2F%2Fvertexaisearch.cloud.google.com%2Fgrounding-api-redirect%2FAUZIYQHEgmyW6kMjwxlz30dMZJo70r-izZaHePIbhgPFvXn1Eznnvux76r0xAaLkXjjUyaJY-nA6GE6OhyRtYE4VFHpoKp0vnKC-4US9Rope0NXi2DIRJIfnhxfpUqQGA2p23Gyt49L2I87tHEpGp5CJEzZX_9jPrb508kuNxW3WTgKt5wNH3b7zjI-5HWPR9uIq_wlEH8BzxiU5307ANDI%3D)][[6](https://www.google.com/url?sa=E&q=https%3A%2F%2Fvertexaisearch.cloud.google.com%2Fgrounding-api-redirect%2FAUZIYQEGxO3p6OofrNRJwxR9AzZx7niod73oRJ3gBYioEYaaOgZ5eulSXEuMfzcy-nyZVjgWGExLcmoA8Zg3SFhGejNIYc9IYqcHbP9mZVWjxLV-6hrcgdFP1minutQZqhGcgBwuOYFyhYdA5EjGaaz9oAHqFmDH8I1cCGaIlxomK7AmUaQQ_1ncnXryzdHUYwIt4xVCdObaRpAva-0%3D)][[7](https://www.google.com/url?sa=E&q=https%3A%2F%2Fvertexaisearch.cloud.google.com%2Fgrounding-api-redirect%2FAUZIYQE7RATJqBrasl_ACswziMNAvamWParg6kmLn1rxczIX8Q6MTiWtaYuOWLswziy5TUBgqUi3IEUTARRXROw6zjSOwyxT0VIz7GDSxxmZ47uxXv7hwPsKTFBIZCHm3ERrplIEVw81KNI%3D)][[8](https://www.google.com/url?sa=E&q=https%3A%2F%2Fvertexaisearch.cloud.google.com%2Fgrounding-api-redirect%2FAUZIYQGrXjaIVBbGSV9hAYGA7qPsC2tyxJy46zdcON3_DFXE7FPX8PUoVJ1scrzvpmx2qOdBc5Pb6K80c6DE6kf3LHlN3s5OAvZJ9ekAlKbAQ0s01lTkgm9FtKMhf-Uqw4-bV67TGLHgqJosyz8UkvuqWzrUmkX9klu3IQ%3D%3D)][[9](https://www.google.com/url?sa=E&q=https%3A%2F%2Fvertexaisearch.cloud.google.com%2Fgrounding-api-redirect%2FAUZIYQFO9SA4lXo4vbKMMaszSrSTMlLEP596hebeqkGGZy6C61T4xAgIutExZfhCiv0zZOY6IHBAbsxTPnFnJvrKr8u71ofor4bYbuvoK5So4MQ8v7AJut0bmErVZWyDZdj9icCbLHtE_aKtAkYS-4ut1tU%3D)]

- **Engine:** Unity 6 (URP - Universal Render Pipeline)
- **Linguagem:** C#
- **Testes:** Unity Test Framework (NUnit)
- **Dados:** ScriptableObjects (para Banners e Personagens)
- **Versionamento:** Git + Git LFS

## 🚀 Como Rodar

1. Certifique-se de ter o **Unity Hub** e a versão **Unity 6000.0.x** instalada.
2. Clone este repositório:
   ```bash
   git clone https://github.com/SEU_USUARIO/GenshinGacha_Clone.git
   ```
3. Adicione o projeto no Unity Hub e abra.
4. Para ver a mágica sem rodar o jogo:

- Vá em Window > General > Test Runner
- Rode os testes e veja a matemática funcionando!

Configurado com _.gitmessage_ para garantir que as mensagens de git tenham o mesmo formato e sejam em português.

```bash
git config --local commit.template .gitmessage
```

_Desenvolvido com 💜 por **Artur (Killdragon)** e a **Gemini Team**._
