using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class GameManager : MonoBehaviourPun
    {
        [Header("Photon Bindings")]
        [SerializeField] private PhotonView _photonView;
        
        [Header("Text Bindings")]
        [SerializeField] private TextMeshProUGUI startCountdownText;
        [SerializeField] private TextMeshProUGUI timeText;
        
        [SerializeField] private GameObject loseText;
        [SerializeField] private GameObject winText;
        [SerializeField] private GameObject waitingForHostText;
        
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
            var player = character._PhotonView.Owner;
            _photonView.RPC("UpdateWinner", RpcTarget.All, player);
        }

        public void SetLoser(Player.Player character)
        {
            var player = character._PhotonView.Owner;
            _photonView.RPC("UpdateLoser", RpcTarget.All, player);

            playerList = FindObjectsOfType<Player.Player>().ToList();
            
            if (playerList.Count == 1) 
                SetWinner(playerList[0]);
        }

        [PunRPC]
        public void UpdateLoser(Photon.Realtime.Player client)
        {
            if (!Equals(PhotonNetwork.LocalPlayer, client)) return;
            
            loseText.SetActive(true);
        }

        [PunRPC]
        public void UpdateWinner(Photon.Realtime.Player client)
        {
            if (Equals(PhotonNetwork.LocalPlayer, client)) 
                winText.SetActive(true);
        }

        private void SetStartRequirements()
        {
            if (PhotonNetwork.IsMasterClient)
                MasterStartButton.gameObject.SetActive(true);

            else
                waitingForHostText.SetActive(true);
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