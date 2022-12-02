using Photon.Pun;
using Player;
using UnityEngine;

namespace Skills
{
    public class Projectile : MonoBehaviourPun
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private PhotonView _photonView;

        [SerializeField] private LayerMask _destroyOnImpactLayer;
        
        private Rigidbody2D _rigidbody2D;
    
        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_photonView.IsMine) return;

            Player.Player playerHit = collision.GetComponent<Player.Player>();

            if (playerHit != null)
            {
                Vector2 direction = (transform.position - collision.transform.position).normalized;
        
                playerHit.ApplyKnockBack(direction);
                playerHit.TakeDamage(playerStats.Damage);
            
                PhotonNetwork.Destroy(gameObject);
            }

            else if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}