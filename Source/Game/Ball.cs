using FlaxEngine;
using Game.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Ball : Script
    {
        public event Action<Pellet, Collision> OnPelletHit;
        public event Action OnDeathzoneHit;

        public float Speed = 50f;
        public float VerticalVelocityIncrease = 5f;
        [Range(0f, 1f)] public float HorizontalVelocityFactor = 0.1f;
        public List<Tag> ColliderTags;
        public Tag PaddleTag;
        public Tag PelletTag;
        public Tag DeathzoneTag;

        private RigidBody rbody;
        private SphereCollider collider;
        private Vector3 lastVelocityFrame;

        public void Init()
        {
            rbody = Actor.Get<RigidBody>();
            collider = Actor.GetInChildren<SphereCollider>();
            collider.CollisionEnter += OnCollisionEnter;
        }

        public void AddForce(Vector3 force, ForceMode mode) => rbody.AddForce(force, mode);

        public override void OnLateUpdate() => lastVelocityFrame = rbody.LinearVelocity;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.OtherCollider.Tags.HasAny(ColliderTags.ToArray()))
            {
                if (collision.OtherCollider.Tags.HasTag(PelletTag))
                {
                    OnPelletHit?.Invoke(collision.OtherCollider.GetInParent<Pellet>(), collision);
                }
                Vector3 hitNormal = collision.Contacts.First().Normal;
                Vector3 linearVelocity = lastVelocityFrame;
                Vector3.Reflect(ref linearVelocity, ref hitNormal, out Vector3 velocity);
                if (collision.OtherActor.Tags.HasTag(PaddleTag))
                {
                    float x = collision.OtherActor.GetInParent<Paddle>().GetVelocity();
                    float y = 0f;

                    if (velocity.Y < 0) y = velocity.Y + -VerticalVelocityIncrease;
                    else if (velocity.Y > 0) y = velocity.Y + VerticalVelocityIncrease;
                    velocity = new(velocity.X + x, y, 0);
                }
                rbody.LinearVelocity = new(velocity.X, velocity.Y, 0f);
            }

            if(collision.OtherCollider.Tags.HasTag(DeathzoneTag))
            {
                OnDeathzoneHit?.Invoke();
            }
        }
    }
}