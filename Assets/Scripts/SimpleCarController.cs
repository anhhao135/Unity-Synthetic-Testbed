using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class SimpleCarController : MonoBehaviour {

	public Transform rootWaypoint;
	public Transform targetWaypoint;
	public int waypointCount;
	public int index;
	public Vector3 relative;
	public float distanceToNext;

	public float m_horizontalInput;
	public float m_verticalInput;
	public float m_steeringAngle;

	public WheelCollider frontDriverW, frontPassengerW;
	public WheelCollider rearDriverW, rearPassengerW;
	public Transform frontDriverT, frontPassengerT;
	public Transform rearDriverT, rearPassengerT;
	public float maxSteerAngle = 30;
	public float motorForce = 50;


	public PIDController accelPID;
	public PIDController steerPID;


	public bool keyboardControl = false;
	public float RayDistance;
	public float constantSpeed = 0f;
	public float frontColliderDistance;
	public float verticalInputMod;

	public RaycastHit frontHit;
	public RaycastHit leftHit;
	public RaycastHit rightHit;
	public RaycastHit leftsideHit;
	public RaycastHit rightsideHit;

	public LayerMask mask;

	public GameObject brakeLights;
	public GameObject leftBlinkers;
	public GameObject rightBlinkers;

	public void Start()
	{
		waypointCount = rootWaypoint.childCount;
		RayDistance = 4.5f;
		

	}

	public void Update()
	{


		brakeLights.SetActive(false);

		if (keyboardControl == false)
		{

			targetWaypoint = rootWaypoint.GetChild(index);

			string waypointTag = targetWaypoint.gameObject.tag;

			string[] attributes = waypointTag.Split(' ');



			if (attributes.Contains("LeftTurn") || attributes.Contains("RightTurn"))
				{


					
					accelPID.Kp = 1.5f;
					accelPID.Ki = 0.01f;
					accelPID.Kd = 0f;

					RayDistance = 5f;

					if (attributes.Contains("RightTurn"))
					{
						rightBlinkers.SetActive(true);
					}

					if (attributes.Contains("LeftTurn"))
					{
						leftBlinkers.SetActive(true);
					}

				}

			else
				{
					rightBlinkers.SetActive(false);
					leftBlinkers.SetActive(false);

					accelPID.Kp = 1.5f;
					accelPID.Ki = 0f;
					accelPID.Kd = 5f;

					RayDistance = 4.5f;

			}
		


			relative = transform.InverseTransformPoint(targetWaypoint.position);



			distanceToNext = Vector3.Magnitude((transform.position + transform.forward) - targetWaypoint.position);

			if (Vector3.Magnitude(transform.position - targetWaypoint.position) < 2.5f)
			{
				index++;
				accelPID.integral = 0;
				steerPID.integral = 0;

				if (index == waypointCount)
				{
					index = 0;
					accelPID.integral = 0;
					steerPID.integral = 0;
				}
			}
		}


		GetInput();
		Steer();
		Accelerate();
		UpdateWheelPoses();


		Physics.Raycast(1.5f * transform.forward + transform.position + transform.up, transform.forward, out frontHit, RayDistance, mask);
		Physics.Raycast(1.5f * transform.forward + transform.position + transform.up, transform.forward + transform.right * -0.4f, out leftHit, RayDistance, mask);
		Physics.Raycast(1.5f * transform.forward + transform.position + transform.up, transform.forward + transform.right * 0.4f, out rightHit, RayDistance, mask);

		Physics.Raycast(1.5f * transform.forward + transform.position + transform.up, -transform.right + 0.7f * transform.forward, out leftsideHit, RayDistance/3, mask);
		Physics.Raycast(1.5f * transform.forward + transform.position + transform.up, transform.right + 0.7f * transform.forward, out rightsideHit, RayDistance/3, mask);

		Debug.DrawRay(1.5f * transform.forward + transform.position + transform.up, transform.forward * RayDistance, Color.yellow);
		Debug.DrawRay(1.5f * transform.forward + transform.position + transform.up, (transform.forward + transform.right * -0.4f) * RayDistance, Color.yellow);
		Debug.DrawRay(1.5f * transform.forward + transform.position + transform.up, (transform.forward + transform.right * 0.4f) * RayDistance, Color.yellow);

		Debug.DrawRay(1.5f * transform.forward + transform.position + transform.up, (-transform.right + 0.7f * transform.forward) * RayDistance/3, Color.yellow);
		Debug.DrawRay(1.5f * transform.forward + transform.position + transform.up, (transform.right + 0.7f * transform.forward) * RayDistance/3, Color.yellow);




		if (frontHit.point != Vector3.zero || leftHit.point != Vector3.zero || rightHit.point != Vector3.zero || rightsideHit.point != new Vector3(0, 0, 0) || leftsideHit.point != new Vector3(0, 0, 0))
		{

			frontColliderDistance = Vector3.Magnitude(frontHit.point - (transform.position + transform.up));

		}

		else
		{
			frontColliderDistance = 0f;
		}

		if (frontColliderDistance != 0)
		{
			rearDriverW.brakeTorque = 20000/frontColliderDistance;
			rearPassengerW.brakeTorque = 20000 / frontColliderDistance;
			frontPassengerW.brakeTorque = 20000 / frontColliderDistance;
			frontDriverW.brakeTorque = 20000 / frontColliderDistance;

			brakeLights.SetActive(true);


		}

		else
		{
			rearDriverW.brakeTorque = 0;
			rearPassengerW.brakeTorque = 0;
			frontPassengerW.brakeTorque = 0;
			frontDriverW.brakeTorque = 0;
		}


		

	}


	void OnDrawGizmos()
	{
		
		if (frontHit.point != new Vector3(0, 0, 0) || leftHit.point != new Vector3(0, 0, 0) || rightHit.point != new Vector3(0, 0, 0) || rightsideHit.point != new Vector3(0, 0, 0) || leftsideHit.point != new Vector3(0, 0, 0))
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(frontHit.point, 0.1f);
			Gizmos.DrawSphere(leftHit.point, 0.1f);
			Gizmos.DrawSphere(rightHit.point, 0.1f);
			Gizmos.DrawSphere(leftsideHit.point, 0.1f);
			Gizmos.DrawSphere(rightsideHit.point, 0.1f);
			//Handles.Label(transform.position + transform.right * 2, "Ray distance: " + frontColliderDistance.ToString());
			//Handles.Label(transform.position + transform.right * -2, "Motorforce: " + motorForce.ToString());

		}
		

		Handles.Label(transform.position + transform.up * 4, this.name);
		Handles.Label(transform.position + transform.up * 3, "Velocity: " + this.GetComponent<Rigidbody>().velocity.ToString());
		Handles.Label(transform.position + transform.up * 2, "Speed: " + this.GetComponent<Rigidbody>().velocity.magnitude.ToString());

	}




	public void GetInput()
	{
		m_horizontalInput = Input.GetAxis("Horizontal");
		m_verticalInput = Input.GetAxis("Vertical");
	}

	private void Steer()
	{

		if (keyboardControl == true)
		{
			m_steeringAngle = maxSteerAngle * m_horizontalInput;
		}

		else
		{
			m_steeringAngle = maxSteerAngle * Mathf.Clamp(steerPID.Update(relative.x), -1f, 1f);
		}


		frontDriverW.steerAngle = m_steeringAngle;
		frontPassengerW.steerAngle = m_steeringAngle;
	}

	private void Accelerate()
	{
		if (Input.GetKey("space"))
		{

			Debug.Log("brake");

			rearDriverW.brakeTorque = 3330;
			rearPassengerW.brakeTorque = 3330;
			frontPassengerW.brakeTorque = 3330;
			frontDriverW.brakeTorque = 3330;
		}

		else
		{

			if (keyboardControl == false)
			{
				if (constantSpeed != 0)
				{
					m_verticalInput = constantSpeed;
				}

				else
				{
					m_verticalInput = Mathf.Clamp(accelPID.Update(relative.z), -100f, 100f);
					m_verticalInput += verticalInputMod;

				}

			}




			rearDriverW.brakeTorque = 0;
			rearPassengerW.brakeTorque = 0;
			frontPassengerW.brakeTorque = 0;
			frontDriverW.brakeTorque = 0;



			rearDriverW.motorTorque = m_verticalInput * motorForce;
			rearPassengerW.motorTorque = m_verticalInput * motorForce;
			frontDriverW.motorTorque = m_verticalInput * motorForce;
			frontPassengerW.motorTorque = m_verticalInput * motorForce;

		}
	}

	private void UpdateWheelPoses()
	{
		UpdateWheelPose(frontDriverW, frontDriverT);
		UpdateWheelPose(frontPassengerW, frontPassengerT);
		UpdateWheelPose(rearDriverW, rearDriverT);
		UpdateWheelPose(rearPassengerW, rearPassengerT);
	}

	private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
	{
		Vector3 _pos = _transform.position;
		Quaternion _quat = _transform.rotation;

		_collider.GetWorldPose(out _pos, out _quat);

		_transform.position = _pos;
		_transform.rotation = _quat;
	}



	[System.Serializable]
	public class PIDController
	{


		public float Kp = 0.2f;

		public float Ki = 0.05f;


		public float Kd = 1f;


		public float value = 0;

		private float lastError;
		public float integral;

		public float outputClamp;

		/// 
		/// Update our value, based on the given error.  We assume here that the
		/// last update was Time.deltaTime seconds ago.
		/// 
		/// <param name="error" />Difference between current and desired outcome.
		/// Updated control value.
		public float Update(float error)
		{
			return Update(error, Time.deltaTime);
		}

		/// 
		/// Update our value, based on the given error, which was last updated
		/// dt seconds ago.
		/// 
		/// <param name="error" />Difference between current and desired outcome.
		/// <param name="dt" />Time step.
		/// Updated control value.
		public float Update(float error, float dt)
		{
			float derivative = (error - lastError) / dt;
			integral += error * dt;
			lastError = error;

			value = (Kp * error + Ki * integral + Kd * derivative);
			value = Mathf.Clamp(value, -outputClamp, outputClamp);


			return value;
		}
	}


}
