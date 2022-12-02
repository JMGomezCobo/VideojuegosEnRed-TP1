using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _currentPlayersText;
        [SerializeField] private TextMeshProUGUI _roomNameText;
        [SerializeField] private TextMeshProUGUI _pingText;
        
        [SerializeField] private GameObject projectileCD, dashCD;

        private TextMeshProUGUI dashCountdown;
        private TextMeshProUGUI projectileCountdown;

        private bool IsDashOnCooldown      { get; set; }
        private bool IsProjectileOnCoolDown { get; set; }

        private void Start()
        {
            UpdateUI();
            SetSkillCooldown();
        }
        private void Update()
        {
            UpdatePing();
        }
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            string roomName     = PhotonNetwork.CurrentRoom.Name;
            string maxPlayers   = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
            string playerCount = PhotonNetwork.CurrentRoom.PlayerCount.ToString();

            _roomNameText.text = "Room Name: " + roomName;
            _currentPlayersText.text = "Players: " + playerCount + "/" + maxPlayers;
        }

        private void UpdatePing()
        {
            _pingText.text = "Ping: " + PhotonNetwork.GetPing().ToString();
        }

        private void SetSkillCooldown()
        {
            dashCountdown      = dashCD.GetComponentInChildren<TextMeshProUGUI>();
            projectileCountdown = projectileCD.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void IsSkillOnCD(string skillName, bool cdCheck)
        {
            switch (skillName)
            {
                case "Projectile":
                    IsProjectileOnCoolDown = cdCheck;
                    break;
                
                case "Dash":
                    IsDashOnCooldown = cdCheck;
                    break;
            }
        }

        public void UpdateSkillUI(float projectileCooldown, float dashCooldown)
        {
            if(IsProjectileOnCoolDown)
            {
                projectileCD.SetActive(true);
                projectileCountdown.text = ChangeTimeDisplay(projectileCooldown);
            }
            
            else
            {
                projectileCD.SetActive(false);
            }
            
            if (IsDashOnCooldown)
            {
                dashCD.SetActive(true);
                dashCountdown.text = ChangeTimeDisplay(dashCooldown);
            }
            
            else
            {
                dashCD.SetActive(false);
            }
        }

        private static string ChangeTimeDisplay(float currentTime)
        {
            int minutes = (int)(currentTime / 60f);
            int seconds = (int)(currentTime - minutes * 60f);

            return $"{seconds:0}";
        }
    }
}