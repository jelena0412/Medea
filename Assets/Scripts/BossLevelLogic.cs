using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossLevelLogic : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private PlayerCombat playerCombat;
    [SerializeField]
    private GameObject deathMessage;
    [SerializeField]
    private GameObject gameFinishedPanel;
    [SerializeField]
    private GameObject[] enemyList;
    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private float spawnCooldown = 10f;

    private List<GameObject> enemySpawnedList = new List<GameObject>();
    private float timer;
    private bool levelFinished = false;

    private void Update()
    {
        if(!levelFinished)
        {
            timer += Time.deltaTime;
            if (timer >= spawnCooldown)
            {
                SpawnRandomEnemy();
                timer = 0;
            }
        }
        
    }

    public void MedeaKilledLogic()
    {
        levelFinished = true;
        gameFinishedPanel.SetActive(true);
        foreach(GameObject obj in enemySpawnedList)
        {
            if(obj != null)
            {
                Destroy(obj);
            }
        }
    }
    public void DisplayDeathMessage()
    {
        deathMessage.SetActive(true);
    }
    private void SpawnRandomEnemy()
    {
        GameObject enemy = Instantiate(enemyList[Random.Range(0, enemyList.Length)],
            spawnPoints[Random.Range(0, spawnPoints.Length)].position,
            Quaternion.identity);
        enemySpawnedList.Add(enemy);
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void OpenTavern()
    {
        SceneManager.LoadScene(9);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(2);
    }
    public void PlayChangeSceneSound()
    {
        audioSource.Play();
    }
}
