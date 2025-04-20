using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using YG;

public class Restart : MonoBehaviour
{
    [SerializeField] private Button revive;
    [SerializeField] private Button reload;
    [SerializeField] private Button settings;

    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI record;
    [SerializeField] private TextMeshProUGUI plusCoins;

    public static int soundsDiasbled = 0;
    public AudioSource clickAudio;
    void OnEnable()
    {
        clickAudio = GameObject.FindGameObjectWithTag("ClickAudio").GetComponent<AudioSource>();
     /*   plusCoins.text = (waveCount * 10).ToString();
        if (YG2.lang == "ru")
            record.text = "Рекорд: " + StaticGameManager.record.ToString();
        else
            record.text = "Record: " + StaticGameManager.record.ToString();
        if (StaticGameManager.record < (waveController.waveCount - 1))
        {
            if (YG2.lang == "ru")
                record.text = "Новый рекорд: " + (waveController.waveCount - 1).ToString();
            else
                record.text = "New Record: " + (waveController.waveCount - 1).ToString();
            StaticGameManager.record = (waveController.waveCount - 1);
            //   GP_Player.Set("Record", StaticGameManager.record);
            //   GP_Player.Sync(SyncStorageType.platform);
        }
     */
    }
    public void ClickSound()
    {
        if (soundsDiasbled != 1)
            clickAudio.Play();
    }
    public void PlayAnotherOne()
    {
        ClickSound();
        SceneManager.LoadScene(0);
        StaticGameManager.startNow = true;
    }
    public void GoToMenu()
    {
        ClickSound();
        StaticGameManager.startNow = false;
        SceneManager.LoadScene(0);
    }
    public void Respawn()
    {
        ClickSound();
    }
    // Показать rewarded video
    //  public void ShowRewarded() => GP_Ads.ShowRewarded("COINS", OnRewardedReward, OnRewardedStart, OnRewardedClose);


    // Начался показ
    //private void OnRewardedStart() => Debug.Log("ON REWARDED: START");
    // Получена награда
    //  private void OnRewardedReward(string value)
    //{
    //    WaveController.Instance.YOURESPAWN();
    //}
    public void stopTime()
    {
        Time.timeScale = 0;
    }

    // Закончился показ
    private void OnRewardedClose(bool success) => Debug.Log("ON REWARDED: CLOSE");
}
