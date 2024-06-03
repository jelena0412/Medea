using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class RiddleLogic : MonoBehaviour
{
    [SerializeField]
    private GameObject riddlePanel;
    [SerializeField]
    private GameObject solvedRiddlePanel;
    [SerializeField]
    private TextMeshProUGUI letter1Text;
    [SerializeField]
    private TextMeshProUGUI letter2Text;
    [SerializeField]
    private TextMeshProUGUI letter3Text;
    [SerializeField]
    private TextMeshProUGUI letter4Text;
    [SerializeField]
    private TextMeshProUGUI message1Text;
    [SerializeField]
    private TextMeshProUGUI message2Text;
    //Ovde se cuvaju reci iz kojih se izvlaci zagonetka, mogu se stalno dodavati neke nove reci i kod ce
    //raditi, samo ce biti vise mogucnosti i manje ponavljanja
    [SerializeField]
    private List<string> riddleWords;

    private BaseLevelLogic levelLogic;
    private GameObject player;
    private Transform playerTransform = null;
    //Svaka od ovih lista predstavlja niz brojeva koji se mogu pojaviti prilikom promene jednog od onih slova,
    //jedan od tih slova jeste resenje dok su ostala slova nasumicno generisana i postavljena u listu
    private List<char> lettersequence1 = new List<char>();
    private List<char> lettersequence2 = new List<char>();
    private List<char> lettersequence3 = new List<char>();
    private List<char> lettersequence4 = new List<char>();
    //Lista od 4 slova koja predstavlja resenje zagonetke
    private List<char> solution = new List<char>();
    //Svaka od ovih listi ima svoj brojac kako bi se znalo na kom trenutnom slovu se igrac nalazi
    private int sequence1Counter = 0;
    private int sequence2Counter = 0;
    private int sequence3Counter = 0;
    private int sequence4Counter = 0;

    //Inicijalizuju se neke promenljive i generise se zagonetka odmah na pocetku nivoa
    private void Start()
    {
        levelLogic = GameObject.Find("LevelLogic").GetComponent<BaseLevelLogic>();
        player = GameObject.Find("Player");
        levelLogic.InitiateRiddleLevel();
        GenerateRiddle();
    }
    private void GenerateRiddle()
    {
        //Cuva se pozicija nasumicno izabranog slova da se prikaze igracu poruka na kojoj poziciji treba
        //da gleda slovo
        int position = 0, letterShift;
        //Pomocna promenljiva koja cuva enkriptovano slovo nakon sto se generise, kako bi mogao da se sacuva
        //u resenju i da se kasnije prikaze u poruci za resavanje
        char encryptedLetter;
        //Prvo se generise 4 nasumicnih slova 
        for (int i = 0; i < 4; i++)
        {
            lettersequence1.Add(GetRandomLetter());
            lettersequence2.Add(GetRandomLetter());
            lettersequence3.Add(GetRandomLetter());
            lettersequence4.Add(GetRandomLetter());
        }
        //Bira se nasumicno jedna rec od ponudjenih reci u listi
        string randomWord = riddleWords[Random.Range(0, riddleWords.Count - 1)];
        //Ispisuje se ta rec kao prva poruka igracu
        message1Text.text = randomWord;

        //Onda se generisu i slova koja ce se nekriptovati na nasumicnim pozicijama i smestanju se u resenje
        for (int i = 0; i < 4; i++)
        {
            encryptedLetter = EncryptLetter(randomWord, ref position);
            //Promenljiva koja odredjuje za koliko ce se to nasumicno izabrano slovo pomeriti u ASCII kodu
            letterShift = Random.Range(0, 5);
            //Dodaje se nasumicno slovo koje je pomereno u listu sa resenjima
            solution.Add((char)((int)encryptedLetter + letterShift));
            //U drugoj poruci se ispisuje pozicija na kojoj je izabrano slovo i za koliko pomeraja je 
            //pomereno to slovo
            message2Text.text += position + "= +" + letterShift + " | ";
        }
        //Dodaju se ta resenja u liste slova
        lettersequence1.Add(solution[0]);
        lettersequence2.Add(solution[1]);
        lettersequence3.Add(solution[2]);
        lettersequence4.Add(solution[3]);
        //Ispis resenja u konzoli
        Debug.Log("Resenje je=" + solution[0] + "" + solution[1] + "" + solution[2] + "" + solution[3] + "");
        //U tekstu kao UI elementu se ispisuju slova u nasumicnom redosledu tako da ne budu prikazani u
        //istom redosledu kao sto su generisani
        letter1Text.text = "" + lettersequence1[Random.Range(0, 4)];
        letter2Text.text = "" + lettersequence2[Random.Range(0, 4)];
        letter3Text.text = "" + lettersequence3[Random.Range(0, 4)];
        letter4Text.text = "" + lettersequence4[Random.Range(0, 4)];

    }
    //Funkcija koja proverava da li je igrac pogodio resenje. Poziva se svaki put kada se klikne dugme za
    //promenu slova
    private void CheckSolution()
    {
        //Ukoliko je resenje pronadjeno, prikazuje se poruka da je zagonetka uspesno resena
        if (lettersequence1[sequence1Counter] == solution[0] &&
            lettersequence2[sequence2Counter] == solution[1] &&
            lettersequence3[sequence3Counter] == solution[2] &&
            lettersequence4[sequence4Counter] == solution[3])
        {
            levelLogic.RiddleSolved();
            CloseRiddlePanel();
            solvedRiddlePanel.SetActive(true);
        }
    }
    //Funkcija koja iz date reci pronalazi nasumicno slovo i vraca slovo koje je izabrano i vraca poziciju
    //tog slova preko reference.
    private char EncryptLetter(string word, ref int position)
    {
        char letter;
        int letterIndex = Random.Range(0, word.Length);
        //Uzima slovo i poziciju tog slova iz reci koja je prosledjena
        letter = word[letterIndex];
        position = letterIndex + 1;
        return letter;
    }
    //Funkcija za otvaranje panela gde se resava zagonetka
    private void OpenRiddlePanel()
    {
        //Osigurava se da igrac ne moze da udara ni da se krece dok je panel otvoren
        player.GetComponent<PlayerCombat>().disabledPlayer = true;
        player.GetComponent<PlayerMovement>().disabledPlayer = true;
        riddlePanel.SetActive(true);
    }
    //Funkcija koja se poziva kada igrac zatvori panel sa zagonetkom
    public void CloseRiddlePanel()
    {
        //Igracu se dozvoljava kretanje
        player.GetComponent<PlayerCombat>().disabledPlayer = false;
        player.GetComponent<PlayerMovement>().disabledPlayer = false;
        riddlePanel.SetActive(false);
    }
    //Funkcija koja zatvara panel 
    public void CloseSovledRiddlePanel()
    {
        solvedRiddlePanel.SetActive(false);
    }
    //Dugme na gore poziva ovu funkciju za promenu slova na gore
    public void LetterUp(int letterPosition)
    {
        switch(letterPosition)
        {
            case 1:
                //Za svako slovo je potrebno da se uveca brojac, ukoliko brojac bude veci od broja ponudjenih
                //slova onda se resetuje brojac, i onda se odgovarajuce slovo prikaze u tekstu
                sequence1Counter++;
                if (sequence1Counter >= lettersequence1.Count)
                    sequence1Counter = 0;
                letter1Text.text = ""+lettersequence1[sequence1Counter];
                break;
            case 2:
                sequence2Counter++;
                if (sequence2Counter >= lettersequence2.Count)
                    sequence2Counter = 0;
                letter2Text.text = "" + lettersequence2[sequence2Counter];
                break;
            case 3:
                sequence3Counter++;
                if (sequence3Counter >= lettersequence3.Count)
                    sequence3Counter = 0;
                letter3Text.text = "" + lettersequence3[sequence3Counter];
                break;
            case 4:
                sequence4Counter++;
                if (sequence4Counter >= lettersequence4.Count)
                    sequence4Counter = 0;
                letter4Text.text = "" + lettersequence4[sequence4Counter];
                break;
        }
        //Nakon svake promene slova proveri da li je igrac pogodio resenje
        CheckSolution();
    }
    //Dugme na dole poziva ovu funkciju koja primenjuje logiku za menjanje odgovarajuceg slova
    public void LetterDown(int letterPosition)
    {
        switch (letterPosition)
        {
            case 1:
                sequence1Counter--;
                if (sequence1Counter < 0)
                    sequence1Counter = lettersequence1.Count - 1;
                letter1Text.text = "" + lettersequence1[sequence1Counter];
                break;
            case 2:
                sequence2Counter--;
                if (sequence2Counter < 0)
                    sequence2Counter = lettersequence2.Count - 1;
                letter2Text.text = "" + lettersequence2[sequence2Counter];
                break;
            case 3:
                sequence3Counter--;
                if (sequence3Counter < 0)
                    sequence3Counter = lettersequence3.Count - 1;
                letter3Text.text = "" + lettersequence3[sequence3Counter];
                break;
            case 4:
                sequence4Counter--;
                if (sequence4Counter < 0)
                    sequence4Counter = lettersequence4.Count - 1;
                letter4Text.text = "" + lettersequence4[sequence4Counter];
                break;
        }
        CheckSolution();
    }

    private void Update()
    {
        if(playerTransform != null)
        {
            //Ukoliko igrac pritisne slovo E treba da se otvori panel za resavanje, ali isto tako treba i da
            //se zatvori ukoliko je otvoren
            if(Input.GetKeyDown(KeyCode.E))
            {
                if (riddlePanel.activeSelf)
                    CloseRiddlePanel();
                else
                    OpenRiddlePanel();
                //U svakom slucaju treba da se zatvori poruka koja govori igracu da je zagonetka resena,
                //jer se ne moze otvoriti rucno
                CloseSovledRiddlePanel();
            }
            //Panel se takodje moze zatvoriti ako igrac pritisne ESCAPE na tastaturi
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                CloseRiddlePanel();
                CloseSovledRiddlePanel();
            }
        }
    }
    //Ukoliko igrac dodje u kontakt sa zagonetkom, daje mu mogucnost da moze da je otvori jer u kodu za proveru
    //gleda da li postoji playerTransform
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerTransform = collision.transform;
        }
    }
    //Kada igrac se udalji od zagonetke, gubi mogucnost da otvori zagonetku
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTransform = null;
        }
    }
    //Funkcija koja generise nasumicna mala slova engleskog alfabeta. Te brojeve pretvara u char zbog ASCII
    //tabele
    private char GetRandomLetter()
    {
        int randomAscii = Random.Range(97, 123); // a = 97, z = 122
        return (char)randomAscii;
    }
}
