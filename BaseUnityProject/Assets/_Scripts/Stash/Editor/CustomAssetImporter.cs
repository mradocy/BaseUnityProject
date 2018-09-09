using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Automatically applies settings to assets upon import.
/// more info: http://www.sarpersoher.com/a-custom-asset-importer-for-unity/
/// </summary>
class CustomAssetImporter : AssetPostprocessor {
    
    void OnPreprocessTexture() {
        
        //string fileName = System.IO.Path.GetFileName(assetPath);
        //TextureImporter textureImporter = assetImporter as TextureImporter;

        // (turns out this wasn't even needed)
        // set texture import spriteMeshType to FullRect (makes some vertex shaders play nice)
        //TextureImporterSettings textureSettings = new TextureImporterSettings();
        //textureImporter.ReadTextureSettings(textureSettings);
        //textureSettings.spriteMeshType = SpriteMeshType.FullRect;
        ////textureSettings.spriteExtrude = 0;
        //textureImporter.SetTextureSettings(textureSettings);
        
    }
    
    void OnPreprocessAudio() {
        //AudioImporter audioImporter = assetImporter as AudioImporter;

    }
    
    void OnPostprocessTexture(Texture2D import) { }
    
    void OnPostprocessAudio(AudioClip import) { }
    
}