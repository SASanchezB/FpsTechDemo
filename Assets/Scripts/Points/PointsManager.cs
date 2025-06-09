using UnityEngine;
using System;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance { get; private set; }

    private int currentPoints;

    public Action<int> onPointsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Solo una instancia
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void AddPoints(int amount)
    {
        currentPoints += amount;
        onPointsChanged?.Invoke(currentPoints);
    }

    public void RemovePoints(int amount)
    {
        if (currentPoints - amount >= 0)
        {
            currentPoints -= amount;
            onPointsChanged?.Invoke(currentPoints);
        }
    }

    public int GetPoints()
    {
        return currentPoints;
    }
}
