using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Managers
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private List<Transform> _spawnPoints;

        private void Start()
        {
            switch (PhotonNetwork.PlayerList.Length)
            {
                case 1:
                    PhotonNetwork.Instantiate("Player", _spawnPoints[0].position, Quaternion.identity);
                    break;
                
                case 2:
                    PhotonNetwork.Instantiate("Player2", _spawnPoints[1].position, Quaternion.identity);
                    break;
                
                case 3:
                    PhotonNetwork.Instantiate("Player3", _spawnPoints[2].position, Quaternion.identity);
                    break;
                
                case 4:
                    PhotonNetwork.Instantiate("Player4", _spawnPoints[3].position, Quaternion.identity);
                    break;
            }
        }
    }
}