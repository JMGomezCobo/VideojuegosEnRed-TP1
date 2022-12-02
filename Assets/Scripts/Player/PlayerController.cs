using System.Collections;
using MoreMountains.Feedbacks;
using Photon.Pun;
using Skills;
using UnityEngine;

namespace Player
{
    public class PlayerController : Player
    {
        [Header("Player Feedbacks")]
        [SerializeField] private MMFeedbacks _shootFeedbacks;
        [SerializeField] private MMFeedbacks _dashFeedbacks;
        
        private Vector3 moveDirection;
        private float normalShootCooldown;
        private float dashCooldown;
        private float dashMult = 1f;

        private Vector3 position;
        private float angle;
        [SerializeField] private float emitterDistance = 5f;

        private float shieldLength = 1f;

        public override void Start()
        {
            base.Start();
            normalShootCooldown = 0f;
        }

        public override void Update()
        {
            if (!photonView.IsMine) return;
            if (!GameManager.IsGameStarted) return;
            
            HandleMovement();
            UpdateEmitterPosition();
            Skills();
            UIManager.UpdateSkillUI(normalShootCooldown, dashCooldown);
        }

        private void HandleMovement()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput   = Input.GetAxis("Vertical");
            
            moveDirection = new Vector3(horizontalInput, verticalInput, 0);
            transform.position += moveDirection * (Time.deltaTime * PlayerStats.Speed * dashMult);
            
            SetAnimation(horizontalInput,verticalInput);
        }

        private void UpdateEmitterPosition()
        {
            position = Input.mousePosition;
            position.z = (transform.position.z - Camera.main.transform.position.z);
            position = Camera.main.ScreenToWorldPoint(position);
            position = position - transform.position;
            angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
            if (angle < 0.0f) angle += 360.0f;
            Emitter.transform.localEulerAngles = new Vector3(0, 0, angle);
            float xPos = Mathf.Cos(Mathf.Deg2Rad * angle) * emitterDistance;
            float yPos = Mathf.Sin(Mathf.Deg2Rad * angle) * emitterDistance;
            Emitter.transform.position = new Vector3(transform.position.x + xPos, transform.position.y + yPos, 0);

            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Emitter.transform.LookAt(new Vector3(0, 0, mousepos.z));
        }

        private void Skills()
        {
            NormalProjectile();
            HandleDashSkill();
        }

        private void NormalProjectile()
        {
            if (normalShootCooldown < 0)
            {
                UIManager.IsSkillOnCD("Projectile", false);

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    _shootFeedbacks.PlayFeedbacks();
                    
                    Vector3 shootDirection = GetShootDirection();

                    GameObject shotProjectile = PhotonNetwork.Instantiate("PlayerProjectile", Emitter.position, Quaternion.identity);
                    Rigidbody2D projectileRb = shotProjectile.GetComponent<Rigidbody2D>();

                    projectileRb.velocity = new Vector2(shootDirection.x, shootDirection.y).normalized * PlayerStats.ProjectileSpeed;
                    normalShootCooldown = PlayerStats.AttackSpeed;

                    StartCoroutine(DestroyObject(5f, shotProjectile));
                    UIManager.IsSkillOnCD("Projectile", true);
                }
            }
            
            normalShootCooldown -= Time.deltaTime;
        }

        private void HandleDashSkill()
        {
            if(dashCooldown <= 0)
            {
                UIManager.IsSkillOnCD("Dash", false);


                //Vector3 blinkTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    dashMult = PlayerStats.DashSpeed;
                    StartCoroutine(RevertDash());
                    _dashFeedbacks.PlayFeedbacks();

                    //    GameObject blink = PhotonNetwork.Instantiate("BlinkEffect", currentPosition, Quaternion.identity);
                    //    StartCoroutine(DestroyObject(1.5f, blink));

                    dashCooldown = PlayerStats.DashSpeed;
                    UIManager.IsSkillOnCD("Dash", true);
                }
            }
            dashCooldown -= Time.deltaTime;
        }

        IEnumerator RevertDash()
        {
            yield return new WaitForSeconds(1f);
            dashMult = 1f;
        }

        private void SetAnimation(float x, float y)
        {
            UpdateAnimations("HSpeed", x);
            UpdateAnimations("VSpeed", y);

        }

        private void UpdateAnimations(string animationName, float movementDir)
        {
            Animator.SetFloat(animationName, movementDir);
        } 

        private Vector3 GetShootDirection()
        {
            var shootDirection = Input.mousePosition;
            
            shootDirection.z = 0.0f;
            shootDirection = Camera.main.ScreenToWorldPoint(shootDirection);
            shootDirection -= transform.position;
            
            return shootDirection;
        }

        private static IEnumerator DestroyObject(float secondsToDestroy, GameObject projectile)
        {
            yield return new WaitForSeconds(secondsToDestroy);
            
            if(projectile != null)
            {
                PhotonNetwork.Destroy(projectile);
            }
        }

        private IEnumerator SetShield()
        {
            PV.RPC("SetShield", RpcTarget.All, true);
            GameObject shield = PhotonNetwork.Instantiate("Shield", transform.position, Quaternion.identity);
            shield.GetComponent<Shield>().Player = gameObject;
            yield return new WaitForSeconds(shieldLength);
            PV.RPC("SetShield", RpcTarget.All, false);
            StartCoroutine(DestroyObject(0f, shield));
        }
    }
}