using FlaxEngine;

namespace Game
{
    public class Paddle : Script
    {
        [Header("Setup")]
        public Actor LeftSide;
        public Actor RightSide;
        [Header("Rules")]
        public float XMin = -256f;
        public float XMax = 256f;
        public float Speed = 50f;
        public float VelocityDecay = 0.5f;
        public float MaxVelocity = 8f;
        [Header("Debug"), Space(2.5f)]
        [ShowInEditor] private bool isAcceptingInput;
        [ShowInEditor] private float velocity = 0f;

        private float startY;
        private float startZ;

        public override void OnStart()
        {
            Vector3 position = Actor.LocalPosition;
            startY = position.Y;
            startZ = position.Z;
        }

        public override void OnUpdate()
        {
            if (velocity < 0f)
            {
                velocity = Mathf.Min(0f, velocity + VelocityDecay);
                velocity = Mathf.Clamp(velocity, -MaxVelocity, 0f);
            }
            else if (velocity > 0f)
            {
                velocity = Mathf.Max(0f, velocity - VelocityDecay);
                velocity = Mathf.Clamp(velocity, 0f, MaxVelocity);
            }

            if(isAcceptingInput)
            {
                float horizontal = Input.GetAxis("Horizontal");
                float delta;
                if (horizontal < 0f)
                {
                    delta = (Mathf.Abs(horizontal) * Speed * Time.DeltaTime) * -1f;
                    velocity += delta;
                }
                else if (horizontal > 0f)
                {
                    delta = horizontal * Speed * Time.DeltaTime;
                    velocity += delta;
                }
                float clampedX = Mathf.Clamp(Actor.LocalPosition.X + velocity, XMin, XMax);
                Actor.LocalPosition = new(clampedX, startY, startZ);
            }
        }

        public void IsAcceptingInput(bool state) => isAcceptingInput = state;
        public float GetVelocity() => velocity;
    }
}
