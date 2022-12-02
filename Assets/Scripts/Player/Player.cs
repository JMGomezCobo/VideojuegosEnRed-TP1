using Managers;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviourPun
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private Transform emitter;

        protected PlayerStats PlayerStats { get => playerStats; set => playerStats = value; }
        private Rigidbody2D Rigidbody2D { get; set; }

        private Collider2D Collider2D { get; set; }

        protected Transform Emitter { get => emitter; set => emitter = value; }

        private int CurrentHealth { get; set; }

        protected Animator Animator { get => animator; set => animator = value; }
        public PhotonView _PhotonView { get => _photonView; set => _photonView = value; }
        private TextMeshPro Tmp { get => healthText; set => healthText = value; }
        public bool IsOnLava { get; set; } = false;
        protected GameManager GameManager { get; set; }

        public bool IsShield { get; set; }

        protected UIManager UIManager;

        [SerializeField] private Animator animator;
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private TextMeshPro healthText;
        [SerializeField] private TextMeshPro _playerName;
        
        public virtual void Start()
        {
            playerStats.Execute();
            
            GameManager = FindObjectOfType<GameManager>();
            UIManager   = FindObjectOfType<UIManager>();

            Collider2D  = GetComponent<Collider2D>();
            Rigidbody2D = GetComponent<Rigidbody2D>();

            CurrentHealth = playerStats.Health;

            _playerName.text = _photonView.Owner.NickName;
        }

        public virtual void Update()
        {
        
        }

        public void TakeDamage(int damage)
        {
            _photonView.RPC("DamageTaken", RpcTarget.All, damage);
        
            ChangePlayerHealth();
            
            if (CurrentHealth <= 0)
            {
                _photonView.RPC("Die", _photonView.Owner);
            }
        }

        [PunRPC]
        public void Die()
        {
            PhotonNetwork.Destroy(gameObject);
            GameManager.SetLoser(this);
        }

        public void ApplyKnockBack(Vector2 direction)
        {
            _photonView.RPC("AddKnockBack", RpcTarget.All, direction);
        }

        private void ChangePlayerHealth()
        {
            _photonView.RPC("UpdatePlayerHealth", RpcTarget.All);
        }
    
        [PunRPC]
        public void AddKnockBack(Vector2 direction)
        {
            Rigidbody2D.AddForce(-direction * playerStats.KnockbackForce, ForceMode2D.Impulse);
        }

        [PunRPC]
        public void DamageTaken(int damage)
        {
            CurrentHealth -= damage;
        }

        [PunRPC]
        public void UpdatePlayerHealth()
        {
            Tmp.text = CurrentHealth.ToString();
        }
    }
}