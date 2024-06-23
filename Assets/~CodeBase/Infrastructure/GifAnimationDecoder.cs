using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _CodeBase.Infrastructure
{
    public sealed class GifAnimationDecoder
    {
        public static GifInfo Process(string gifName, string targetSaveFile = null)
        {
            var frames = new List<Sprite>();
            var frameDelay = new List<float>();
            
            if (string.IsNullOrWhiteSpace(gifName)) return default;
            var path = Path.Combine(Application.streamingAssetsPath, gifName);

            using var decoder = new MG.GIF.Decoder(File.ReadAllBytes(path));
            var img = decoder.NextImage();
            
            while (img != null)
            {
                var tex = img.CreateTexture();

                var sprite = Object.Instantiate(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2f, 100));
                frames.Add(sprite);
                frameDelay.Add(img.Delay / 1000.0f);
                img = decoder.NextImage();
            }

            if (targetSaveFile != null)
            {
                frames = ExportSpriteToAsset(frames.ToArray(), targetSaveFile).ToList();
            }
            
            return new GifInfo(frames.ToArray(), frameDelay.ToArray());
        }
        
        
        private static Sprite[] ExportSpriteToAsset(Sprite[] sprites, string assetPath)
        {
            var savedSprs = new Sprite[sprites.Length];

            for (var i = 0; i < sprites.Length; i++)
            {
                var finalPath = $"{assetPath}_f{i}.png";
                var sprite = sprites[i];
                File.WriteAllBytes(finalPath, sprite.texture.EncodeToPNG());
                var textureImporter = (TextureImporter)AssetImporter.GetAtPath(finalPath);
                if (textureImporter)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.spritePixelsPerUnit = sprite.pixelsPerUnit;
                    textureImporter.spritePivot = sprite.pivot;
                    textureImporter.SaveAndReimport();
                }

                AssetDatabase.ImportAsset(finalPath, ImportAssetOptions.ForceUpdate);
                savedSprs[i] = AssetDatabase.LoadAssetAtPath<Sprite>(finalPath);
            }

            return savedSprs;
        }
    }

    [Serializable] public sealed class GifInfo
    {
        [PreviewField]
        [SerializeField] private Sprite[] _frames;
        
        [SerializeField] private float[] _framesDelays;
        
        
        public GifInfo(Sprite[] frames, float[] framesDelays)
        {
            _frames = frames;
            _framesDelays = framesDelays;
        }

        public Sprite[] Frames => _frames;
        public float[] FramesDelays => _framesDelays;
    }
}