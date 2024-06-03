using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    //Osigurava se da se objekat MusicManager nadje u sceni DontDestroyOnLoad kako bi se muzika pustala
    //kroz svaki nivo bez ometanja
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    //Funkcija koja se poziva kada se ucita neki nivo
    private void OnLevelWasLoaded(int level)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        //Unistava se ovaj objekat kada je scena mainmenu ili tavern jer se tu pusta druga muzika, 
        //i level1 zato sto muzika treba da krene iz pocetka
        if(sceneName == "Level1" || sceneName == "MainMenu" || sceneName == "Tavern")
        {
            Destroy(gameObject);
        }
    }
}
