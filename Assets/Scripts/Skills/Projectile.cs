using Photon.Pun;
using Player;
using UnityEngine;

namespace Skills
{
    public class Projectile : MonoBehaviourPun
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private PhotonView _photonView;
        
        private Rigidbody2D _rigidbody2D;
    
        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_photonView.IsMine) return;

            Player.Player playerHit = collision.GetComponent<Player.Player>();
        
            if (playerHit == null) return;
            Vector2 direction = (transform.position - collision.transform.position).normalized;
        
            if (!playerHit.IsShield)
            {
                playerHit.ApplyKnockBack(direction);
                playerHit.TakeDamage(playerStats.Damage);
            
                PhotonNetwork.Destroy(gameObject);
            }
        
            else
            {
                _rigidbody2D.velocity *= -1f;
            }
        }
    }
}