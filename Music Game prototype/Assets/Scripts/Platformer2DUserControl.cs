using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
		private bool teleUp;
		private bool dash;
		private BeatMatcher matcher;
		private bool autoRun = true;
		private bool startHold = true;
		
		private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
			matcher = FindObjectOfType<BeatMatcher> ();
        }


        private void Update()
        {
			if (!startHold) {
				if (!teleUp) {
					teleUp = CrossPlatformInputManager.GetButtonDown ("Teleport");
				}

				if (!dash) {
					dash = CrossPlatformInputManager.GetButtonDown ("Dash");
				}
			}
			if (CrossPlatformInputManager.GetButtonDown("Reset")) {
				FindObjectOfType<LevelGenerator>().DisableTutorial();
				Application.LoadLevel("BackgroundAssets");
			}

			if (CrossPlatformInputManager.GetButtonDown ("SuperReset")) {
				FindObjectOfType<LevelGenerator>().EnableTutorial();
				Application.LoadLevel("BackgroundAssets");
			}

			if (Input.GetKeyDown (KeyCode.L)) {
				m_Character.gameObject.SendMessage("LevelUp");
			}
		}

		public void FreeControl(){
			autoRun = false;
		}

		public void AutoRun(){
			autoRun = true;
		}


        private void FixedUpdate() {
            // Read the inputs.
			float keyboard = CrossPlatformInputManager.GetAxis ("Horizontal");
			float gamepad = CrossPlatformInputManager.GetAxis ("HorizontalGamepad");

			float h;
			if (Mathf.Abs (gamepad) > Mathf.Abs (keyboard)) {
				h = gamepad;
			} else {
				h = keyboard;
			}

			if (!startHold) {
				// Pass all parameters to the character control script.
				if (autoRun) {
					m_Character.Move (1);
				} else {
					m_Character.Move (h);
				}

				if (teleUp) {
					m_Character.Teleport (CrossPlatformInputManager.GetAxis ("Teleport"));
				}
				if (dash) {
					m_Character.Dash (CrossPlatformInputManager.GetAxis ("Dash"));
				}
			} else {
				m_Character.Move (0);
			}

			if (Input.anyKeyDown && startHold) {
				startHold = false;
			}

            teleUp = false;
			dash = false;
        }
    }
}
