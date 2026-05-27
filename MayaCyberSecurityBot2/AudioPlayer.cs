using System;
using System.IO;
using System.Media;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace MayaCyberSecurityBot2
{
    public static class AudioPlayer
    {
        /// Plays the greeting WAV file asynchronously without blocking the WPF UI.
        /// </summary>
        public static async Task PlayGreetingAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "greeting.wav");

                    // Debug output - check Visual Studio Output window (View > Output)
                    System.Diagnostics.Debug.WriteLine($"[AUDIO] Looking for: {audioPath}");

                    if (File.Exists(audioPath))
                    {
                        using (SoundPlayer player = new SoundPlayer(audioPath))
                        {
                            player.Load();        //  Load file into memory first (critical!)
                            player.PlaySync();    //  Plays synchronously ON THE BACKGROUND THREAD
                                                  
                        }
                        System.Diagnostics.Debug.WriteLine("[AUDIO] Playback completed.");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[AUDIO]  greeting.wav NOT FOUND at: " + audioPath);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[AUDIO]  Error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[AUDIO] Stack: {ex.StackTrace}");
                }
            });
        }
    }
}

