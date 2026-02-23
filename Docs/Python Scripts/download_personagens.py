import os
import time
import requests
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

# --- CONFIGURAÇÕES ---
LISTA_PERSONAGENS = [
    'Amber', 'Barbara', 'Beidou', 'Bennett', 'Chongyun', 'Diluc', 'Fischl', 
    'Jean', 'Kaeya', 'Keqing', 'Klee', 'Lisa', 'Mona', 'Ningguang', 'Noelle', 
    'Qiqi', 'Razor', 'Sucrose', 'Venti', 'Xiangling', 'Xingqiu'
]

DIRETORIO_BASE = r"D:\recursos"
NOME_PASTA = "Multi-Wish Artworks"
URL_ALVO = "https://genshin-impact.fandom.com/wiki/Wish/Gallery"

# --- FUNÇÕES UTILITÁRIAS ---

def criar_pasta_destino():
    caminho_completo = os.path.join(DIRETORIO_BASE, NOME_PASTA)
    if not os.path.exists(caminho_completo):
        try:
            os.makedirs(caminho_completo)
            print(f"[PASTA] Criada: {caminho_completo}")
        except Exception as e:
            print(f"[ERRO] {e}")
            return None
    return caminho_completo

def baixar_imagem(url, caminho_arquivo):
    """Faz o download do arquivo via HTTP (igual ao 'Salvar Como')."""
    try:
        # User-agent para o site não bloquear o download
        headers = {'User-Agent': 'Mozilla/5.0'}
        resposta = requests.get(url, headers=headers, stream=True)
        if resposta.status_code == 200:
            with open(caminho_arquivo, 'wb') as f:
                for chunk in resposta.iter_content(1024):
                    f.write(chunk)
            return True
    except Exception as e:
        print(f"[ERRO DOWNLOAD] {e}")
    return False

# --- FUNÇÕES DO SELENIUM ---

def iniciar_browser():
    print("[BROWSER] Abrindo Chrome...")
    options = Options()
    options.add_argument("--start-maximized")
    options.add_experimental_option('excludeSwitches', ['enable-logging'])
    return webdriver.Chrome(options=options)

def encontrar_galeria_sessao(driver, wait, nome_sessao):
    """Encontra a caixa da galeria baseada no H2."""
    print(f"[BUSCA] Localizando sessão '{nome_sessao}'...")
    xpath = (
        f"//h2[contains(., '{nome_sessao}')]"
        f"/following::div[contains(@class, 'wikia-gallery')][1]"
    )
    galeria = wait.until(EC.presence_of_element_located((By.XPATH, xpath)))
    # Scroll para garantir que carregou
    driver.execute_script("arguments[0].scrollIntoView({block: 'center'});", galeria)
    time.sleep(1)
    return galeria

def encontrar_link_personagem(galeria_elemento, personagem):
    """
    Busca o link do personagem DENTRO da galeria encontrada.
    Retorna o elemento se achar, e avisa no console.
    """
    # XPath relativo: procura thumb -> link com title do personagem
    xpath = f".//div[contains(@class, 'thumb')]//a[contains(@title, '{personagem}')]"
    
    try:
        # Tenta encontrar o elemento
        elemento = galeria_elemento.find_element(By.XPATH, xpath)
        
        # Se chegou aqui, é porque achou!
        print(f"✅ ENCONTRADO: Personagem '{personagem}' localizado na galeria!")
        return elemento
        
    except:
        # Se cair aqui, é porque o find_element falhou
        print(f"⚠️ AVISO: Não consegui encontrar '{personagem}' nesta galeria.")
        return None

def salvar_imagem_modal(driver, wait, personagem, pasta_destino):
    """
    Função Especializada:
    1. Espera a estrutura: lightboxContainer > WikiaLightbox > media > img
    2. Pega o SRC.
    3. Salva no disco.
    """
    print(f"[MODAL] Extraindo imagem de {personagem}...")
    
    # O XPath Exato que você descreveu:
    xpath_imagem = (
        "//div[contains(@class, 'lightboxContainer')]"
        "//div[contains(@class, 'WikiaLightbox')]"
        "//div[contains(@class, 'media')]"
        "//img"
    )

    try:
        # Espera a imagem aparecer no modal
        img_element = wait.until(EC.visibility_of_element_located((By.XPATH, xpath_imagem)))
        
        # Pega a URL
        src_url = img_element.get_attribute("src")
        
        print(f"[LINK] URL encontrada: {src_url[:50]}...")
        
        # Define onde salvar
        caminho_final = os.path.join(pasta_destino, f"{personagem}.png")
        
        # Executa o download (Equivalente ao "Salvar Como")
        if baixar_imagem(src_url, caminho_final):
            print(f"✅ SUCESSO: {personagem}.png salvo!")
            return True
        else:
            print(f"❌ ERRO: Falha ao escrever o arquivo.")
            return False

    except Exception as e:
        print(f"❌ ERRO MODAL: Não achei a imagem na estrutura esperada. Detalhe: {e}")
        return False

def processar_personagens(driver, pasta_destino):
    wait = WebDriverWait(driver, 10)
    
    # 1. Acha a galeria geral
    try:
        galeria = encontrar_galeria_sessao(driver, wait, NOME_PASTA)
    except:
        print("Galeria não encontrada. Fim.")
        return

    # 2. Loop nos personagens
    for personagem in LISTA_PERSONAGENS:
        print(f"\n--- {personagem} ---")
        
        link = encontrar_link_personagem(galeria, personagem)
        
        if link:
            try:
                # Scroll e Click
                driver.execute_script("arguments[0].scrollIntoView({block: 'center'});", link)
                time.sleep(0.5)
                driver.execute_script("arguments[0].click();", link)
                
                # Pausa para animação do modal
                time.sleep(2)

                # 3. Chama a função nova de salvar
                salvar_imagem_modal(driver, wait, personagem, pasta_destino)

            except Exception as e:
                print(f"Erro no fluxo: {e}")
            
            # 4. Fecha o modal (ESC)
            try:
                webdriver.ActionChains(driver).send_keys(Keys.ESCAPE).perform()
                time.sleep(1.5) # Espera modal sumir
            except:
                pass
        else:
            print(f"[PULAR] {personagem} não encontrado.")

# --- MAIN ---

def main():
    pasta = criar_pasta_destino()
    if not pasta: return

    driver = iniciar_browser()
    try:
        driver.get(URL_ALVO)
        time.sleep(3)
        
        # Tenta aceitar cookies
        try:
            btn = WebDriverWait(driver, 5).until(EC.element_to_be_clickable((By.XPATH, "//*[contains(text(), 'I Accept')]")))
            btn.click()
        except:
            pass

        processar_personagens(driver, pasta)
        
        print("\n" + "="*30 + "\nCONCLUÍDO! 🎉\n" + "="*30)

    except Exception as e:
        print(f"Erro Fatal: {e}")
    finally:
        time.sleep(3)
        driver.quit()

if __name__ == "__main__":
    main()