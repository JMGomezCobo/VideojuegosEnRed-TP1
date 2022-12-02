using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(PhotonView))]
    public class DamageCollider : MonoBehaviourPun
    {
        private PhotonView _photonView;
        
        [Header("Settings")]
        [SerializeField] private int _layerDamage = 2;
        [SerializeField] private float _damageRate = 1f;

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_photonView.IsMine) return;

            Player.Player playerHit = collision.GetComponent<Player.Player>();

            if (!playerHit) return;
            
            playerHit.IsOnLava = true;
            StartCoroutine(TakingDamage(playerHit));
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!_photonView.IsMine) return;

            Player.Player playerHit = collision.GetComponent<Player.Player>();
            
            if (playerHit)
            {
                playerHit.IsOnLava = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!_photonView.IsMine) return;

            Player.Player playerHit = collision.GetComponent<Player.Player>();
            
            if (playerHit)
            {
                playerHit.IsOnLava = false;
            }
        }

        private IEnumerator TakingDamage(Player.Player player)
        {
            while (player.IsOnLava)
            {
                player.TakeDamage(_layerDamage);
                yield return new WaitForSeconds(_damageRate);
            }
            
            yield return null;
        }
    }
}