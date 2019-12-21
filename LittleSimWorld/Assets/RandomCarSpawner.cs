using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCarSpawner : MonoBehaviour
{
    public ObjectRandomizer Pool;
    public float MinSpawnTime = 5;
    public float MaxSpawnTime = 60;
    public float TimeWithoutSpawning = 0;
    private float CurrentUpdateTime;

    private float UpdatePeriod;

    public AnimationCurve Probs;
    // Start is called before the first frame update
    private void Awake()
    {
        UpdatePeriod = MaxSpawnTime / MinSpawnTime;
    }
   

    private void FixedUpdate()
    {
        TimeWithoutSpawning += Time.deltaTime;
        CurrentUpdateTime += Time.deltaTime;
        if(CurrentUpdateTime > UpdatePeriod)
        {
            CurrentUpdateTime = 0;
            float chance = Probs.Evaluate(Mathf.Clamp(TimeWithoutSpawning, MinSpawnTime, MaxSpawnTime));
            if (chance <= Random.Range(1, 100))
            {
                TimeWithoutSpawning = 0;
                SpawnCar();
            }
        }
    }

    public void SpawnCar()
    {
        GameObject car = (GameObject) Pool.RandomObject();
        GameObject InstantiatedCar = Instantiate(car, transform.position, car.transform.rotation);
    }
}
