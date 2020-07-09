// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Cameras;
using Talespin.Core.Swizzle;
using UnityEngine;

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// Automatically adjusts the object's transform according to the user's rotation
	/// </summary>
	public class FollowCameraBehaviour : MonoBehaviour
	{
		private static int collectiveShouldMove = 0;

		[SerializeField] private bool stopRotatingWhenOffsetIsSet = true;
		[SerializeField] private bool useRealTime;

		[Header("User Facing")]
		[SerializeField] private float xSmoothing = 5.0f;
		[SerializeField] private float xMinimumDelta = 0.0f;
		[SerializeField] private float xDeadZone = 1.0f;

		[Header("User Height")]
		[SerializeField] private float yMinimumDelta = 0.15f;
		[SerializeField] private float yMinimumVelocity = 0.1f;
		[SerializeField] private float ySmoothing = 0.3f;

		private ICameraController cameraController;

		private Quaternion originalRotation;
		private Vector3 targetForward;

		private float yPos;
		private float yVelocity;

		private bool rotating = true;
		private bool instanceShouldMove = false;

		public void InjectDependencies(ICameraController cameraController)
		{
			this.cameraController = cameraController;
		}

		protected void Start()
		{
			originalRotation = transform.rotation;
			targetForward = cameraController.Head.forward.xoz();

			transform.forward = targetForward;
			transform.position = cameraController.Head.position;
			yPos = transform.position.y;
		}

		protected void OnDestroy()
		{
			if (instanceShouldMove)
			{
				collectiveShouldMove = Math.Max(0, collectiveShouldMove - 1);
			}
		}

		protected void Update()
		{
			bool shouldMove = Mathf.Abs(cameraController.Head.position.y - yPos) >= yMinimumDelta || yVelocity > yMinimumVelocity;

			if (shouldMove && !instanceShouldMove)
			{
				collectiveShouldMove++;
			}

			if (!shouldMove && instanceShouldMove)
			{
				collectiveShouldMove = Math.Max(0, collectiveShouldMove - 1);
			}

			instanceShouldMove = shouldMove;

			if (rotating)
			{
				if (!stopRotatingWhenOffsetIsSet || cameraController.RotationOffset.y == 0.0f)
				{
					float deltaCamera = Vector3.Angle(transform.forward.xoz(), cameraController.Head.forward.xoz());
					float deltaTarget = Vector3.Angle(transform.forward.xoz(), targetForward);
					if (deltaCamera >= xMinimumDelta || deltaTarget > xDeadZone)
					{
						targetForward = cameraController.Head.forward.xoz();
					}

					float delta = useRealTime ? Time.unscaledDeltaTime : Time.deltaTime;
					transform.forward = Slerp(transform.forward, targetForward, xSmoothing, delta);
				}
				else
				{
					transform.rotation = originalRotation;
					rotating = false;
				}
			}
		}

		protected void LateUpdate()
		{
			Vector3 userPosition = cameraController.Head.position;
			if (collectiveShouldMove > 0)
			{
				yPos = userPosition.y;
			}

			float delta = useRealTime ? Time.unscaledDeltaTime : Time.deltaTime;
			if (delta > 0)
			{
				userPosition.y = Mathf.SmoothDamp(transform.position.y, yPos, ref yVelocity, ySmoothing, float.MaxValue, delta);
				transform.position = userPosition;
			}
		}

		private static Vector3 Slerp(Vector3 eulerAnglesCurrent, Vector3 eulerAnglesTarget, float speed, float deltaTime)
		{
			Vector3 velocity = Vector3.zero;
			eulerAnglesCurrent.x = Mathf.SmoothDampAngle(eulerAnglesCurrent.x, eulerAnglesTarget.x, ref velocity.x, speed * deltaTime, Mathf.Infinity, deltaTime);
			eulerAnglesCurrent.y = Mathf.SmoothDampAngle(eulerAnglesCurrent.y, eulerAnglesTarget.y, ref velocity.y, speed * deltaTime, Mathf.Infinity, deltaTime);
			eulerAnglesCurrent.z = Mathf.SmoothDampAngle(eulerAnglesCurrent.z, eulerAnglesTarget.z, ref velocity.z, speed * deltaTime, Mathf.Infinity, deltaTime);
			return eulerAnglesCurrent;
		}
	}
}
