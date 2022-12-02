using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [Header("Photon Bindings")]
        [SerializeField] private PhotonView _photonView;
        
        [Header("Text Bindings")]
        [SerializeField] private TextMeshProUGUI startCountdownText;
        [SerializeField] private TextMeshProUGUI timeText;
        
        [SerializeField] private GameObject loseText;
        [SerializeField] private GameObject winText;
        [SerializeField] private GameObject waitingForHostText;
        [SerializeField] private GameObject waitingForPlayersText;

        [Header("Button Bindings")]
        [SerializeField] private Button MasterStartButton;
        
        private List<Player.Player> playerList;
        private float currentTime;
        
        public bool IsGameStarted { get; private set; }

        private void Start()
        {
            SetStartRequirements();
            playerList = new List<Player.Player>();
        }

        private void Update()
        {
            ManageTime();
        }

        private void SetWinner(Player.Player character)
        {
            //Update only the player that is the winner (comes from check last player standing method)
            var player = character._PhotonView.Owner;
            _photonView.RPC("UpdateWinner", player);
        }

        public void SetLoser(Player.Player character)
        {
            if (!winText.activeSelf)
            {
                Debug.Log("Player kill! Setting loser status");
                //When a player dies, he and only he will get the update 
                var player = character._PhotonView.Owner;
                _photonView.RPC("UpdateLoser", player);

                //We check if there is a player standing, if there is only one, the winner is set
                CheckLastPlayerStanding();
            }
        }

        [PunRPC]
        public void UpdateLoser()
        {
            loseText.SetActive(true);
        }

        [PunRPC]
        public void UpdateWinner()
        {
            winText.SetActive(true);
        }

        private void SetStartRequirements()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CheckRoomStatus();
            }
            else
            {
                waitingForHostText.SetActive(true);
            }
        }

        private void CheckRoomStatus()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    Debug.Log("Reached max players: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString());
                    MasterStartButton.gameObject.SetActive(true);
                    waitingForPlayersText.SetActive(false);
                }
                else
                {
                    waitingForPlayersText.SetActive(true);
                }
            }
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            if (PhotonNetwork.IsMasterClient)
            {
                CheckRoomStatus();
            }
        }

        private void CheckLastPlayerStanding()
        {
            playerList = FindObjectsOfType<Player.Player>().ToList();
            Debug.Log("Checking winner");
            if (playerList.Count == 1)
            {
                Debug.Log("Setting winner, we have one player left");
                SetWinner(playerList[0]);
            }
        }

        public void StartGame()
        {
            _photonView.RPC("GameStarted", RpcTarget.All);
        }

        private IEnumerator WaitToStart()
        {
            startCountdownText.gameObject.SetActive(true);
        
            startCountdownText.text = "Starting In: 3";
            yield return new WaitForSeconds(1f);
        
            startCountdownText.text = "Starting In: 2";
            yield return new WaitForSeconds(1f);
        
            startCountdownText.text = "Starting In: 1";
            yield return new WaitForSeconds(1f);
        
            startCountdownText.gameObject.SetActive(false);
            IsGameStarted = true;
        
            playerList = FindObjectsOfType<Player.Player>().ToList();
        }

        [PunRPC]
        public void GameStarted()
        {
            if (PhotonNetwork.IsMasterClient)
                MasterStartButton.gameObject.SetActive(false);

            else
                waitingForHostText.SetActive(false);

            StartCoroutine(WaitToStart());
        }

        private int minutes, seconds;

        private void ManageTime()
        {
            if (!IsGameStarted) return;
            
            if (PhotonNetwork.IsMasterClient == false) return;

            currentTime += Time.deltaTime;
            minutes = (int)(currentTime / 60f);
            seconds = (int)(currentTime - minutes * 60f);

            _photonView.RPC("UpdateTime", RpcTarget.All, minutes, seconds);
        }

        [PunRPC]
        public void UpdateTime(int minutes, int seconds)
        {
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}