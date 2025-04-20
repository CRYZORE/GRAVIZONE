using UnityEngine;
using YG;
namespace YG
{
    public partial class SavesYG
    {
        public int isFirstPlay = 0;
        public int soundsDiasbled;
        public int record;
        public int coins = 0;
        public string systemLanguage;
        public int[] SAVE_PREFIX = {0,0,0,0};
    }
}
public class StaticGameManager : MonoBehaviour
{
    public int soundsDiasbled;
    public int record;
    public int coins = 0;
    public string systemLanguage;
    public int[] SAVE_PREFIX = { 0, 0, 0, 0 };
    public static bool isMobile = false;
    public static bool startNow = false;
    public static int isFirstPlay = 0;
    [SerializeField] GameObject mobileUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        isMobile = true; 
        if (Application.isMobilePlatform)
        {
            QualitySettings.SetQualityLevel(1);
            isMobile = true;
            Debug.Log("��� � �����: " + isMobile);
        }
        Debug.Log("��� � �����: " + isMobile);
    }
    public static void SaveBalance()
    {
        //    StaticGameManager.record = SavesYG.record;
    }





    /*   private void Awake()
       {
           if (GP_Init.isReady)
           {
               GP_Player.Load();
               systemLanguage = GP_Player.GetString("systemLanguage");
               isFirstPlay = GP_Player.GetInt("hasPlayed");
               coins = GP_Player.GetInt("Coins");
               record = GP_Player.GetInt("Record");
           }
           //   isMobile = true; 
           if (Application.isMobilePlatform)
           {
               isMobile = true;
           }
       }
       public static void SaveBalance()
       {
           GP_Player.Set("Record", record);
           GP_Player.Sync(SyncStorageType.platform);
       }
    */
}