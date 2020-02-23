using System;
using UnityEngine;

namespace EventCallbacks
{
    public abstract class Event<T> where T : Event<T> {
        public string Description;

        private bool hasFired;
        public delegate void EventListener(T info);
        private static event EventListener listeners;
        
        public static void RegisterListener(EventListener listener) {
            listeners += listener;
        }

        public static void UnregisterListener(EventListener listener) {
            listeners -= listener;
        }

        public void FireEvent() {
            if (hasFired) {
                throw new Exception("This event has already fired, to prevent infinite loops you can't refire an event");
            }
            hasFired = true;
            listeners?.Invoke(this as T);
        }
    }

    public class DebugEvent : Event<DebugEvent>
    {
        public int VerbosityLevel;
    }
    public class MissleHitEvent : Event<MissleHitEvent>
    {
        public int damage;
        public GameObject UnitGO;
    }
    public class EnemyShotHit : Event<EnemyShotHit>
    {
        public int damage;
        public GameObject UnitGO;
    }
    public class FireWeapon : Event<FireWeapon>
    {
        public bool fire;
    }

    public class EnemyDied : Event<EnemyDied>
    {
        public Transform parent;
        public Transform point;
        public Transform enemy;
    }

    public class ReachedPoint : Event<ReachedPoint>
    {
        public Transform objTransform;
        public Transform parentTransform;
    }

    public class FormationDead : Event<FormationDead>
    {
        
    }
}