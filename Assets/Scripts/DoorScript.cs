using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoorScript:MonoBehaviour
{
    private BaseLevelLogic levelLogic;
    [SerializeField]
    private GameObject door;
    [SerializeField]
    private Sprite openedDoorSprite;
    private bool isOpened = false;
    [SerializeField]
    private List<string> levelScenes;
   
    //Funkcija koju poziva BaseLevelLogic kako bi igrac onda mogao da prodje kroz vrata na sledeci nivo
    public void OpenDoor()
    {
        levelLogic = GameObject.Find("LevelLogic").GetComponent<BaseLevelLogic>();
        isOpened = true;
        door.GetComponent<SpriteRenderer>().sprite = openedDoorSprite;
    }
    //Funkcija koja se poziva kada vrata dodju u dodir sa nekim objektom. Ako je to igrac i ako su vrata
    //otvorena, onda moze da se ucita nova scena
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("Player") && isOpened == true)
            {
                SwitchToNewScene();
            }
        }
    }
    //Funkcija koja se poziva kada se ucitava nova scena, potrebno je da se zna koji je trenutni indeks
    //scene da bi mogao da u funkciji NextLevel prosledi indeks scene koja treba da se ucita
    private void SwitchToNewScene()
    { 
        int currentScene = levelScenes.IndexOf(SceneManager.GetActiveScene().name);
        levelLogic.NextLevel(currentScene+1);
    }
}
