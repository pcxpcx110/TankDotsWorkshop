﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Workshop.TankGame
{
	/// <summary>
	/// JobComponentSystem enable us to create jobs with Job System.
	/// Turning this into a JobComponentSystem will make Unity to split our job in different threads for us, faster.
	/// </summary>
	public class MoveForwardSystem : JobComponentSystem
	{
		// =============================================================================================================
		/// <summary>
		/// This will be our job to run in our JobComponentSystem. We could create more than one, as many as we want.
		/// First part of it is a query - what data you are operating on and, for execution, what you do to that data?
		/// And our query says "find everything that has a Translation, Rotation and MoveSpeed".
		/// Also see that we ask for a tag to run it, otherwise other entities with the same data (translation...
		/// rotation and MoveSpeed) could also be executed by our query, because it does not know what is what.
		/// Note: we already added BurstCompile.
		/// Note 2: you can't access Unity normal components and MonoBehaviours from here.
		/// </summary>
		[BurstCompile]
		[RequireComponentTag(typeof(MoveForwardTag))] //this is a way to identify what entities we want to run this query.
		struct MoveForwardRotation : IJobForEach<Translation, Rotation, MoveSpeed>
		{
			public float myDeltaTime;
			/// <summary>
			/// Each entity that has these data, will be executed by our query.
			/// It will happen for each data.
			/// </summary>
			/// <param name="pos"></param>
			/// <param name="rot"></param>
			/// <param name="speed"></param>
			public void Execute(ref Translation pos, [ReadOnly] ref Rotation rot, [ReadOnly] ref MoveSpeed speed)
			{
				//We can't use Time.deltaTime here, since it only be called from the main thread.
				//That's what our job will do for us. If you do, even if you don't get a compile error, it will crash
				//during runtime.
				pos.Value = pos.Value + (myDeltaTime * speed.speedValue * math.forward(rot.Value));
			}
		}
		// =============================================================================================================
		/// <summary>
		/// This runs our jobs and reads in our input dependencies and return to us.
		/// </summary>
		/// <param name="inputDeps">Input dependencies</param>
		/// <returns></returns>
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var moveForwardRotationJob = new MoveForwardRotation
			{
				myDeltaTime = Time.deltaTime
			};

			return moveForwardRotationJob.Schedule(this, inputDeps);
		}
		// =============================================================================================================
	}
}