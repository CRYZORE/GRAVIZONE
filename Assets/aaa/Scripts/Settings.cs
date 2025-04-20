using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.LowLevel;
using Unity.VisualScripting;
using YG;
namespace YG
{
    public partial class SavesYG
    {
        public int maxFpsIndex;
        public int currentResolutionIndex;

        public float sensitivity;
        public float masterVolume;
        public float SFXVolume;
        public float musicVolume;
    } 
}
public class Settings : MonoBehaviour
{
    public int maxFpsIndex = 2;
    public int currentResolutionIndex;

    public float sensitivity;
    public float masterVolume;
    public float SFXVolume;
    public float musicVolume;

    public Slider masterSlider;
    public Slider SFXSlider;
    public Slider musicSlider;

    public TMP_Text sensitivityText;
    public Slider sensitivitySlider;

    public bool fullscreen;

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fpsDropdown;

    public int[] fpsList;
    public Resolution[] resolutions;

    public AudioMixer audioMixer;
    public AudioSource clickAudio;
    public TMP_Text languageText;
    public TMP_Text[] allText;
    public PlayerCam cam;
    [Header("Настройки")]
    [SerializeField] private List<string> allLanguages = new List<string> { "en", "ru" }; // Все доступные языки
    private List<string> availableLanguages = new List<string>(); // Актуальный список языков
    private int currentLanguageIndex = 0;

    private void Start()
    {
        //    if (GP_Init.isReady)
        //  {
       // Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        //GP_Player.Load();
        // GP_Player.Sync(SyncStorageType.platform);
        clickAudio = GameObject.FindGameObjectWithTag("ClickAudio").GetComponent<AudioSource>();
        if (StaticGameManager.isFirstPlay == 0)
        {
            fullscreen = Screen.fullScreen;
            StaticGameManager.isFirstPlay = 1;
            //  StaticGameManager.maxFpsIndex;
            // StaticGameManager.currentResolutionIndex;
            masterVolume = 1;
            SFXVolume = 1;
            musicVolume = 1;
            sensitivity = 1;
            PlayerPrefs.SetInt("MaxFrames", maxFpsIndex);
            PlayerPrefs.SetInt("Resolution", currentResolutionIndex);
            PlayerPrefs.SetFloat("SFXVolume", 1);
            PlayerPrefs.SetFloat("musicVolume", 1);
            PlayerPrefs.SetFloat("masterVolume", 1);
            PlayerPrefs.SetFloat("sensitivity", 1);
            //GP_Player.Set("musicVolume", 1);
            //GP_Player.Set("sensitivity", 1);

  
            if (fullscreen)
            {
                PlayerPrefs.SetInt("Fullscreen", 1);
            }
            else
            {
                PlayerPrefs.SetInt("Fullscreen", 0);
            }
  
            //GP_Player.Sync(SyncStorageType.platform);
        }
        else
        {
            //   GP_Player.Load();
         maxFpsIndex = PlayerPrefs.GetInt("MaxFPS");
         currentResolutionIndex = PlayerPrefs.GetInt("ScreenSize");
         masterVolume = PlayerPrefs.GetFloat("masterVolume");
         SFXVolume = PlayerPrefs.GetFloat("SFXVolume");
         musicVolume = PlayerPrefs.GetFloat("musicVolume");
         sensitivity = PlayerPrefs.GetFloat("sensitivity");

            /**
            if (PlayerPrefs.GetInt("Fullscreen") == 1)
            {
                fullscreen = true;
            }
            else
            {
                fullscreen = false;
            }
            **/
            LoadSettings();
        }
        SetSensetivity(sensitivity);
        if (!StaticGameManager.isMobile)
        {
            AddFPS();
            AddResolutions();
            //    ChangeMaxFPS(maxFpsIndex);
            //    ChangeResolution(currentResolutionIndex);
        }
        //  }
    }

    public void LoadSettings()
    {
        if (!StaticGameManager.isMobile)
        {
            //  Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, true);
            //  Application.targetFrameRate = fpsList[maxFpsIndex];
        }
        sensitivitySlider.value = sensitivity;
        masterSlider.value = masterVolume;
        SFXSlider.value = SFXVolume;
        musicSlider.value = musicVolume;
    }
    public void SetSensetivity(float sense)
    {
        sensitivity = sense;
        if (!StaticGameManager.isMobile)
        {
            cam.sensX = sensitivity*400;
            cam.sensY = sensitivity*400;
        }
        else
        {
            cam.sensX = sensitivity * 400;
            cam.sensY = sensitivity * 400;

        }
        // sensitivity = sense;
        sensitivityText.text = sensitivity.ToString("F2");
    }

    public void SaveSettings()
    {
        if (!StaticGameManager.isMobile)
        {
            //  Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, true);
            //  Application.targetFrameRate = fpsList[maxFpsIndex];
        }
        audioMixer.SetFloat("masterVolume", Mathf.Log10(masterVolume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(SFXVolume) * 20);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(musicVolume) * 20);

        /*  GP_Player.Set("sensitivity", sensitivity);
          GP_Player.Set("MaxFrames", maxFpsIndex);
          GP_Player.Set("Resolution", currentResolutionIndex);
          GP_Player.Set("masterVolume", masterVolume);
          GP_Player.Set("SFXVolume", SFXVolume);
          GP_Player.Set("musicVolume", musicVolume);
          GP_Player.Sync(SyncStorageType.platform);
        */
    }

    public void ChangeResolution(int num)
    {
       currentResolutionIndex = num;
        //  ClickSound();
    }

    /**
    public void ChangeFullscreen(int num)
    {
        if (num == 0)
        {
            fullscreen = false;
        }
        else
        {
            fullscreen = true;
        }
    }
    **/

    public void ChangeMaxFPS(int index)
    {
       maxFpsIndex = index;
        //  ClickSound();
    }

    public void AddResolutions()
    {
        resolutionDropdown.ClearOptions();
        resolutions = Screen.resolutions;

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (Screen.currentResolution.width == resolutions[i].width &&
            Screen.currentResolution.height == resolutions[i].height)
            {
              currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void AddFPS()
    {
        fpsDropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 0; i < fpsList.Length; i++)
        {
            string option = fpsList[i].ToString() + "fps";
            options.Add(option);
        }

        fpsDropdown.AddOptions(options);
        fpsDropdown.value = maxFpsIndex;
        fpsDropdown.RefreshShownValue();
    }

    public void SetMasterVolume(float level)
    {
        masterVolume = level;
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20);
    }

    public void SetSFXVolume(float level)
    {
        SFXVolume = level;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20);
    }

    public void SetMusicVolume(float level)
    {
        musicVolume = level;
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20);
    }

    public void ClickSound()
    {

            clickAudio.enabled = true;
            clickAudio.Play();
    }

}