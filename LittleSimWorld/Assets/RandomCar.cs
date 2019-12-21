using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCar : MonoBehaviour
{
    public Vector2 MoveDirection = new Vector2(1,0);
    public Collider2D CarStopZone;
    public float CarMaxAcceleration = 0.1f;
    private bool isBreaking = false;
    public LayerMask CarStopable;
    public float MaxBreakTime = 1;
    public float CurrentAcceleration = 1;
    public float AccelerationSpeed = 0.01f;

    public float CarLifetime = 30;
    public float CurrentLifetime = 0;
    private bool WillBeDestroyedAfterOutsideOfTheScreen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (!CarStopZone.IsTouchingLayers(CarStopable.value) && !isBreaking)
        {
            Move();
        }
        else
        {
            if (!isBreaking)
            {
                Break().Start();
            }
        }

        CurrentLifetime += Time.deltaTime;
        if (CurrentLifetime > CarLifetime)
        {
            WillBeDestroyedAfterOutsideOfTheScreen = true;
        }

    }
    private void OnBecameInvisible()
    {
        if (WillBeDestroyedAfterOutsideOfTheScreen)
        {
            Destroy(gameObject);
        }
    }
    public void Move()
    {
        if(CurrentAcceleration < CarMaxAcceleration)
        {
            CurrentAcceleration += AccelerationSpeed;
        }
        transform.Translate(MoveDirection * CurrentAcceleration, Space.Self);
    }
    public  IEnumerator<float> Break()
    {
       
        isBreaking = true;
        float breakingTime = 0;
        while (breakingTime < MaxBreakTime)
        {
            transform.Translate((MoveDirection * CurrentAcceleration) * ( MaxBreakTime - breakingTime), Space.Self);
            breakingTime += Time.deltaTime;
            yield return 0f;
           // Debug.Log(breakingTime);
        }
        while (CarStopZone.IsTouchingLayers(CarStopable.value))
        {
            yield return 0f;
        }
        CurrentAcceleration = 0;
        isBreaking = false;
    }
}
