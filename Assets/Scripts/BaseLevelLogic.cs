using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseLevelLogic : MonoBehaviour
{
    [SerializeField]
    private GameObject musicManager;
    [SerializeField]
    private AudioClip changeSceneSound;
    [SerializeField]
    private AudioSource audioSource;
    public float timer = 0;
    public float maxTime;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI counterText;
    public int maxEnemies;
    private float enemiesKilled = 0;
    private bool timeStopped = false;

    private PlayerCombat playerCombat;

    public GameObject deathMessage;

    public DoorScript doorLogic;

    //Logika koja se pokrece kada se ucita nivo, inicijalizuju se promenljive za kasniju upotrebu
    void Awake()
    {
        playerCombat = GameObject.Find("Player").GetComponent<PlayerCombat>();
        maxTime = PlayerData.remainingTime;
        doorLogic = GameObject.Find("Door").GetComponent<DoorScript>();
        timer = maxTime;
        Invoke("CheckMusicManager", 1);
        timeStopped = false;
    }
    //
    private void CheckMusicManager()
    {
        GameObject musicManagerPrefab = GameObject.Find("MusicManager");

        //Ako ne postoji Music Manager, stvori ga ali samo u prvom nivou
        if (musicManagerPrefab == null)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;
            if (sceneName == "Level1")
                Instantiate(musicManager);
        }
    }

    //Prati se preostalo vreme i to vreme se formatira kao npr 1:30
    private void Update()
    {
        if(!timeStopped)
        {
            timer -= Time.deltaTime;
            timerText.text = ((int)timer).ToString();
            string formattedTime = ConvertSecondsToMinutesAndSeconds(timer);
            if (timer <= 0)
            {
                PlayerData.remainingTime = PlayerData.maxTime;
                playerCombat.TakeDamage(200, 0, null, 0);
                timeStopped = true;
            }

            timerText.text = formattedTime;
        }
        
    }
    //Pokrece se samo u 4 nivou gde ima zagonetka jer nema neprijatelje vec samo treba da se resi jedna
    //zagonetka
    public void InitiateRiddleLevel()
    {
        maxEnemies = 0;
        counterText.text = "0/1";
    }
    //Pokrece se kada se zagonetka resi da bi de lepo prikazao counter i da se otvore vrata
    public void RiddleSolved()
    {
        counterText.text = "1/1";
        doorLogic.OpenDoor();
    }
    private string ConvertSecondsToMinutesAndSeconds(float seconds)
    {
        if (seconds < 0)
        {
            seconds = Mathf.Abs(seconds);
        }

        int minutes = Mathf.FloorToInt(seconds / 60f);
        float remainingSeconds = seconds % 60f;

        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }
    //Azurira se broj ubijenih neprijatelja, i dodaje se u counter i proverava se ukoliko su svi neprijatelju
    //koji treba da budu porazeni, onda se otvaraju vrata za sledeci nivo
    public void EnemyKilled()
    {
        enemiesKilled++;
        counterText.text = enemiesKilled +"/"+maxEnemies;
        if (enemiesKilled == maxEnemies)
        {
            doorLogic.OpenDoor(); 
        }
    }
    //Logika da se sacuva helt i vreme preostalo za igraca pa se onda ucita nova scena
    public void NextLevel(int nextScene)
    {
        PlayerData.remainingHealth = playerCombat.playerHealth;
        PlayerData.remainingTime = timer;
        SceneManager.LoadScene(nextScene);
    }
    //Poziva se kada igrac umre, i prikazuje se panel sa nekim opcijama
    public void DisplayDeathMessage()
    {
        deathMessage.SetActive(true);
    }
    //Poziva dugme koje vraca igraca na main meni i resetuje preostalo vreme
    public void OpenMainMenu()
    {
        SceneManager.LoadScene(0);
        PlayerData.remainingTime = PlayerData.maxTime;
    }
    public void OpenTavern()
    {
        SceneManager.LoadScene(9);
        PlayerData.remainingTime = PlayerData.maxTime;
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(2);
        PlayerData.remainingTime = PlayerData.maxTime;
    }
    //Funkcija koju poziva svako dugme da bi se pustio zvuk kliktanja dugmeta
    public void PlayChangeSceneSound()
    {
        audioSource.clip = changeSceneSound;
        audioSource.Play();
    }
}
