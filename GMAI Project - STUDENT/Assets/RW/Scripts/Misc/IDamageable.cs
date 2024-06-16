using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// decided to make use of an interface that uses the TakeDamage() method
// moreso because its faster to just call all scripts that implement the IDamageable interface when doing hit registration between NPC and player characters
public interface IDamageable
{
    void TakeDamage(int damage);
}
