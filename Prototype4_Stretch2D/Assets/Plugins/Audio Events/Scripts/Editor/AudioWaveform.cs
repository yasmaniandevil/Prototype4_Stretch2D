namespace Dimension19.AudioEvents.EditorWindows
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class AudioWaveform
    {
        public static Color marker = Color.green;

        private AudioSource audioSource = null;
        //private SpriteRenderer spriteRenderer = null;
        private int sampleSize;
        private float[] samples = null;
        private float[] waveform = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public AudioWaveform(AudioSource _audioSource = null)
        {
            audioSource = _audioSource;
        }

        ///// <summary>
        ///// Create Waveform
        ///// </summary>
        //public void CreateWaveform(int width = 1024, int height = 64)
        //{
        //    Texture2D waveformTexture = GetWaveformAsTexture(width, height);
        //    Rect rect = new Rect(Vector2.zero, new Vector2(width, height));
        //    spriteRenderer.sprite = Sprite.Create(waveformTexture, rect, Vector2.zero);

        //    arrow.transform.position = new Vector3(0f, 0f);
        //}

        /// <summary>
        /// Paints the Audio Waveform onto a Texture
        /// </summary>
        public Texture2D GetWaveformAsTexture(Color _BackgroundColor, Color _ForegroundColor, int width, int height, float peakMagnitude = 0.6f)
        {
            int halfHeight = height / 2;
            float heightScale = (float)height * peakMagnitude; // The Magnitude of the peaks for the Waveform

            // Get the Sound Data
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            waveform = new float[width];

            sampleSize = audioSource.clip.samples * audioSource.clip.channels;
            samples = new float[sampleSize];
            audioSource.clip.GetData(samples, 0);

            int packSize = (sampleSize / width);
            for (int i = 0; i < width; i++)
            {
                waveform[i] = Mathf.Abs(samples[i * packSize]);
            }

            // Map the Sound Data to the Texture
            // 1 - Clear
            // Sets all pixels on the Texture to the background colour
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    texture.SetPixel(x, y, _BackgroundColor);
                }
            }

            // 2 - Plot
            // Plots the appropriate Waveform pixels to the foreground colour
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < waveform[x] * heightScale; y++)
                {
                    texture.SetPixel(x, halfHeight + y, _ForegroundColor);
                    texture.SetPixel(x, halfHeight - y, _ForegroundColor);
                }
            }

            texture.Apply(); // Applies this to the texture

            return texture;
        }

        public Texture2D[] GetWaveformAsTextures(Color _BackgroundColor, Color _ForegroundColor, int width, int height, float peakMagnitude = 0.6f)
        {
            Texture2D[] textures = new Texture2D[0]; // Create new Texture2D array

            int halfHeight = height / 2;
            float heightScale = (float)height * peakMagnitude; // The Magnitude of the peaks for the Waveform

            //int MAX_TEXTURE_PIXELS = SystemInfo.maxTextureSize; // The maximum amount of pixels that a texture can have
            int MAX_TEXTURE_PIXELS = 2048 * 2048; // The maximum amount of pixels that a texture can have

            // Check if the texture is too big
            if (width * height > MAX_TEXTURE_PIXELS)
            {
                //Debug.Log("TEXTURE IS TOO BIG! Width * Height = " + width * height + " / " + MAX_TEXTURE_PIXELS);
                int amountOfTextures = Mathf.CeilToInt(width * height / MAX_TEXTURE_PIXELS) + 1; // Draw more appropriate textures over the default 1
                //Debug.Log("Amount of Textures to be drawn: " + amountOfTextures);

                // Get the Sound Data
                waveform = new float[width];

                sampleSize = audioSource.clip.samples * audioSource.clip.channels;
                samples = new float[sampleSize];
                audioSource.clip.GetData(samples, 0);

                int packSize = (sampleSize / width);
                for (int i = 0; i < width; i++)
                {
                    waveform[i] = Mathf.Abs(samples[i * packSize]);
                }

                // Resize the textures array
                textures = new Texture2D[amountOfTextures];

                // Populate the textures array with textures of the cut size
                for (int i = 0; i < amountOfTextures; i++)
                {
                    Texture2D texture = new Texture2D(width / amountOfTextures, height, TextureFormat.RGBA32, false);
                    textures[i] = texture;
                }

                // Create an X Position variable to hold
                int xPos = 0;

                // Map the Sound Data to the Texture
                for (int i = 0; i < amountOfTextures; i++)
                {
                    // 1 - Clear
                    // Sets all pixels on the Texture to the background colour
                    for (int x = 0; x < textures[i].width; x++)
                    {
                        for (int y = 0; y < textures[i].height; y++)
                        {
                            textures[i].SetPixel(x, y, _BackgroundColor);
                        }
                    }

                    // 2 - Plot
                    // Plots the appropriate Waveform pixels to the foreground colour
                    for (int x = 0; x < textures[i].width; x++)
                    {
                        for (int y = 0; y < waveform[xPos + x] * heightScale; y++)
                        {
                            textures[i].SetPixel(x, halfHeight + y, _ForegroundColor);
                            textures[i].SetPixel(x, halfHeight - y, _ForegroundColor);
                        }
                    }

                    xPos += textures[i].width; // Increment the X Position
                }
                
                // Apply all the textures
                for (int i = 0; i < textures.Length; i++)
                {
                    textures[i].Apply();
                }
            }
            else // If the texture is NOT too big
            {
                // Resize the textures array
                textures = new Texture2D[1];

                // Get the Sound Data
                textures[0] = new Texture2D(width, height, TextureFormat.RGBA32, false);
                waveform = new float[width];

                sampleSize = audioSource.clip.samples * audioSource.clip.channels;
                samples = new float[sampleSize];
                audioSource.clip.GetData(samples, 0);

                int packSize = (sampleSize / width);
                for (int i = 0; i < width; i++)
                {
                    waveform[i] = Mathf.Abs(samples[i * packSize]);
                }

                // Map the Sound Data to the Texture
                // 1 - Clear
                // Sets all pixels on the Texture to the background colour
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        textures[0].SetPixel(x, y, _BackgroundColor);
                    }
                }

                // 2 - Plot
                // Plots the appropriate Waveform pixels to the foreground colour
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < waveform[x] * heightScale; y++)
                    {
                        textures[0].SetPixel(x, halfHeight + y, _ForegroundColor);
                        textures[0].SetPixel(x, halfHeight - y, _ForegroundColor);
                    }
                }

                textures[0].Apply(); // Applies this to the texture
            }

            return textures;
        }
    }
}