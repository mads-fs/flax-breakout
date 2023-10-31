using FlaxEngine;

namespace Game
{
    public class Pellet : Script
    {
        public int Value;
        private StaticModel mesh;

        public override void OnStart()
        {
            mesh = Actor.As<StaticModel>();
            if (mesh == null) Debug.LogError("There is no StaticModel attached to the Pellet.");
        }

        public void SetColor(MaterialBase newColor) => mesh.SetMaterial(0, newColor);

        public override void OnUpdate()
        {
            // Here you can add code that needs to be called every frame
        }

        public void Despawn(Vector3 worldImpactPoint)
        {
            // Destroy the collider to make sure we don't accidentally collide
            // again while we despawn this one.
            Destroy(Parent.GetChild<BoxCollider>());
            // Convert the World Impact Point to Local Space
            Vector3 localImpactPoint = Transform.WorldToLocal(worldImpactPoint);
            Destroy(Parent);
        }
    }
}