using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject[] windows;
    public GameObject[] PC_Tips;
    public GameObject menu;
    public GameObject pause_canvas;
    public GameObject lose_canvas;
    public GameObject UI_canvas;
    public GameObject[] objects;
    public GameObject sharkDance;
    public GameObject shop;
    public Camera camera;
    public int currentWindow = -1;
    bool mobileActivation = false;
    public static bool gameIsPaused = false;
    public static bool gameCanBePaused = false;
    public static int soundsDiasbled = 0;
    public AudioSource clickAudio;
    public PlayerMovement movement;
    public Gun gun;
    public TextMeshProUGUI[] balances;
    public GameObject player;
    public GameObject basuka;
    [SerializeField] GameObject mobileUI;
    private float saveWalkSpeed, saveSprintSpeed;
    public bool gameIsStarted = false;
    public AudioClip startSound;
    public static int canShowCutscene = 0;
    private void Awake()
    {
        gameCanBePaused = false;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Time.timeScale = 1f;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }
    // �������� preloader
  //  public void ShowPreloader() => GP_Ads.ShowPreloader(OnPreloaderStart, OnPreloaderClose);

    // ������� �����
  //  private void OnPreloaderStart() => Debug.Log("ON PRELOADER: START");
    // ���������� �����
   // private void OnPreloaderClose(bool success) => Debug.Log("ON PRELOADER: CLOSE");
    public void OnApplicationFocus(bool focus)
    {
        if (!gameIsStarted)
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }
    public void GET_FUCKING_MOUSE()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }
    private void OnFullscreenStart() => Debug.Log("ON FULLSCREEN START");
    // ���������� �����
    private void OnFullscreenClose(bool success) => Debug.Log("ON FULLSCREEN CLOSE");
    private void Start()
    {
        clickAudio = GameObject.FindGameObjectWithTag("ClickAudio").GetComponent<AudioSource>();
        //if (GP_Init.isReady)
        // {
        //     ShowPreloader();
        //     AllBalanceSync();
        // }
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
    }
    public void DisableMovement()
    {
        movement.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        movement.canMove = false;
        movement.enabled = false;
        camera.enabled = false;
        gun.enabled = false;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }
    public void EnableMovement()
    {
        if (movement != null)
        {
            movement.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            movement.canMove = true;
            movement.enabled = true;
        }

        if (camera != null)
            camera.enabled = true;

        if (gun != null)
        {
            gun.enabled = true;
        }
        Debug.Log("SAS");
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
    private void CloseSettingsWindows()
    {
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].SetActive(false);
        }
        gameObject.GetComponent<Settings>().SaveSettings();
        ChangeWindow(currentWindow);
    }
    public void ChangeWindow(int num)
    {
        if (num == currentWindow)
        {
            currentWindow = -1;
            windows[0].SetActive(false);
        }
        else 
        {
            if (currentWindow >= 0)
            {   
                windows[currentWindow].SetActive(false);
            }
            windows[0].SetActive(true);
            currentWindow = num;
        }
        GET_FUCKING_MOUSE();
    }
    public void Startgame()
  {
      canShowCutscene = PlayerPrefs.GetInt("canShowCutscene", 0);
      if (canShowCutscene == 0)
      {
          PlayerPrefs.SetInt("canShowCutscene", 1);
          ClickSound();
          SceneManager.LoadScene(1);
      }
      else
      {
          if (StaticGameManager.isMobile)
          {
              for (int i = 0; i < PC_Tips.Length; i++)
              {
                  Destroy(PC_Tips[i]);
              }
          }
          camera.enabled = true;
          CloseSettingsWindows();
          StartCoroutine(SAS());
          menu.SetActive(false);
          gameCanBePaused = true;
          UnityEngine.Cursor.visible = false;
          UnityEngine.Cursor.lockState = CursorLockMode.Locked;
          ClickSound();
      }
  }
    private IEnumerator SAS()
    {
        yield return new WaitForSeconds(4f);
        UI_canvas.SetActive(true);
        mobileUI.SetActive(StaticGameManager.isMobile);
    }
        void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Tab)) || (mobileActivation))
        {
            PauseController();
        }
    }
        public void PauseController()
    {
        if (!gameIsPaused && gameCanBePaused)
        {
            mobileActivation = false;
            //   savedClickAudioTime = clickAudio.time;
            //  clickAudio.Pause();
            Activation();
            CloseSettingsWindows();
           //   gun.GetComponent<Gun>().enabled = false;
            //  player.GetComponent<Skills>().enabled = false;
            if (Application.isMobilePlatform)
            {
              //  camera.enabled = false;
                //  joystick.SetActive(false);
            }
            else
            {
             //   camera.enabled = false;
            }
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            gameIsPaused = true;
            pause_canvas.SetActive(true);
            Time.timeScale = 0f;

        }
        else if (gameIsPaused && gameCanBePaused)
        {
            gameIsPaused = false;
            //    clickAudio.time = savedClickAudioTime;
            //    clickAudio.Play();
            pause_canvas.SetActive(false);
            CloseSettingsWindows();
           //   gun.GetComponent<Gun>().enabled = true;
            //    player.GetComponent<Skills>().enabled = true;
            if (Application.isMobilePlatform)
            {
                camera.enabled = true;
                //     joystick.SetActive(true);
            }
            else
            {
                camera.enabled = true;
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }
            //  allSoundsToDisable[0].Play();
            //   allSoundsToDisable[8].Play();
            Time.timeScale = 1f;
        }
}
    void Activation()
    {
        var random = Random.Range(0, 101);
        if (random < 60)
        {
            return;
        }
        else
        {
            //  if(GP_Ads.IsFullscreenAvailable())
            // GP_Ads.ShowFullscreen(OnFullscreenStart, OnFullscreenClose);
        }
    }
    public void ResumeGameButton()
    {
        if (gameIsPaused && gameCanBePaused)
        {
            gameIsPaused = false;
            //  clickAudio.time = savedClickAudioTime;
            //      clickAudio.Play();
            CloseSettingsWindows();
            pause_canvas.SetActive(false);
          //  gun.GetComponent<Gun>().enabled = true;
          //  allSoundsToDisable[0].Play();
          //  allSoundsToDisable[8].Play();
          //  player.GetComponent<Skills>().enabled = true;
            Time.timeScale = 1f;
            if (Application.isMobilePlatform)
            {
                camera.GetComponent<PlayerCam>().enabled = true;
               // joystick.SetActive(true);
            }
            else
            {
                camera.GetComponent<PlayerCam>().enabled = true;
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }
            ClickSound();
        }
    }
    public void ClickSound()
    {

            clickAudio.enabled = true;
            clickAudio.Play();
    }
    public void StartSound()
    {
            clickAudio.PlayOneShot(startSound);
    }
    public void ExitGame()
    {
        ClickSound();
        Application.Quit();
    }
    public void GoToMenu()
    {
        ClickSound();
        SceneManager.LoadScene(0);
    }

}
