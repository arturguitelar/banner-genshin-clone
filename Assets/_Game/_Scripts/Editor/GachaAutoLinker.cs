using UnityEngine;
using UnityEditor;
using Game.Data;
using Game.Core;

public class GachaAutoLinker : EditorWindow
{
    // Caminhos exatos baseados nos seus prints (Ajuste se mudar a pasta no futuro)
    private const string PATH_SPLASH = "Assets/_Game/Art/Sprites/Characters/Splash";
    private const string PATH_ICONS = "Assets/_Game/Art/Sprites/Characters/Icons";

    [MenuItem("Genshin/🔧 Link Characters Only")]
    public static void LinkCharacters()
    {
        Object[] selectedObjects = Selection.objects;
        int count = 0;

        foreach (Object obj in selectedObjects)
        {
            // 1. Verifica se é um Item de Gacha
            if (obj is GachaItemSO item)
            {
                // 2. Só processa se for PERSONAGEM (Ignora armas em PT-BR)
                if (item.itemType == GachaType.Character)
                {
                    LinkCharacterSprites(item);
                    count++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"<color=green>Pronto!</color> Processou {count} personagens.");
    }

    private static void LinkCharacterSprites(GachaItemSO item)
    {
        // O nome do arquivo (ex: "Amber")
        string targetName = item.name.Trim();

        // --- 1. Busca Splash Art ---
        // Procura: "Amber" do tipo Sprite, APENAS na pasta de Splash
        string[] splashGuids = AssetDatabase.FindAssets($"{targetName} t:Sprite", new[] { PATH_SPLASH });

        if (splashGuids.Length > 0)
        {
            // Pega o primeiro que achar (geralmente só tem um se o nome for exato)
            string path = AssetDatabase.GUIDToAssetPath(splashGuids[0]);
            item.splashArt = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        else
        {
            Debug.LogWarning($"Não achei Splash para: {targetName}");
        }

        // --- 2. Busca Ícone ---
        // Procura: "Amber" do tipo Sprite, APENAS na pasta de Icons
        string[] iconGuids = AssetDatabase.FindAssets($"{targetName} t:Sprite", new[] { PATH_ICONS });

        if (iconGuids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(iconGuids[0]);
            item.icon = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        else
        {
            Debug.LogWarning($"Não achei Ícone para: {targetName}");
        }

        // Marca como alterado para salvar
        EditorUtility.SetDirty(item);
    }
}