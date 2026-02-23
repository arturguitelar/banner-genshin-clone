import os
import time
import requests
from selenium import webdriver
from selenium.common.exceptions import TimeoutException
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

# --- LISTA DE ARMAS ---
WEAPONS_LIST = [
    # --- 5 STARS ---
    "Aquila Favonia", "Skyward Blade",
    "Wolf's Gravestone", "Skyward Pride",
    "Primordial Jade Winged-Spear", "Skyward Spine",
    "Amos' Bow", "Skyward Harp",
    "Lost Prayer to the Sacred Winds", "Skyward Atlas",
    
    # --- 4 STARS ---
    # Favonius & Sacrificial
    "Favonius Sword", "Favonius Greatsword", "Favonius Lance", "Favonius Warbow", "Favonius Codex",
    "Sacrificial Sword", "Sacrificial Greatsword", "Sacrificial Fragments", "Sacrificial Bow",
    
    # Others
    "The Flute", "Lion's Roar", "The Bell", "Rainslasher", "Dragon's Bane", 
    "Eye of Perception", "The Widsith", "Rust", "The Stringless"
]

# --- CONFIGURAÇÕES ---
DIRETORIO_BASE = r"D:\recursos"
NOME_PASTA = "Weapons Artworks"
BASE_URL = "https://genshin-impact.fandom.com/wiki/"

# --- FUNÇÕES UTILITÁRIAS ---

def criar_pasta_destino():
    caminho_completo = os.path.join(DIRETORIO_BASE, NOME_PASTA)
    if not os.path.exists(caminho_completo):
        try:
            os.makedirs(caminho_completo)
            print(f"[PASTA] OK: {caminho_completo}")
        except Exception as e:
            print(f"[ERRO] {e}")
            return None
    return caminho_completo

def baixar_imagem(url, caminho_arquivo):
    try:
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

def formatar_url_arma(nome_arma):
    return f"{BASE_URL}{nome_arma.replace(' ', '_')}"

# --- FUNÇÕES SELENIUM ---

def iniciar_browser():
    print("[BROWSER] Iniciando Chrome em modo 'EAGER'...")
    options = Options()
    options.add_argument("--start-maximized")
    options.add_experimental_option('excludeSwitches', ['enable-logging'])
    
    # --- O PULO DO GATO ---
    # 'eager': O Selenium devolve o controle assim que o HTML é lido, 
    # sem esperar imagens, css ou scripts pesados terminarem.
    options.page_load_strategy = 'eager'
    
    driver = webdriver.Chrome(options=options)
    
    # Define um limite: se a página não responder em 20s, lança erro (que vamos tratar)
    driver.set_page_load_timeout(20)
    
    return driver

def rolar_pagina_na_marra(driver):
    """
    Rola a página em etapas para 'acordar' o Lazy Loading das imagens.
    """
    print("[SCROLL] Forçando renderização...")
    
    # Desce até a metade
    driver.execute_script("window.scrollTo(0, document.body.scrollHeight * 0.5);")
    time.sleep(1)
    
    # Desce tudo (Fundo)
    driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")
    time.sleep(2) # Espera crítica para a imagem aparecer
    
    # Sobe um pouco (para o elemento não ficar escondido no footer)
    driver.execute_script("window.scrollTo(0, document.body.scrollHeight * 0.7);")
    time.sleep(1)

def salvar_imagem_modal(driver, wait, nome_arquivo, pasta_destino):
    print(f"[MODAL] Baixando: {nome_arquivo}...")
    
    xpath_imagem = (
        "//div[contains(@class, 'lightboxContainer')]"
        "//div[contains(@class, 'WikiaLightbox')]"
        "//div[contains(@class, 'media')]"
        "//img"
    )

    try:
        img_element = wait.until(EC.visibility_of_element_located((By.XPATH, xpath_imagem)))
        src_url = img_element.get_attribute("src")
        print(f"[LINK] {src_url[:60]}...")
        
        caminho_final = os.path.join(pasta_destino, f"{nome_arquivo}.png")
        
        if baixar_imagem(src_url, caminho_final):
            print(f"✅ SALVO: {nome_arquivo}.png")
            return True
        else:
            print(f"❌ ERRO GRAVAÇÃO")
            return False

    except Exception as e:
        print(f"❌ ERRO MODAL: {e}")
        return False

def encontrar_e_clicar_arma(driver, wait, nome_arma):
    print(f"[BUSCA] Procurando thumb com legenda 'Wish/Artwork'...")
    
    # XPath Irmãos: Legenda -> Irmão de Cima (Thumb) -> Link Imagem
    xpath_target = (
        "//div[contains(@class, 'lightbox-caption') and (contains(., 'Wish') or contains(., 'Artwork'))]"
        "/preceding-sibling::div[contains(@class, 'thumb')]"
        "//a[contains(@class, 'image')]"
    )
    
    try:
        elemento = wait.until(EC.presence_of_element_located((By.XPATH, xpath_target)))
        
        # Centraliza
        driver.execute_script("arguments[0].scrollIntoView({block: 'center'});", elemento)
        time.sleep(1)
        
        print(f"✅ ACHEI! Clicando...")
        driver.execute_script("arguments[0].click();", elemento)
        
        time.sleep(3) # Tempo para abrir o modal HD
        return True
        
    except:
        print(f"⚠️ AVISO: Não achei a seção 'Multi-Wish Artwork' para {nome_arma}.")
        return False

def processar_armas(driver, pasta_destino):
    wait = WebDriverWait(driver, 10)
    
    for arma in WEAPONS_LIST:
        url_arma = formatar_url_arma(arma)
        print(f"\n--- Processando: {arma} ---")
        
        # --- NAVEGAÇÃO SEGURA ---
        try:
            print(f"[NAVEGAÇÃO] Acessando URL...")
            driver.get(url_arma)
        except TimeoutException:
            print("⚠️ A página demorou demais. Parando carregamento e continuando na marra!")
            driver.execute_script("window.stop();") # Mata o carregamento infinito
        
        # Mesmo se der timeout ou carregar rápido, esperamos um pouquinho pro DOM estabilizar
        time.sleep(2)
        
        try:
            # Rola para carregar as imagens lá embaixo
            rolar_pagina_na_marra(driver)
            
            # Tenta clicar
            if encontrar_e_clicar_arma(driver, wait, arma):
                salvar_imagem_modal(driver, wait, arma, pasta_destino)
                
                # Fecha Modal
                try:
                    webdriver.ActionChains(driver).send_keys(Keys.ESCAPE).perform()
                except:
                    pass
            else:
                print(f"[PULAR] Arte não encontrada (Elemento não apareceu).")

        except Exception as e:
            print(f"❌ ERRO NO FLUXO: {e}")

# --- MAIN ---

def main():
    pasta = criar_pasta_destino()
    if not pasta: return

    driver = iniciar_browser()
    try:
        # Acessa home só pra garantir cookie (pode dar timeout tbm, tratamos igual)
        try:
            driver.get(BASE_URL)
        except:
            driver.execute_script("window.stop();")
            
        time.sleep(2)
        try:
            btn = WebDriverWait(driver, 3).until(EC.element_to_be_clickable((By.XPATH, "//*[contains(text(), 'I Accept')]")))
            btn.click()
            print("[COOKIES] Aceitos.")
        except:
            pass

        processar_armas(driver, pasta)
        
        print("\n" + "="*30 + "\nFIM DO TESTE! 🧪\n" + "="*30)

    except Exception as e:
        print(f"❌ ERRO GERAL: {e}")
    finally:
        time.sleep(2)
        driver.quit()

if __name__ == "__main__":
    main()