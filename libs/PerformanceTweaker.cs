using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(PerformanceTweaker.MainMod), "Performance & Lighting Tweaker", "1.4.0", "Custom")]
[assembly: MelonGame(null, null)]

namespace PerformanceTweaker
{
    public class MainMod : MelonMod
    {
        private MelonPreferences_Category category;
        private MelonPreferences_Entry<bool> enableRenderDistanceTweak;
        private MelonPreferences_Entry<float> renderDistanceValue;
        private MelonPreferences_Entry<bool> disableShadows;
        private MelonPreferences_Entry<bool> reduceLighting;
        private MelonPreferences_Entry<float> lightMultiplier;
        private MelonPreferences_Entry<float> ambientMultiplier;

        public override void OnInitializeMelon()
        {
            category = MelonPreferences.CreateCategory("PerformanceTweaker", "Fitur Optimasi Performa");

            enableRenderDistanceTweak = category.CreateEntry("EnableRenderDistanceTweak", true, "Aktifkan Batas Render Distance");
            renderDistanceValue = category.CreateEntry("RenderDistanceValue", 100f, "Jarak Render Distance (dalam meter)");

            disableShadows = category.CreateEntry("DisableShadows", true, "Matikan Bayangan Sepenuhnya");

            reduceLighting = category.CreateEntry("ReduceLighting", true, "Aktifkan Pengurangan Cahaya");
            lightMultiplier = category.CreateEntry("LightMultiplier", 0.5f, "Pengali Intensitas Lampu (0.0 - 1.0)");
            ambientMultiplier = category.CreateEntry("AmbientMultiplier", 0.5f, "Pengali Ambient Light (0.0 - 1.0)");

            category.SaveToFile();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            ApplyPerformanceTweaks();
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                category.LoadFromFile();
                ApplyPerformanceTweaks();
                MelonLogger.Msg("Konfigurasi direload!");
            }
        }

        private void ApplyPerformanceTweaks()
        {
            if (enableRenderDistanceTweak.Value)
            {
                Camera[] cameras = Camera.allCameras;
                if (cameras != null)
                {
                    foreach (Camera cam in cameras)
                    {
                        if (cam != null)
                            cam.farClipPlane = renderDistanceValue.Value;
                    }
                }
            }

            if (disableShadows.Value)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
                QualitySettings.shadowDistance = 0f;
            }

            if (reduceLighting.Value)
            {
                Light[] lights = Object.FindObjectsOfType<Light>();
                if (lights != null)
                {
                    foreach (Light light in lights)
                    {
                        if (light == null) continue;

                        if (disableShadows.Value)
                        {
                            light.shadows = LightShadows.None;
                        }
                        light.intensity *= lightMultiplier.Value;
                    }
                }

                RenderSettings.ambientLight *= ambientMultiplier.Value;
                RenderSettings.ambientIntensity *= ambientMultiplier.Value;
            }

            MelonLogger.Msg("[Tweaker] Tweak berhasil diterapkan.");
        }
    }
}
