﻿using System;
using System.Collections.Generic;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Bounce
{
    class BrickBreaker : PhysicalScene
    {
        override public Vector2 SceneSize { get { return new Vector2(sceneStack.GraphicsDevice.Viewport.Width, sceneStack.GraphicsDevice.Viewport.Height); } }
        private Camera2D camera;
        override public bool BlockDraw { get { return true; } }

        private List<RectangleItem> framing;
        private Paddle paddle;
        private PaddleBall ball;
        public List<Brick> bricks;

        public BrickBreaker(SceneStack sceneStack, Camera2D camera)
            : base(sceneStack, camera)
        {
            this.camera = camera;
            background = new Background(this, Vector2.Zero, "background2");

            Input.OnKeyDown += new KeyboardEvent(onKeyDown);
            Input.OnKeyUp += new KeyboardEvent(onKeyUp);
        }

        public override void Initialize()
        {
            framing = ItemStructures.CreateFraming(this, SceneSize, 20);
            PhysicalItems.AddRange(framing);

            bricks = brickWall(10);
            PhysicalItems.AddRange(bricks);

            paddle = ItemFactory.CreatePaddle(this, new Vector2(SceneSize.X / 2, SceneSize.Y * 0.95f));
            PhysicalItems.Add(paddle);

            ball = ItemFactory.CreatePaddleBall(this, paddle);
            PhysicalItems.Add(ball);

            positionBall(ball, paddle);
            killOnTouch<Brick>(ball);
        }

        private void positionBall(PaddleBall ball, Paddle paddle)
        {
            JointFactory.CreateWeldJoint(World, ball.Body, paddle.Body,
                Vector2.UnitY * ConvertUnits.ToSimUnits(ball.Texture.Height * 2),
                Vector2.Zero);
        }

        private List<Brick> brickWall(int rows)
        {
            var sampleBrick = ItemFactory.CreateBrick(this);
            var bricks = new List<Brick>();

            var rowStartingPositions = VectorStructures.Column(rows,
                new Vector2(sampleBrick.Texture.Width / 2, sampleBrick.Texture.Height / 2),
                sampleBrick.Texture.Height);

            foreach (Vector2 startingPosition in rowStartingPositions)
            {
                bricks.AddRange(ItemStructures.BrickRow(this,
                    sceneStack.GraphicsDevice.Viewport.Width / sampleBrick.Texture.Width - 1,
                    startingPosition,
                    sampleBrick.Texture.Width));
            }

            sampleBrick.Kill();

            return bricks;
        }

        private void killOnTouch<T>(PhysicalItem killAfterTouching) where T : PhysicalItem
        {
            killAfterTouching.Body.OnCollision += (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                if (fixtureB.Body.UserData is T)
                    (fixtureB.Body.UserData as T).Kill();

                return true;
            };
        }

        private void killAfterTouch<T>(PhysicalItem killAfterTouching) where T : PhysicalItem
        {
            killAfterTouching.Body.FixtureList[0].OnSeparation = (Fixture fixtureA, Fixture fixtureB) =>
            {
                if (fixtureB.Body.UserData is T)
                    (fixtureB.Body.UserData as T).Kill();
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (IsTop)
                Input.Update();

            worldGravityRotation(camera.Rotation);
            base.Update(gameTime);
        }

        void onKeyDown(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                if (ball.Body.JointList != null)
                {
                    World.RemoveJoint(ball.Body.JointList.Joint);

                    var force = new Vector2(BounceGame.r.Next(-100, 101) / 100f, -1);
                    ball.Body.ApplyLinearImpulse(force * BounceGame.MovementCoEf);
                }
                else
                {
                    ball.Kill();

                    ball = ItemFactory.CreatePaddleBall(this, paddle);
                    PhysicalItems.Add(ball);
                    positionBall(ball, paddle);
                    killOnTouch<Brick>(ball);
                }
            }
        }

        void onKeyUp(KeyboardState previousKeyboardState)
        {
            if (previousKeyboardState.IsKeyDown(Keys.RightControl) && previousKeyboardState.IsKeyDown(Keys.D1))
                sceneStack.Push(new BrickBreaker(sceneStack, camera));
            else if (previousKeyboardState.IsKeyDown(Keys.RightControl) && previousKeyboardState.IsKeyDown(Keys.Delete))
                sceneStack.Pop();
            else if (previousKeyboardState.IsKeyDown(Keys.RightShift) && previousKeyboardState.IsKeyDown(Keys.Delete))
                sceneStack.PopToHead();
        }
    }
}