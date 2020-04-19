using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable : IHealth
{
    void Damage(int damage);
}
