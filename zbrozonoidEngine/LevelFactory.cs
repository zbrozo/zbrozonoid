﻿using System;
using System.Collections.Generic;
using System.Linq;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public class LevelFactory
    {
        private IScreen screen;
        private ILevelManager levelManager;
        private IPadManager padManager;
        private IBorderManager borderManager;
        private IBorderCollisionManager borderCollisionManager;
        private BallFactory ballFactory;
        private IGameConfig gameConfig;
        private ICollection<IBrick> bricks;

        public LevelFactory(
            IScreen screen,
            ILevelManager levelManager,
            IPadManager padManager,
            IBorderManager borderManager,
            IBorderCollisionManager borderCollisionManager,
            BallFactory ballFactory,
            IGameConfig gameConfig,
            ICollection<IBrick> bricks)
        {
            this.screen = screen;
            this.levelManager = levelManager;
            this.padManager = padManager;
            this.borderManager = borderManager;
            this.borderCollisionManager = borderCollisionManager;
            this.ballFactory = ballFactory;
            this.gameConfig = gameConfig;
            this.bricks = bricks;
        }

        public void Create(bool restartLevel)
        {
            CreateObjects();
            CreateLevelMap(restartLevel);
        }

        private void CreateObjects()
        {
            padManager.Create(gameConfig);
            borderManager.Create(screen, gameConfig);

            foreach (var pad in padManager)
            {
                borderCollisionManager.DetectAndVerify(borderManager, pad.Item3);
            }

            ballFactory.CreateBalls();
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
