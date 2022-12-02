using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Utilities
{
    public class LavaCollider : MonoBehaviourPun
    {
        [SerializeField] private PhotonView pv;
        [SerializeField] private int lavaDmg = 2;
        [SerializeField] private float lavaDmgInterval = 1f;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!pv.IsMine) return;

            Player.Player playerHit = collision.GetComponent<Player.Player>();

            if (!playerHit) return;
            
            playerHit.IsOnLava = true;
            StartCoroutine(TakingDamage(playerHit));
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!pv.IsMine) return;

            Player.Player playerHit = collision.GetComponent<Player.Player>();
            
            if (playerHit)
            {
                playerHit.IsOnLava = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!pv.IsMine) return;

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
                player.TakeDamage(lavaDmg);
                yield return new WaitForSeconds(lavaDmgInterval);
            }
            
            yield return null;
        }
    }
}