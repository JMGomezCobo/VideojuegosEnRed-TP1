using Photon.Pun;
using UnityEngine;

namespace Skills
{
    public class Shield : MonoBehaviourPun
    {
        private PhotonView _photonView;
    
        public GameObject Player { get; set; }

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
        }

        private void Update()
        {
            if (!_photonView.IsMine) return;
        
            transform.position = Player.transform.position;
        }
    }
}