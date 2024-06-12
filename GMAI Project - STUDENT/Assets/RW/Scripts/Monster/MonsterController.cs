using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class MonsterController : MonoBehaviour
{

    public float detectionRange = 10f;
    public float curiousRange = 20f;

    public float playerStoppingDistance = 3.5f;
    public float pointStoppingDistance = 2.0f;

    public float chaseSpeed = 9.0f;
    public float investigateSpeed= 5.5f;
    public float patrolSpeed = 4.0f;

    public int hitPoints = 3;

    [Task]
    public bool IsDead {  get; private set; }

    [Task]
    public bool IsLow()
    {
        return hitPoints <= 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        IsDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitPoints <= 0)
        {
            IsDead = true;
        }
    }
}
