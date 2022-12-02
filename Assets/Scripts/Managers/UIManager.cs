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
        
        [SerializeField] private GameObject projectileCD, shotgunCD, blinkCD, shieldCD;

        private TextMeshProUGUI blinkCountdown;
        private TextMeshProUGUI projectileCountdown;
        private TextMeshProUGUI shieldCountdown;
        private TextMeshProUGUI shotgunCountdown;

        private bool IsBlinkOnCooldown      { get; set; }
        private bool IsProjectileOnCoolDown { get; set; }
        private bool IsShieldOnCooldown     { get; set; }
        private bool IsShotGunOnCooldown    { get; set; }

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
            blinkCountdown      = blinkCD.GetComponentInChildren<TextMeshProUGUI>();
            projectileCountdown = projectileCD.GetComponentInChildren<TextMeshProUGUI>();
            shieldCountdown     = shieldCD.GetComponentInChildren<TextMeshProUGUI>();
            shotgunCountdown    = shotgunCD.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void IsSkillOnCD(string skillName, bool cdCheck)
        {
            switch (skillName)
            {
                case "Projectile":
                    IsProjectileOnCoolDown = cdCheck;
                    break;
                
                case "Shotgun":
                    IsShotGunOnCooldown = cdCheck;
                    break;
                
                case "Blink":
                    IsBlinkOnCooldown = cdCheck;
                    break;
                
                case "Shield":
                    IsShieldOnCooldown = cdCheck;
                    break;
            }
        }

        public void UpdateSkillUI(float projectileCooldown, float shotgunCooldown, float blinkCooldown, float shieldCooldown)
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
            
            if (IsShotGunOnCooldown)
            {
                shotgunCD.SetActive(true);
                shotgunCountdown.text = ChangeTimeDisplay(shotgunCooldown);
            }
            
            else
            {
                shotgunCD.SetActive(false);
            }
            
            if (IsBlinkOnCooldown)
            {
                blinkCD.SetActive(true);
                blinkCountdown.text = ChangeTimeDisplay(blinkCooldown);
            }
            
            else
            {
                blinkCD.SetActive(false);
            }
            
            if (IsShieldOnCooldown)
            {
                shieldCD.SetActive(true);
                shieldCountdown.text = ChangeTimeDisplay(shieldCooldown);
            }
            
            else
            {
                shieldCD.SetActive(false);
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