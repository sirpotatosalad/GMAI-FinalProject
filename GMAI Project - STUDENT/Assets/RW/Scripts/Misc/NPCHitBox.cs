using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class NPCHitBox : MonoBehaviour
    {
        public int attackDamage;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Character>().TakeDamage(attackDamage);
            }
        }
    }
}

