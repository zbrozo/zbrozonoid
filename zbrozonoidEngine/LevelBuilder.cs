using System;
using System.Collections.Generic;
using System.Linq;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public class LevelBuilder
    {

        private ICollection<Player> players;

        private IScreen screen;
        private ILevelManager levelManager;
        private IPadManager padManager;
        private IBorderManager borderManager;
        private IBorderCollisionManager borderCollisionManager;
        private BallBuilder ballBuilder;
        private IGameConfig gameConfig;
        private ICollection<IBrick> bricks;

        public LevelBuilder(
            IScreen screen,
            ILevelManager levelManager,
            IPadManager padManager,
            IBorderManager borderManager,
            IBorderCollisionManager borderCollisionManager,
            BallBuilder ballBuilder,
            IGameConfig gameConfig,
            ICollection<IBrick> bricks)
        {
            this.screen = screen;
            this.levelManager = levelManager;
            this.padManager = padManager;
            this.borderManager = borderManager;
            this.borderCollisionManager = borderCollisionManager;
            this.ballBuilder = ballBuilder;
            this.gameConfig = gameConfig;
            this.bricks = bricks;
        }

        public void SetPlayers(ICollection<Player> players)
        {
            this.players = players;
        }

        public void Create(bool restartLevel)
        {
            CreateObjects();
            CreateLevelMap(restartLevel);
        }

        private void CreateObjects()
        {
            padManager.Create(gameConfig, players);
            borderManager.Create(screen, gameConfig, players);

            foreach (var pad in padManager)
            {
                borderCollisionManager.DetectAndVerify(borderManager, pad.Item3);
            }
        }

        private void CreateLevelMap(bool restartLevel)
        {
            if (restartLevel)
            {
                levelManager.Restart();
            }
            else
            {
                levelManager.MoveNext();
                levelManager.Load();
            }

            bricks.Clear();
            foreach (var brick in levelManager.GetCurrent().Bricks)
            {
                bricks.Add(brick);
            }
        }
    }
}
