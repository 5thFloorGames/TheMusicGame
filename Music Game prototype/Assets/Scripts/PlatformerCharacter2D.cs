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
		public AudioSource drums;
		public AudioSource bass;
		public AudioSource bassSwap;
		public AudioSource teleport;
		public AudioSource melody;
		private AudioMixer mixer;
		private int rayMask = 0;
		private BeatMatcher beat;
		private bool loopedBeat;
		private bool loopedQuant;
		private Queue<Direction> inputBuffer;
		private AudioClip[] clips;

		private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
			mixer = bass.outputAudioMixerGroup.audioMixer;
			rayMask |= 1 << LayerMask.NameToLayer ("Platform");
			beat = FindObjectOfType<BeatMatcher> ();
			inputBuffer = new Queue<Direction> ();
			clips = Resources.LoadAll<AudioClip>("Audio/Melodies");
		}

		private void Start(){
			beat.registerQuant (this);
		}


		// 0.0625 seconds in length
		IEnumerator fadeIn(AudioMixer source){
			float volume = -20f;
			while (volume < 0f) {
				volume += 1f;
				source.SetFloat("bassvolume", volume);
				yield return null;
			}
		}
		
		IEnumerator fadeOut(AudioMixer source){
			bassSwap.mute = false;
			float volume = 0f;
			while (volume > -20f) {
				volume -= 1f;
				source.SetFloat("swapvolume", volume);
				yield return null;
			}
			bassSwap.mute = true;
		}

		public void setClip(AudioClip clip){
			bassSwap.clip = bass.clip;
			StartCoroutine(fadeOut (bassSwap.outputAudioMixerGroup.audioMixer));
			bass.clip = clip;
			StartCoroutine(fadeIn (bass.outputAudioMixerGroup.audioMixer));
			bass.Play ();
			bassSwap.Play ();
			bass.timeSamples = drums.timeSamples;
			bassSwap.timeSamples = drums.timeSamples;
		}

		private void Update(){
			if ((Mathf.Min(drums.timeSamples % 24000, Mathf.Abs((drums.timeSamples % 24000) - 24000)) < 2000) && !loopedBeat) {
				loopedBeat = true;
				beat.ReportBeat();
			}
			if (drums.timeSamples % 24000 > 12000 && Mathf.Abs((drums.timeSamples % 24000) - 24000) > 4000) {
				loopedBeat = false;
			}

			if ((Mathf.Min(drums.timeSamples % 12000, Mathf.Abs((drums.timeSamples % 12000) - 12000)) < 2000) && !loopedQuant) {
				loopedQuant = true;
				beat.ReportDoubleQuant();
			}
			if (drums.timeSamples % 12000 > 6000 && Mathf.Abs((drums.timeSamples % 12000) - 12000) > 4000) {
				loopedQuant = false;
			}
		}

        private void FixedUpdate()
        {
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }

		public void Act(){
			if (inputBuffer.Count > 0) {
				Direction direction = inputBuffer.Dequeue ();
				if (direction != null) {
					if (direction == Direction.UP) {
						transform.position = transform.position + Vector3.up * (4);
						m_Rigidbody2D.velocity = Vector2.zero;
					} 
					if (direction == Direction.DOWN) {
						transform.position = transform.position + Vector3.down * (4);
						m_Rigidbody2D.velocity = Vector2.zero;
					}
					if (direction == Direction.LEFT) {
						transform.position = transform.position + Vector3.left * (3);
					} 
					if (direction == Direction.RIGHT) {
						transform.position = transform.position + Vector3.right * (3);
					}
					melody.PlayOneShot (clips [UnityEngine.Random.Range (0, clips.Length)]);
				}
			}
		}

		public void Teleport(float direction){
			if (direction > 0) {
				inputBuffer.Enqueue(Direction.UP);
			} 
			if (direction < 0) {
				inputBuffer.Enqueue(Direction.DOWN);
			}
		}

		public void Dash(float direction){
			if (direction > 0) {
				inputBuffer.Enqueue(Direction.LEFT);
			} 
			if (direction < 0) {
				inputBuffer.Enqueue(Direction.RIGHT);
			}
		}

        public void Move(float move, bool crouch, bool jump)
        {
            // If crouching, check to see if the character can stand up
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

			if (!m_Grounded) {

				RaycastHit hit;
				Debug.DrawRay (transform.position, Vector3.down);
				Physics.Raycast (transform.position, Vector3.down, out hit, ~rayMask);
				mixer.SetFloat ("highpass", 1000f); // Placeholder, sounds cool though
			} else {
				mixer.SetFloat ("highpass", 0f);
			}

			drums.volume = Mathf.Abs (move / 4);
			bass.volume = Math.Abs (move) + 0.7f;
			mixer.SetFloat ("lowpass",(Mathf.Abs (move) * 19500f) + 2500f);

            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*m_CrouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);

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
            // If the player should jump...
            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
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
    }
}
