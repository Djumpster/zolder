// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.AI;

namespace Talespin.Core.Foundation.Extensions
{
	public static class NavMeshAgentHelpers
	{
		public static NavMeshAgent Clone(this NavMeshAgent org, GameObject obj)
		{
			NavMeshAgent clone = obj.AddComponent<NavMeshAgent>();

			clone.agentTypeID = org.agentTypeID;
			clone.baseOffset = org.baseOffset;

			//Steering
			clone.speed = org.speed;
			clone.angularSpeed = org.angularSpeed;
			clone.acceleration = org.acceleration;
			clone.stoppingDistance = org.stoppingDistance;
			clone.autoBraking = org.autoBraking;

			//Obstacle Avoidance
			clone.radius = org.radius;
			clone.height = org.height;
			clone.obstacleAvoidanceType = org.obstacleAvoidanceType;
			clone.avoidancePriority = org.avoidancePriority;

			//path finding
			clone.autoTraverseOffMeshLink = org.autoTraverseOffMeshLink;
			clone.autoRepath = org.autoRepath;
			clone.areaMask = org.areaMask;

			return clone;
		}
	}
}
