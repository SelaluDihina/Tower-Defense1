using UnityEngine;

// [PENTING]: Ini daftar ID musuh lu
[System.Serializable]
public struct EnemyGroup 
{
    public EnemyType enemyType;
    public int count;
    public float spawnInterval;
}

[CreateAssetMenu(fileName = "New Wave", menuName = "TowerDefense/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("Simple Settings (Spawner Lama)")]
    public int enemiesPerWave;
    public float spawnInterval;
    public EnemyType enemyType;

    [Header("Modular Settings (Wave Manager Baru)")]
    public EnemyGroup[] enemyGroups;
}