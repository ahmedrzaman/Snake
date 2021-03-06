﻿using System;
using System.Collections.Generic;
using System.Linq;
using SnakeNet.Components;
using Gamecmder.Input;
using Gamecmder.Rendering;
using Gamecmder.Screens;
using SnakeNet.GameObjects;

namespace SnakeNet.Screens
{
    public class GameScreen : ScreenBase
    {
        private const int GenerateFoodCount = 3;


        private readonly ICollisionSystem _collisionSystem;
        private readonly IScreenManager _screenManager;

        private readonly Snake _snake;
        private readonly Random _foodRandomiser = new Random();
        private readonly IList<Food> _foods;

        private readonly int _rendererWidth;
        private readonly int _rendererHeight;

        // Score related variables
        private int _score = 0;
        private int _scoreX = 0;
        private readonly int _scoreY = 0;
        private string _scoreText = string.Empty;

        // Game state
        private bool _isGameOver = false;

        public GameScreen(IScreenManager screenManager, int rendererWidth, int rendererHeight)
            : base(ScreenType.Screen)
        {
            _collisionSystem = new CollisionSystem();

            _snake = new Snake(_collisionSystem, 5, 60, 30);
            _foods = new List<Food>();

            _rendererWidth = rendererWidth;
            _rendererHeight = rendererHeight;

            _screenManager = screenManager;

            foreach (var bit in _snake.GetBits())
                _collisionSystem.Add(bit);

            UpdateScore();

            _collisionSystem.OnCollisionDetected += CollisionSystem_OnCollisionDetected;
        }

        public override void Draw(IRenderer renderer)
        {
            _snake.Draw(renderer);

            // Draw food
            foreach (var food in _foods)
                food.Draw(renderer);

            renderer.DrawText(_scoreText, _scoreX, _scoreY);
        }

        public override void Update(TimeSpan elapsed)
        {
            HandleInput(elapsed);
            
            _collisionSystem.Update(elapsed);

            if (_foods.Count == 0)
            {
                foreach (var _ in Enumerable.Range(0, GenerateFoodCount))
                {
                    var randomX = _foodRandomiser.Next(_rendererWidth);
                    var randomY = _foodRandomiser.Next(_rendererHeight);

                    var food = new Food(randomX, randomY);
                    _foods.Add(food);

                    _collisionSystem.Add(food);
                }
            }

            _snake.Update(elapsed);
        }

        public void HandleInput(TimeSpan elapsed)
        {
            var key = _screenManager.InputManager.State.KeyPressed;

            if (key == Key.None)
                return;

            if (key == Key.Up)
                _snake.Direction = MoveDirection.Up;
            else if (key == Key.Down)
                _snake.Direction = MoveDirection.Down;
            else if (key == Key.Left)
                _snake.Direction = MoveDirection.Left;
            else if (key == Key.Right)
                _snake.Direction = MoveDirection.Right;
        }

        private void IncrementScore()
        {
            _score++;

            UpdateScore();
        }

        private void UpdateScore()
        {
            _scoreText = $"Score: {_score:000}";
            _scoreX = _rendererWidth - _scoreText.Length;
        }

        private void CollisionSystem_OnCollisionDetected(CollisionSystem system, ICollidable first, ICollidable second)
        {
            if (first is SnakeBit && second is Food food)
            {
                IncrementScore();

                _foods.Remove(food);
                _snake.GrowSnake();
                _collisionSystem.Remove(food);
            }
            else if (first is SnakeBit bit1 && second is SnakeBit bit2)
            {
                if (!_isGameOver)
                {
                    var screen = new GameOverScreen(_screenManager, _rendererWidth, _rendererHeight, _score);
                    _screenManager.AddScreen(screen);
                    _screenManager.RemoveScreen(this);

                    screen.Show();
                }

                _isGameOver = true;
            }
        }
    }
}
