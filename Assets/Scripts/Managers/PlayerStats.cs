using UnityEngine;

public class PlayerStats : MonoBehaviour {
    public static int Money;
    public int startMoney = 200;

    public static int Lives;
    public int startLives = 10; // Langsung set 20 di sini biar aman

    void Awake() { 
        // Pake Awake biar data siap sebelum GameManager butuh di Start
        Money = startMoney;
        Lives = startLives;
    }
}