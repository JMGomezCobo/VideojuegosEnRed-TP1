using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "Player", menuName = "Scriptable Object/Player", order = 0)]

    public class PlayerStats : ScriptableObject
    {
        [SerializeField] private int maxHealth = 100;

        [SerializeField] private float speed = 4;
        [SerializeField] private int damage = 10;
        [SerializeField] private float knockbackForce = 2f;
        [SerializeField] private float attackSpeed = 0.6f, dashSpeed = 6f;
        [SerializeField] private float projectileSpeed = 6f;
        [SerializeField] private float dashMult = 1.5f;

        public float Speed { get => speed; set => speed = value; }
        public int Damage { get => damage; set => damage = value; }
        public float KnockbackForce { get => knockbackForce; set => knockbackForce = value; }
        public int Health { get; set; }

        public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
        public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
        public float DashSpeed { get => dashSpeed; set => dashSpeed = value; }
        public float DashMult { get => dashMult; set => dashMult = value; }

        public void Execute()
        {
            Health = maxHealth;
        }
    }
}