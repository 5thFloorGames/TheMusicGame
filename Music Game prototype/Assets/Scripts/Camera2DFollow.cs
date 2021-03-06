using System;
using UnityEngine;

public class Camera2DFollow : MonoBehaviour
{
    public Transform target;
    public float damping = 1;
    public float lookAheadFactor = 3;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;

    private float m_OffsetZ;
    private Vector3 m_LastTargetPosition;
    private Vector3 m_CurrentVelocity;
    private Vector3 m_LookAheadPos;
	private bool zoomOut = false;

    // Use this for initialization
    private void Start()
    {
        m_LastTargetPosition = target.position;
        m_OffsetZ = (transform.position - target.position).z;
        transform.parent = null;
		transform.position = new Vector3(transform.position.x, transform.position.y, -400);
    }


    // Update is called once per frame
    private void Update()
    {
		if (zoomOut) {
			if(transform.position.z > -600){
				transform.position += Vector3.back * Time.deltaTime * 40;
			}
			transform.position += Vector3.left * 3 * Time.deltaTime * 40;
			transform.position += Vector3.up / 2f * Time.deltaTime * 40;
			if(transform.position.x < -1250f){
				FindObjectOfType<LevelGenerator>().EnableTutorial();
				Application.LoadLevel("BackgroundAssets");
			}
		} else {
			// only update lookahead pos if accelerating or changed direction
			float xMoveDelta = Mathf.Abs((target.position - m_LastTargetPosition).x);

			bool updateLookAheadTarget = Mathf.Abs (xMoveDelta) > lookAheadMoveThreshold;

			if (updateLookAheadTarget) {
				m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign (xMoveDelta);
			} else {
				m_LookAheadPos = Vector3.MoveTowards (m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
			}

			Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
			Vector3 newPos = Vector3.SmoothDamp (transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

			transform.position = newPos;

			m_LastTargetPosition = target.position;
		}
    }

	public void ZoomOut(){
		zoomOut = true;
	}
}
