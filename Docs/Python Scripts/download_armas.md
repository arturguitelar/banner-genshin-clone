# ⚔️ Genshin Impact Weapon Art Downloader

Este script automatiza o download das artes de desejo ("Multi-Wish Artwork") das armas de Genshin Impact. Diferente dos personagens, as armas possuem páginas individuais, exigindo uma estratégia de navegação mais robusta.

## 📋 O que ele faz?

1.  Gera a URL da Wiki para cada arma baseada em uma lista de nomes.
2.  Acessa a página individual de cada arma.
3.  Utiliza uma estratégia de carregamento `eager` e rolagem agressiva para lidar com o _Lazy Loading_ e o peso da Wiki do Fandom.
4.  Localiza a imagem correta verificando a legenda (`caption`) que contenha "Wish" ou "Artwork" e selecionando a miniatura correspondente.
5.  Abre o modal, extrai a URL da imagem em alta qualidade e faz o download.
6.  Salva na pasta configurada.

## 🛠️ Pré-requisitos

- **Python 3.x** instalado.
- **Google Chrome** instalado.
- Conexão com a Internet estável (a Wiki é pesada).

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

**Nota:** O script pode parecer "travar" ou rolar a página rapidamente. Isso é normal. O modo de carregamento foi configurado para ignorar anúncios lentos e focar apenas no conteúdo principal para agilizar o processo.
