using System.Collections;
using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEngine;

namespace Player
{
    public class PlayerController : Player
    {
        [Header("Player Feedbacks")]
        [SerializeField] private MMFeedbacks _shootFeedbacks;
        [SerializeField] private MMFeedbacks _dashFeedbacks;
        
        private Vector3 _moveDirection;
        private Vector3 _position;

        private Camera _mainCamera;
        
        private float _normalShootCooldown;
        private float _dashCooldown;
        private float _dashMultiplier = 1f;
        private float _angle;

        [SerializeField] private float emitterDistance = 5f;
        
        public override void Start()
        {
            base.Start();
            
            _mainCamera = Camera.main;
            _normalShootCooldown = 0f;
        }

        public override void Update()
        {
            if (!photonView.IsMine) return;
            if (!GameManager.IsGameStarted) return;
            
            HandleMovement();
            UpdateEmitterPosition();
            HandleShoot();
            HandleDash();
            
            UIManager.UpdateSkillUI(_normalShootCooldown, _dashCooldown);
        }

        private void HandleMovement()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput   = Input.GetAxis("Vertical");
            
            _moveDirection = new Vector3(horizontalInput, verticalInput, 0);
            transform.position += _moveDirection * (Time.deltaTime * PlayerStats.Speed * _dashMultiplier);
            
            SetAnimation(horizontalInput,verticalInput);
        }

        private void UpdateEmitterPosition()
        {
            _position = Input.mousePosition;
            _position.z = (transform.position.z - _mainCamera.transform.position.z);
            _position = _mainCamera.ScreenToWorldPoint(_position);
            
            _position -= transform.position;
            
            _angle = Mathf.Atan2(_position.y, _position.x) * Mathf.Rad2Deg;
            
            if (_angle < 0.0f) _angle += 360.0f;
            
            Emitter.transform.localEulerAngles = new Vector3(0, 0, _angle);
            
            float positionX = Mathf.Cos(Mathf.Deg2Rad * _angle) * emitterDistance;
            float positionY = Mathf.Sin(Mathf.Deg2Rad * _angle) * emitterDistance;
            
            Emitter.transform.position = new Vector3(transform.position.x + positionX, transform.position.y + positionY, 0);

            Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Emitter.transform.LookAt(new Vector3(0, 0, mousePosition.z));
        }
        
        private void HandleShoot()
        {
            if (_normalShootCooldown < 0)
            {
                UIManager.IsSkillOnCD("Projectile", false);

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    _shootFeedbacks.PlayFeedbacks();
                    
                    Vector3 shootDirection = GetShootDirection();

                    GameObject shotProjectile = PhotonNetwork.Instantiate("PlayerProjectile", Emitter.position, Quaternion.identity);
                    Rigidbody2D projectileRb = shotProjectile.GetComponent<Rigidbody2D>();

                    projectileRb.velocity = new Vector2(shootDirection.x, shootDirection.y).normalized * PlayerStats.ProjectileSpeed;
                    _normalShootCooldown = PlayerStats.AttackSpeed;

                    StartCoroutine(DestroyObject(5f, shotProjectile));
                    UIManager.IsSkillOnCD("Projectile", true);
                }
            }
            
            _normalShootCooldown -= Time.deltaTime;
        }

        private void HandleDash()
        {
            if(_dashCooldown <= 0)
            {
                UIManager.IsSkillOnCD("Dash", false);
                
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _dashMultiplier = PlayerStats.DashSpeed;
                    StartCoroutine(RevertDash());
                    _dashFeedbacks.PlayFeedbacks();

                    _dashCooldown = PlayerStats.DashSpeed;
                    UIManager.IsSkillOnCD("Dash", true);
                }
            }
            _dashCooldown -= Time.deltaTime;
        }

        private IEnumerator RevertDash()
        {
            yield return new WaitForSeconds(1f);
            _dashMultiplier = 1f;
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
            shootDirection = _mainCamera.ScreenToWorldPoint(shootDirection);
            shootDirection -= transform.position;
            
            return shootDirection;
        }

        private static IEnumerator DestroyObject(float secondsToDestroy, GameObject projectile)
        {
            yield return new WaitForSeconds(secondsToDestroy);
            
            if (projectile != null)
            {
                PhotonNetwork.Destroy(projectile);
            }
        }
    }
}