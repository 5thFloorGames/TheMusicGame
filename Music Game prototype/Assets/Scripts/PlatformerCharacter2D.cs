using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour, Quanter
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
		private int rayMask = 0;
		private int platformMask = 0;
		private BeatMatcher beat;
		private Queue<Direction> inputBuffer;
		private int teleportCharges = 1;
		private int maxCharges = 1;
		private CharacterMusicSystem musicSystem;
		public bool frozen = false;
		private float speed = 1f;
		private Coroutine boosting;
		private ScoreCounter scoreCounter;
		private bool muted = false;
		private GameObject teleportParticle;
		private GameObject dashParticle;
		private GameObject ringParticle;
		
		private void Awake()
        {
			beat = FindObjectOfType<BeatMatcher> ();
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
			rayMask |= 1 << LayerMask.NameToLayer ("Destructible");
			platformMask |= 1 << LayerMask.NameToLayer ("Platform");
			beat = FindObjectOfType<BeatMatcher> ();
			inputBuffer = new Queue<Direction> ();
			musicSystem = GetComponent<CharacterMusicSystem> ();
			scoreCounter = FindObjectOfType<ScoreCounter> ();
			teleportParticle = Resources.Load<GameObject> ("ParticleEffects/TeleportParticle");
			dashParticle = Resources.Load<GameObject>("ParticleEffects/DashParticle");
			ringParticle = Resources.Load<GameObject> ("ParticleEffects/RingParticle");
		}
		
		private void Start(){
			beat.registerQuant (this);
		}

		private void Update(){
//			if (Input.GetKeyDown(KeyCode.H)) {
//				print (transform.position);
//			}
		}


		// try with 100 FPS or something
        private void FixedUpdate()
        {
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject){
                    m_Grounded = true;
					teleportCharges = maxCharges;
				}
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }

		public void Cheat(){
			maxCharges = 1000;
		}

		private void Teleport(Direction direction){
			if (teleportCharges > 0) {
				Instantiate (teleportParticle, transform.position + Vector3.left * 4f, Quaternion.identity);
				if (direction == Direction.UP) {
					transform.position = transform.position + Vector3.up * (4);
					Teleported(direction);
					m_Rigidbody2D.velocity = Vector2.zero;
				} 
				if (direction == Direction.DOWN) {
					if(!m_Grounded){
						transform.position = transform.position + Vector3.down * (2);
						Teleported(direction);
					} else {
						RaycastHit2D hit = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y - 4f), Vector2.down, 10f, platformMask);
						if(hit){
							transform.position = transform.position + Vector3.down * (4);
							Teleported(direction);
							m_Rigidbody2D.velocity = Vector2.zero;
						}
					}
				}

			}
		}

		private void Teleported(Direction direction){
			musicSystem.TeleportSound(direction);
			teleportCharges--;
		}
		
		private void Dash(Direction direction){
			Instantiate (dashParticle, transform.position + Vector3.left * 4f, Quaternion.identity);
			if (direction == Direction.LEFT) {
				transform.position = transform.position + Vector3.left * (3);
				CheckForDestructibles();
			} 
			if (direction == Direction.RIGHT) {
				transform.position = transform.position + Vector3.right * (3);
				CheckForDestructibles();
			}
			musicSystem.DashSound ();
		}
		
		public void Act(){
			if (inputBuffer.Count > 0) {
				Direction direction = inputBuffer.Dequeue ();
				if (direction != null && !frozen) {
					if(direction == Direction.UP || direction == Direction.DOWN){
						Teleport(direction);
					} else {
						Dash (direction);
					}
				}
			}
		}

		public void CheckForDestructibles(){
			RaycastHit2D hit;
			Vector2 direction;
			if (m_FacingRight) {
				direction = Vector2.left;
			} else {
				direction = Vector2.right;
			}
			Debug.DrawRay (transform.position, direction,Color.green, 2f);
			hit = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y), direction, 2.5f, rayMask);
			if (hit) {
				if(hit.collider.tag == "Destructible"){
					hit.collider.SendMessage("DestroyOnQuant");
				}
			}
		}

		public void Teleport(float direction){
			if (direction > 0) {
				AddToBuffer(Direction.UP);
			} 
			if (direction < 0) {
				AddToBuffer(Direction.DOWN);
			}
		}

		public void Dash(float direction){
			if (direction > 0) {
				AddToBuffer(Direction.LEFT);
			} 
			if (direction < 0) {
				AddToBuffer(Direction.RIGHT);
			}
		}

		private void AddToBuffer(Direction direction){
			if (inputBuffer.Count < 2) {
				inputBuffer.Enqueue (direction);
			}
		}

        public void Move(float move)
        {
			if (muted) {
				musicSystem.MoveSounds (0, m_Grounded);
			} else {
				musicSystem.MoveSounds (1, m_Grounded);
			}

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
				if(!frozen){
                	m_Rigidbody2D.velocity = new Vector2(move * speed * m_MaxSpeed, m_Rigidbody2D.velocity.y);
				}

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
        }

		public void SpeedBoost(){
			scoreCounter.IncreaseScore ();
			if (boosting != null) {
				StopCoroutine(boosting);
			}
			GameObject newParticle = (GameObject)Instantiate (ringParticle, transform.position, Quaternion.identity);
			newParticle.transform.parent = transform;
			boosting = StartCoroutine (SpeedForSecond ());
		}

		IEnumerator SpeedForSecond(){
			speed += 0.4f;
			if (speed > 2.2f) {
				speed = 2.2f;
			}
			yield return new WaitForSeconds (2f);
			speed = 1.0f;
		}

        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

		public void MuteMovement(){
			muted = true;
		}

		public void UnmuteMovement(){
			muted = false;
		}
		
		public void Freeze(){
			scoreCounter.DecreaseScore ();
			frozen = true;
			teleportCharges = 0;
			m_Rigidbody2D.isKinematic = true;
		}

		public void Unfreeze(){
			frozen = false;
			UnmuteMovement ();
			m_Rigidbody2D.isKinematic = false;
		}
	}
}
