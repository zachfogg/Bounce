﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bounce
{
    class Camera2D// : GameComponent
    {
        public Camera2D(GraphicsDevice graphicsDevice)
        {
            Zoom = 1.0f;
            Rotation = 0.0f;
            Position = new Vector2(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f);
        }

        public Vector2 Position;
        public Matrix Transform;
        public float Zoom;
        public float Rotation;
        private Vector2 movement;

        public void Step(float movementCoEf)
        {
            movement = Vector2.Zero;

            if (BounceGame.KeyboardState.GetPressedKeys().Length != 0)
            {
                if (BounceGame.KeyboardState.IsKeyDown((Keys.NumPad8)))
                    movement.Y += -1f;
                if (BounceGame.KeyboardState.IsKeyDown((Keys.NumPad6)))
                    movement.X += 1f;
                if (BounceGame.KeyboardState.IsKeyDown((Keys.NumPad2)))
                    movement.Y += 1f;
                if (BounceGame.KeyboardState.IsKeyDown((Keys.NumPad4)))
                    movement.X += -1f;

                if (movement != Vector2.Zero)
                    movement.Normalize();

                Position += movement * movementCoEf;

                //Consider changing to exponential multiplication for zoom's value.
                if (BounceGame.KeyboardState.IsKeyDown(Keys.Add))
                    Zoom += 0.025f;
                if (BounceGame.KeyboardState.IsKeyDown(Keys.Subtract))
                    Zoom += -0.025f;

                //To do: implement camera rotation around the Z axis.
            }
        }

        public Matrix GetTransformation(GraphicsDevice graphicsDevice)
        {
            Transform =
                Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                    Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));

            return Transform;
        }
    }
}
