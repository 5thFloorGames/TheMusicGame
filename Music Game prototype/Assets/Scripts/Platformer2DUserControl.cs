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

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
			matcher = FindObjectOfType<BeatMatcher> ();
        }


        private void Update()
        {
            if (!teleUp)
            {
                // Read the jump input in Update so button presses aren't missed.

				teleUp = CrossPlatformInputManager.GetButtonDown("Teleport");
				dash = CrossPlatformInputManager.GetButtonDown("Dash");
            }
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
            m_Character.Move(h, crouch, m_Jump);

			if (teleUp) {
				m_Character.Teleport (CrossPlatformInputManager.GetAxis("Teleport"));
			}
			if (dash) {
				m_Character.Dash (CrossPlatformInputManager.GetAxis("Dash"));
			}

            teleUp = false;
        }
    }
}
