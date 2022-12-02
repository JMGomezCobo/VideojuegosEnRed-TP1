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
        [SerializeField] private MMFeedbacks _shieldFeedbacks;
        
        private Vector3 moveDirection;
        private float normalShootCooldown;
        private float shotgunProjectileCD;
        private float dashCooldown;
        private float shieldCooldown;

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
            UIManager.UpdateSkillUI(normalShootCooldown, shotgunProjectileCD, dashCooldown, shieldCooldown);
        }

        private void HandleMovement()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput   = Input.GetAxis("Vertical");
            
            moveDirection = new Vector3(horizontalInput, verticalInput, 0);
            transform.position += moveDirection * (Time.deltaTime * PlayerStats.Speed);
            
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
            ShotgunProjectile();
            HandleDashSkill();
            HandleShieldSkill();
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

        private void ShotgunProjectile()
        {
            if (shotgunProjectileCD < 0f)
            {
                UIManager.IsSkillOnCD("Shotgun", false);

                if (Input.GetKey(KeyCode.Mouse1))
                {
                    Vector3 shootDirection = GetShootDirection();
                    float spreadAngle = 0;

                    for (int i = 0; i <= 2; i++)
                    {
                        GameObject shotProjectile = PhotonNetwork.Instantiate("PlayerProjectile", Emitter.position, Quaternion.identity);
                        Rigidbody2D projectileRb = shotProjectile.GetComponent<Rigidbody2D>();

                        switch (i)
                        {
                            case 0:
                                spreadAngle = 15f;
                                break;
                            case 1:
                                spreadAngle = 0f;
                                break;
                            case 2:
                                spreadAngle = -15f;
                                break;
                        }

                        float rotateAngle = spreadAngle + (Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg);
                        Vector2 MovementDirection = new Vector2(Mathf.Cos(rotateAngle * Mathf.Deg2Rad), Mathf.Sin(rotateAngle * Mathf.Deg2Rad)).normalized;
                        projectileRb.velocity = MovementDirection * (PlayerStats.ProjectileSpeed * 2f);

                        StartCoroutine(DestroyObject(0.3f, shotProjectile));
                    }
                    UIManager.IsSkillOnCD("Shotgun", true);
                    shotgunProjectileCD = PlayerStats.ShotgunAttackSpeed;
                }
            }
            shotgunProjectileCD -= Time.deltaTime;
        }

        private void HandleDashSkill()
        {
            if(dashCooldown <= 0)
            {
                UIManager.IsSkillOnCD("Blink", false);

                Vector3 blinkTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    var currentPosition = transform.position;
                    GameObject blink = PhotonNetwork.Instantiate("BlinkEffect", currentPosition, Quaternion.identity);
                    
                    _dashFeedbacks.PlayFeedbacks();
                    
                    StartCoroutine(DestroyObject(1.5f, blink));

                    currentPosition = Vector3.MoveTowards(currentPosition, blinkTo, PlayerStats.BlinkMaxDistance);
                    currentPosition = new Vector3(currentPosition.x, currentPosition.y, 0f);
                    transform.position = currentPosition;

                    GameObject blink2 = PhotonNetwork.Instantiate("BlinkEffect", currentPosition, Quaternion.identity);
                    StartCoroutine(DestroyObject(1.5f, blink2));

                    dashCooldown = PlayerStats.BlinkSpeed;
                    UIManager.IsSkillOnCD("Blink", true);
                }
            }
            dashCooldown -= Time.deltaTime;
        }

        private void HandleShieldSkill()
        {
            if(shieldCooldown <= 0f)
            {
                UIManager.IsSkillOnCD("Shield", false);

                if (Input.GetKey(KeyCode.E))
                {
                    _shieldFeedbacks.PlayFeedbacks();
                    
                    StartCoroutine(SetShield());
                    shieldCooldown = PlayerStats.ShieldSpeed;
                    UIManager.IsSkillOnCD("Shield", true);

                }
            }
            shieldCooldown -= Time.deltaTime;
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