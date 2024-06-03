using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TavernLogic : MonoBehaviour
{
    private int playerCoins;
    private int selectedItem;
    [SerializeField]
    private AudioClip selectSound;
    [SerializeField]
    private AudioClip buySound;
    [SerializeField]
    private AudioClip changeSceneSound;
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private TextMeshProUGUI coinsText;
    [SerializeField]
    private GameObject[] boughtSigns;
    [SerializeField]
    List<int> shopItemsCost;
    private void Start()
    {
        playerCoins = PlayerData.playerCoins;
        coinsText.text = playerCoins.ToString();
        CheckIfBought();
    }
    public void OpenMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(2);
    }
    public void BuyItem()
    {
        if(playerCoins >= shopItemsCost[selectedItem])
        {
            if (boughtSigns[selectedItem].activeSelf) return;

            ImplementItemLogic();
        }
    }
    public void SelectItem(int selection)
    {
        selectedItem = selection;
    }
    private void ImplementItemLogic()
    {
        switch(selectedItem)
        {
            case 0:
                playerCoins -= 100;
                PlayerData.maxPlayerDamage += 10f;
                break;
            case 1:
                playerCoins -= 150;
                PlayerData.maxHealth += 25f;
                break;
            case 2:
                playerCoins -= 250;
                break;
            case 3:
                playerCoins -= 330;
                PlayerData.maxTime += 30f;
                break;
            case 4:
                playerCoins -= 180;
                break;
            case 5:
                playerCoins -= 37250;
                break;
        }
        boughtSigns[selectedItem].SetActive(true);
        PlayerData.boughtSigns[selectedItem] = true;
        coinsText.text = playerCoins.ToString();
        PlayerData.playerCoins = playerCoins;
    }
    private void CheckIfBought()
    {
        for (int i = 0; i < 6; i++)
        {
            if (PlayerData.boughtSigns[i])
                boughtSigns[i].SetActive(true);
        }
    }
    public void PlaySelectSound()
    {
        audioSource.clip = selectSound;
        audioSource.Play();
    }
    public void PlayBuySound()
    {
        audioSource.clip = buySound;
        audioSource.Play();
    }
    public void PlayChangeSceneSound()
    {
        audioSource.clip = changeSceneSound;
        audioSource.Play();
    }
}
