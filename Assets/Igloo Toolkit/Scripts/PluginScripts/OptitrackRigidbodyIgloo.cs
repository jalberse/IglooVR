﻿using System;
using UnityEngine;

namespace Igloo.Controllers
{
#pragma warning disable IDE0051 // Remove unused private members
    /// <summary>
    /// Igloo Optitrack Rigid Body Interpreter class
    /// </summary>
    public class OptitrackRigidBodyIgloo : MonoBehaviour
    {
        /// <summary>
        /// The Optitrack Streaming Client in the scene
        /// </summary>
        public OptitrackStreamingClient StreamingClient;

        /// <summary>
        /// ID of the rigid body
        /// </summary>
        public int _rigidBodyId;

        /// <summary>
        /// If True, Will follow position of Optitrack controller
        /// </summary>
        bool _followPosition = true;

        /// <summary>
        /// If True, Will follow rotation of Optitrack controller
        /// </summary>
        bool _followRotation = true;

        /// <summary>
        /// Sets up the optitrack system for Igloo use
        /// </summary>
        /// <param name="RigidBodyId">Id of the rigidbody</param>
        /// <param name="followPosition">Should follow the position</param>
        /// <param name="followRotation">Should follow the rotation</param>
        public void Setup(int RigidBodyId, bool followPosition = true, bool followRotation = true)
        {
            _rigidBodyId = RigidBodyId;
            _followPosition = followPosition;
            _followRotation = followRotation;
            // If the user didn't explicitly associate a client, find a suitable default.
            if (this.StreamingClient == null)
            {
                this.StreamingClient = OptitrackStreamingClient.FindDefaultClient();

                if (this.StreamingClient == null) this.StreamingClient = gameObject.AddComponent<OptitrackStreamingClient>();

                // If we still couldn't find one, disable this component.
                if (this.StreamingClient == null)
                {
                    Debug.LogError($"<b>[Igloo]</b> {GetType().FullName}: Streaming client not set, and no {typeof(OptitrackStreamingClient).FullName} components found in scene; disabling this component.", this);
                    this.enabled = false;
                    return;
                }
            }
        }

#if UNITY_2017_1_OR_NEWER
        /// <summary>
        /// Registers on before render event
        /// </summary>
        void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;
        }

        /// <summary>
        /// Deregisters on before render event
        /// </summary>
        void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
        }

        /// <summary>
        /// Update pose before rendering the scene
        /// </summary>
        void OnBeforeRender()
        {
            UpdatePose();
        }
#else
        /// <summary>
        /// Update pose during update method
        /// </summary>
        void Update()
        {
            UpdatePose();
        }
#endif

        /// <summary>
        /// Updates the current pose of the Optitrack system and sets the follow position and rotation. 
        /// </summary>
        void UpdatePose()
        {
            OptitrackRigidBodyState rbState = StreamingClient.GetLatestRigidBodyState(_rigidBodyId);
            if (rbState != null)
            {
                if (_followPosition) this.transform.localPosition = rbState.Pose.Position;
                if (_followRotation) this.transform.localRotation = rbState.Pose.Orientation;
            }
        }
    }
}

