using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour, Quanter, SuperQuanter
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
		public AudioSource pulse;
		public AudioSource bassSwap;
		public AudioSource pulseSwap;
		public AudioSource teleport;
		private AudioMixer mixer;
		private int rayMask = 0;
		private BeatMatcher beat;
		private bool loopedBeat;
		private bool loopedQuant;
		private bool loopedActualQuant;
		private Queue<Direction> inputBuffer;
		private AudioClip[] melodies;
		private AudioClip[] notes;
		private AudioClip[] pulses;
		private AudioClip[] arpeggios;
		public Note activeNote;
		private Dictionary<Note, AudioClip[]> noteToMelody;
		private Dictionary<Note, AudioClip[]> noteToDashMelody;
		private Dictionary<Note, AudioClip[]> shootingSequence;
		private AudioClip[] dashes;
		private int gracePeriodMilliseconds = 1000;
		private int gracePeriodSamples;
		private int teleportCharges = 1;
		private int maxCharges = 1;

		private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
			mixer = bass.outputAudioMixerGroup.audioMixer;
			rayMask |= 1 << LayerMask.NameToLayer ("Destructible");
			beat = FindObjectOfType<BeatMatcher> ();
			inputBuffer = new Queue<Direction> ();
			melodies = Resources.LoadAll<AudioClip>("Audio/Teleports");
			notes = Resources.LoadAll<AudioClip>("Audio/PlatformNotes/Bass");
			pulses = Resources.LoadAll<AudioClip>("Audio/PlatformNotes/Pulse");
			dashes = Resources.LoadAll<AudioClip>("Audio/Dashes");
			arpeggios = Resources.LoadAll<AudioClip> ("Audio/Shooting/Arpeggios");
			noteToMelody = new Dictionary<Note, AudioClip[]> ();
			noteToMelody.Add (Note.i, buildMelodyClipArray (melodies, 2, 3, 6, 7, 9));
			noteToMelody.Add (Note.III, buildMelodyClipArray (melodies, 6, 7, 9, 1));
			noteToMelody.Add (Note.iv, buildMelodyClipArray (melodies, 8,0,2,3));
			noteToMelody.Add (Note.v, buildMelodyClipArray (melodies, 9,1,4,5));
			noteToMelody.Add (Note.VI, buildMelodyClipArray (melodies, 0,2,3,6,7));
			noteToMelody.Add (Note.VII, buildMelodyClipArray (melodies, 1,4,5,8));
			noteToDashMelody = new Dictionary<Note, AudioClip[]> ();
			noteToDashMelody.Add (Note.i, buildMelodyClipArray (dashes, 2, 3, 6, 7, 9));
			noteToDashMelody.Add (Note.III, buildMelodyClipArray (dashes, 6, 7, 9, 1));
			noteToDashMelody.Add (Note.iv, buildMelodyClipArray (dashes,8,0,2,3));
			noteToDashMelody.Add (Note.v, buildMelodyClipArray (dashes,9,1,4,5));
			noteToDashMelody.Add (Note.VI, buildMelodyClipArray (dashes,0,2,3,6,7));
			noteToDashMelody.Add (Note.VII, buildMelodyClipArray (dashes,1,4,5,8));
			gracePeriodSamples = gracePeriodMilliseconds * 48;
			shootingSequence = new Dictionary<Note, AudioClip[]> ();
			shootingSequence.Add (Note.i, buildMelodyClipArray (arpeggios, 2, 5,6));
			shootingSequence.Add (Note.III, buildMelodyClipArray (arpeggios, 4, 6,1));
			shootingSequence.Add (Note.iv, buildMelodyClipArray (arpeggios, 2,5,0));
			shootingSequence.Add (Note.v, buildMelodyClipArray (arpeggios, 3,6,1));
			shootingSequence.Add (Note.VI, buildMelodyClipArray (arpeggios, 2,4,0));
			shootingSequence.Add (Note.VII, buildMelodyClipArray (arpeggios, 3, 5, 1));	
		}
		
		private void Start(){
			beat.registerQuant (this);
			beat.registerActualQuant (this);
			activeNote = Note.v;
			setClip (Note.i);
			setClip (Note.i);
		}

		private AudioClip[] buildMelodyClipArray(AudioClip[] clips, params int[] indexes){
			AudioClip[] melodyClips = new AudioClip[indexes.Length];
			for (int i = 0; i < indexes.Length; i++) {
				melodyClips[i] = clips[indexes[i]];
			}
			return melodyClips;
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
		
		IEnumerator fadeOut(AudioMixer mixer, AudioSource source){
			source.mute = false;
			float volume = 0f;
			while (volume > -20f) {
				volume -= 1f;
				mixer.SetFloat("swapvolume", volume);
				yield return null;
			}
			source.mute = true;
		}

		public void setClip(Note note){
			if (note != activeNote) {
				activeNote = note;
				bassSwap.clip = bass.clip;
				pulseSwap.clip = pulse.clip;
				StartCoroutine (fadeOut (bassSwap.outputAudioMixerGroup.audioMixer, bassSwap));
				StartCoroutine (fadeOut (pulseSwap.outputAudioMixerGroup.audioMixer, pulseSwap));
				bass.clip = notes [(int)note];
				pulse.clip = pulses [(int)note];
				StartCoroutine (fadeIn (bass.outputAudioMixerGroup.audioMixer));
				StartCoroutine (fadeIn (pulse.outputAudioMixerGroup.audioMixer));
				bass.Play ();
				pulse.Play ();
				bassSwap.Play ();
				pulseSwap.Play ();
				bass.timeSamples = drums.timeSamples;
				pulse.timeSamples = drums.timeSamples;
				bassSwap.timeSamples = drums.timeSamples;
				pulseSwap.timeSamples = drums.timeSamples;
			}
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

			if ((Mathf.Min(drums.timeSamples % 6000, Mathf.Abs((drums.timeSamples % 6000) - 6000)) < 2000) && !loopedQuant) {
				loopedActualQuant = true;
				beat.ReportQuant();
			}
			if (drums.timeSamples % 6000 > 3000 && Mathf.Abs((drums.timeSamples % 6000) - 6000) > 4000) {
				loopedActualQuant = false;
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
				if (direction == Direction.UP) {
					transform.position = transform.position + Vector3.up * (4);
					m_Rigidbody2D.velocity = Vector2.zero;
				} 
				if (direction == Direction.DOWN) {
					transform.position = transform.position + Vector3.down * (4);
					m_Rigidbody2D.velocity = Vector2.zero;
				}
				teleport.PlayOneShot (noteToMelody [activeNote] [UnityEngine.Random.Range (0, noteToMelody [activeNote].Length)], teleport.volume);
				teleportCharges--;
			}
		}

		private void Dash(Direction direction){
			if (direction == Direction.LEFT) {
				transform.position = transform.position + Vector3.left * (3);
				CheckForDestructibles();
			} 
			if (direction == Direction.RIGHT) {
				transform.position = transform.position + Vector3.right * (3);
				CheckForDestructibles();
			}
			//teleport.PlayOneShot (dashes[UnityEngine.Random.Range(0,dashes.Length)], teleport.volume);
			teleport.PlayOneShot (noteToDashMelody[activeNote] [UnityEngine.Random.Range (0, noteToDashMelody[activeNote].Length)], teleport.volume);
		}
		
		public void Act(){
			if (inputBuffer.Count > 0) {
				Direction direction = inputBuffer.Dequeue ();
				if (direction != null) {
					if(direction == Direction.UP || direction == Direction.DOWN){
						Teleport(direction);
					} else {
						Dash (direction);
					}
				}
			}
		}

		public void Quant(){
			//teleport.PlayOneShot (shootingSequence [activeNote] [UnityEngine.Random.Range (0, shootingSequence[activeNote].Length)], teleport.volume);
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
				inputBuffer.Enqueue(Direction.UP);
			} 
			if (direction < 0) {
				inputBuffer.Enqueue(Direction.DOWN);
			}
//			if ((Mathf.Min (drums.timeSamples % 12000, Mathf.Abs ((drums.timeSamples % 12000) - 12000)) < gracePeriodSamples)) {
//				print ("Grace!" + (drums.timeSamples % 12000));
//				Act ();
//			}
		}

		public void Dash(float direction){
			if (direction > 0) {
				inputBuffer.Enqueue(Direction.LEFT);
			} 
			if (direction < 0) {
				inputBuffer.Enqueue(Direction.RIGHT);
			}
//			if ((Mathf.Min(drums.timeSamples % 12000, Mathf.Abs((drums.timeSamples % 12000) - 12000)) < gracePeriodSamples)) {
//				print ("Grace!" + (drums.timeSamples % 12000));
//				Act();
//			}
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
				mixer.SetFloat ("highpass", 1000f); // Placeholder, sounds cool though
			} else {
				mixer.SetFloat ("highpass", 0f);
			}

			drums.volume = Mathf.Abs (move / 4);
			bass.volume = Math.Abs (move) + 0.7f;
			pulse.volume = Math.Abs (move) + 0.2f;
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
