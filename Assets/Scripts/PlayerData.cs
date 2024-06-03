using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData 
{
    //MaxTime koja cuva maksimalno moguce vreme i preostalo vreme koje se prenosi kroz nivoe
    public static float maxTime = 180f;
    public static float remainingTime=180f;
    //MaxHealth koji cuva maksimalan moguci health i preostali health koji se prenosi kroz nivoe
    public static float maxHealth = 100f;
    public static float remainingHealth=100f;
    //MaxPlayerDamage koji cuva maksimalan moguci attack damage i damage koji treba igrac da prenosi kroz nivoe
    public static float maxPlayerDamage = 20f;
    public static float playerDamage=20f;
    //Promenljiva koja cuva koliko igrac poseduje novca kako bi mogao u krcmi da kupuje unapredjenja.
    public static int playerCoins = 0;

    //Niz koji cuva podatke o tome koja unapredjenja su kupljena
    public static bool[] boughtSigns = new bool[6];
}
