using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class SwipeControls : MonoBehaviour {
	private PlatformerCharacter2D m_Character;
	private bool teleUp;
	private float teleDirection = 0f;
	private bool dash;
	private BeatMatcher matcher;
	private bool autoRun = true;
	private bool startHold = true;
	public Transform player;  // Drag your player here
	private Vector2 fp;  // first finger position
	private Vector2 lp ;  // last finger position
	
	private void Awake()
	{
		m_Character = GetComponent<PlatformerCharacter2D>();
		matcher = FindObjectOfType<BeatMatcher> ();
	}
	
	void Update() {
		if (!startHold) {
			foreach (Touch touch in Input.touches) {
				if (touch.phase == TouchPhase.Began) {
					fp = touch.position;
					lp = touch.position;
				}
				if (touch.phase == TouchPhase.Moved) {
					lp = touch.position;
				}
				if (touch.phase == TouchPhase.Ended) { 
					if(!teleUp) {
						if ((fp.y - lp.y) > 80) {
								teleUp = true;
								teleDirection = -1f;
							} else if ((fp.y - lp.y) < -80) {
								teleUp = true;
								teleDirection = 1f;							}
						}
					}
				if (!dash) {
					if ((fp.x - lp.x) < -80) {
						dash = true;
					} 
				}
			}
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

		if (!startHold) {
			// Pass all parameters to the character control script.
			if (autoRun) {
				m_Character.Move (1f);
			} else {
				m_Character.Move (0f);
			}
			
			if (teleUp) {
				m_Character.Teleport (teleDirection);
			}
			if (dash) {
				m_Character.Dash (1f);
			}
		} else {
			m_Character.Move (0f);
		}
		
		if (Input.touches.Length > 0 && startHold) {
			startHold = false;
		}
		
		teleUp = false;
		dash = false;
	}
}	