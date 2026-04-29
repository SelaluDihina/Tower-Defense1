using UnityEngine;

// Daftar tipe musuh resmi!
public enum EnemyType 
{ 
    RajaTikus, 
    TikusBiasa, 
    TikusLincah 
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyType enemyType; 
    public float lives;
    public int damage;
    public float speed;
    public int goldReward;
}