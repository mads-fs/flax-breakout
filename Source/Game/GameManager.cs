using FlaxEngine;
using FlaxEngine.GUI;
using System.Collections.Generic;
using System.Linq;
using static Game.Extensions.CoreExtensions;

namespace Game
{
    public class GameManager : Script
    {
        [Header("Setup")]
        public Paddle PaddleActor;
        public Ball BallActor;
        public Vector3 PaddleStartPosition = new(0f, 60f, 70f);
        public float BallStartPositionOffset = 20f;
        [Header("Prefabs"), Space(2.5f)]
        public Prefab PelletPrefab;
        public Prefab HalfPelletPrefab;
        public Prefab PowerUpPrefab;
        public List<Prefab> Levels;
        [Header("Ui")]
        public UIControl ScoreTextControl;
        [Header("Read Only")]
        [ShowInEditor] private int score = 0;

        private int levelIndex = -1;
        private Actor level;
        private int levelGreyPellets = 0;
        private bool isBallStationary = true;

        private Label scoreTextLabel;

        public override void OnStart()
        {
            scoreTextLabel = ScoreTextControl.Get<Label>();
            scoreTextLabel.Text = "0";
            PaddleActor.IsAcceptingInput(true);
            BallActor.OnPelletHit += BallActor_OnPelletHit;
            BallActor.OnDeathzoneHit += BallActor_OnDeathzoneHit;
            BallActor.Init();
            LoadNextLevel();
            ResetPlayer();
        }

        public override void OnUpdate()
        {
            if (isBallStationary)
            {
                Vector3 paddlePosition = PaddleActor.Parent.Position;
                BallActor.Parent.Position = new(
                    paddlePosition.X,
                    paddlePosition.Y + BallStartPositionOffset,
                    paddlePosition.Z);
                if (Input.GetKeyDown(KeyboardKeys.Spacebar))
                {
                    isBallStationary = false;
                    BallActor.AddForce(Vector3.Up * BallActor.Speed, ForceMode.Impulse);
                }
            }
        }

        private void LoadNextLevel()
        {
            if (level != null) Destroy(level);

            levelIndex += 1;
            level = Instantiate(Levels[levelIndex], Vector3.Zero);
            foreach (Actor actor in level.Children)
            {
                Pellet pellet = actor.GetScript<Pellet>();
                if (pellet)
                {
                    if (pellet.Value == 0) levelGreyPellets += 1;
                }
            }
        }

        private void ResetPlayer() => isBallStationary = true;

        private void BallActor_OnPelletHit(Pellet pellet, Collision collision)
        {
            if (pellet.Value > 0)
            {
                score += pellet.Value;
                pellet.Despawn(collision.Contacts.First().Point);
                scoreTextLabel.Text = $"{score}";
                //Actor powerup = Instantiate(PowerUpPrefab, pelletPosition);
                if (level.ChildrenCount - levelGreyPellets <= 0)
                {
                    LoadNextLevel();
                    ResetPlayer();
                }
            }
        }

        private void BallActor_OnDeathzoneHit() => ResetPlayer();
    }
}