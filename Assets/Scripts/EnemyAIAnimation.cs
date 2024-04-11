using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIAnimation : MonoBehaviour
{
    [SerializeField] private EnemyAI _myAi;

    public void DamagePlayer() {
        _myAi.CauseDamage();
    }

    public void DisableAI() {
        _myAi.SetAi(false);
    }

    public void EnableAI() {
        _myAi.SetAi(true);
    }
}
