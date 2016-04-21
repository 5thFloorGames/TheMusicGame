using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;
		private bool teleUp;
		private bool dash;
		private BeatMatcher matcher;
		private bool autoRun = true;

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
			matcher = FindObjectOfType<BeatMatcher> ();
        }


        private void Update()
        {
            if (!teleUp)
            {
				teleUp = CrossPlatformInputManager.GetButtonDown("Teleport");
            }

			if (!dash) {
				dash = CrossPlatformInputManager.GetButtonDown("Dash");
			}
			if (CrossPlatformInputManager.GetButtonDown("Reset")) {
				Application.LoadLevel("BackgroundAssets");
			}
		}

		public void FreeControl(){
			autoRun = false;
		}

		public void AutoRun(){
			autoRun = true;
		}


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
			float keyboard = CrossPlatformInputManager.GetAxis ("Horizontal");
			float gamepad = CrossPlatformInputManager.GetAxis ("HorizontalGamepad");

			float h;
			if (Mathf.Abs (gamepad) > Mathf.Abs (keyboard)) {
				h = gamepad;
			} else {
				h = keyboard;
			}
            // Pass all parameters to the character control script.
			if (autoRun) {
				m_Character.Move (1, crouch, m_Jump);
			} else {
				m_Character.Move (h, crouch, m_Jump);
			}

			if (teleUp) {
				m_Character.Teleport (CrossPlatformInputManager.GetAxis("Teleport"));
			}
			if (dash) {
				m_Character.Dash (CrossPlatformInputManager.GetAxis("Dash"));
			}

			if (Input.GetButtonDown ("Cheat")) {
				m_Character.Cheat();
			}

            teleUp = false;
			dash = false;
        }
    }
}
