using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using TMPro; 

public class GameManager : MonoBehaviour
{
    public List<GameObject> enemies; 
    [SerializeField] GameObject endGamePanel; 
    [SerializeField] TMP_Text endGameText; 
    [SerializeField] PlayerCam mainCam; 
    [SerializeField] GameObject playerObj; 
    [SerializeField] TMP_Text timerText; 
    [SerializeField] float startTime; 
    [SerializeField] float currentTime; 
    [SerializeField] bool oneMinuteSoundPlayed, twoPlayerSoundPlayed; 
    [SerializeField] AudioClip oneMinuteSound, twoPlayerSound; 
    AudioSource source; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObj = GameObject.FindObjectOfType<Player>().gameObject; 
        source = GetComponent<AudioSource>();
        endGamePanel.SetActive(false); 
        currentTime = startTime; 
    }

    // Update is called once per frame
    void Update()
    {
        if(enemies.Count <= 0)
            Win(); 

        if(currentTime > 0)
            currentTime -= Time.deltaTime; 
        timerText.text = ((int)currentTime).ToString(); 
        if(!oneMinuteSoundPlayed && currentTime <= 60) {
            source.PlayOneShot(oneMinuteSound);
            oneMinuteSoundPlayed = true; 
        }
        if(enemies.Count <= 2 && !twoPlayerSoundPlayed) {
            source.PlayOneShot(twoPlayerSound); 
            twoPlayerSoundPlayed = true; 
        }
        if(currentTime <= 0)
            Lose(); 
    }
    public void Win() {
        endGameText.text = "Вы победили!"; 
        Cursor.lockState = CursorLockMode.Confined; 
        Cursor.visible = true; 
        endGamePanel.SetActive(true); 
        mainCam.enabled = false; 
        Destroy(playerObj); 
    }
    public void Lose() {
        endGameText.text = "Вы проиграли!"; 
        Cursor.lockState = CursorLockMode.Confined; 
        Cursor.visible = true; 
        endGamePanel.SetActive(true); 
        mainCam.enabled = false; 
        Destroy(playerObj); 
    }
}
