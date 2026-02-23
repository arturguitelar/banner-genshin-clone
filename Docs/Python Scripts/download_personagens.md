# 🌟 Genshin Impact Character Art Downloader

Este script automatiza o download das artes de desejo ("Full Wish Artworks" ou "Multi-Wish Artworks") dos personagens de Genshin Impact, extraindo-as diretamente da Galeria da Wiki oficial (Fandom).

## 📋 O que ele faz?

1.  Abre o navegador Google Chrome automaticamente via Selenium.
2.  Acessa a galeria de desejos da Wiki.
3.  Aceita os cookies do site (se necessário).
4.  Localiza a galeria específica ("Full Wish Artworks").
5.  Itera sobre uma lista pré-definida de personagens.
6.  Clica na miniatura, abre o modal em alta resolução e baixa a imagem original (`.png`).
7.  Salva tudo automaticamente na pasta configurada.

## 🛠️ Pré-requisitos

- **Python 3.x** instalado.
- **Google Chrome** instalado.
- Conexão com a Internet.

## 📦 Instalação

1.  Clone este repositório ou baixe o arquivo `.py`.
2.  Instale as dependências necessárias executando o comando abaixo no terminal:

```bash
pip install selenium requests
```

## ⚙️ Configuração

Abra o arquivo do script e edite as variáveis no topo conforme sua necessidade:

WEAPONS_LIST: Adicione ou remova os nomes das armas de banner que deseja baixar (devem corresponder aos nomes na Wiki em inglês).

DIRETORIO_BASE: O local onde a pasta de imagens será criada (Padrão: D:\recursos).

NOME_PASTA: O nome da subpasta (Padrão: Weapons Artworks).

## 🚀 Como Rodar

No terminal, navegue até a pasta onde o script está salvo e execute:

```bash
python  nome_do_arquivo.py
```

O navegador abrirá e fará todo o trabalho sozinho. Apenas aguarde a mensagem de _"MISSÃO CUMPRIDA"_ no console.
