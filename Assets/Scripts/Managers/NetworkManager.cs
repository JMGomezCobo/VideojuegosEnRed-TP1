using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        [Header("Input Field Bindings")]
        [Tooltip("Drag here the PlayerName input field.")]
        [SerializeField] private TMP_InputField _playerName;
        
        [Tooltip("Drag here the RoomName input field.")]
        [SerializeField] private TMP_InputField _roomName;
        
        [Tooltip("Drag here the MaxPlayers input field.")]
        [SerializeField] private TMP_InputField _maxPlayers;
        
        [Header("Button Bindings")]
        [Tooltip("Drag here the ConnectButton.")]
        public Button _connectButton;
        
        [Header("Text Bindings")]
        [Tooltip("Drag here the ConnectionStatus text.")]
        [SerializeField] private TextMeshProUGUI _connectionStatus;

        [Header("Scenes Bindings")] 
        [Tooltip("Type here the name of the scene to be loaded.")] [SerializeField]
        private string _sceneName = "MainLevel";

        private void Start()
        {
            _playerName.text = "NoobMaster69";
            _roomName.text   = "Room420";
            _maxPlayers.text = 2.ToString();
            
            PhotonNetwork.ConnectUsingSettings();
        
            _connectButton.interactable = false;
            _connectionStatus.text = "Connecting to master server.";
        }

        #region • Photon methods (9)

        public void Connect()
        {
            if (string.IsNullOrEmpty(_roomName.text)   || string.IsNullOrWhiteSpace(_roomName.text))   return;
            if (string.IsNullOrEmpty(_playerName.text) || string.IsNullOrWhiteSpace(_playerName.text)) return;
            if (string.IsNullOrEmpty(_maxPlayers.text) || string.IsNullOrWhiteSpace(_maxPlayers.text)) return;
        
            PhotonNetwork.NickName = _playerName.text;
        
            RoomOptions options = new RoomOptions {MaxPlayers = byte.Parse(_maxPlayers.text)};
        
            PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);

            _connectButton.interactable = false;
        }

        public override void OnConnectedToMaster()
        {
            _connectButton.interactable = false;
            PhotonNetwork.JoinLobby();
        
            _connectionStatus.text = "Connecting to lobby.";
        }

        public override void OnJoinedLobby()
        {
            _connectButton.interactable = true;
            _connectionStatus.text = "Connected to lobby.";
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _connectionStatus.text = "Failed to connect to master server. Cause: " + cause;
        }

        public override void OnLeftLobby()
        {
            _connectionStatus.text = "Lobby failed.";
        }

        public override void OnCreatedRoom()
        {
            _connectionStatus.text = "Created room.";
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            _connectionStatus.text = "Created room failed.";
            _connectButton.interactable = true;
        }

        public override void OnJoinedRoom()
        {
            _connectionStatus.text = "Joined room " + PhotonNetwork.CurrentRoom.Name;
            PhotonNetwork.LoadLevel(_sceneName);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            _connectionStatus.text = "Failed to join room " + PhotonNetwork.CurrentRoom.Name;
            _connectButton.interactable = true;
        }

        #endregion
    }
}