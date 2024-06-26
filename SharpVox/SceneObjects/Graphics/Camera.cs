﻿using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.Window;
using SharpVox.Utilities;
using SharpVox.Environment;
using SharpVox.Input;

namespace SharpVox.Graphics
{
    public class Camera : SceneObject
    {
        public Vec3 position;
        public Vec3 target;

        public Vec3 up;
        public Vec3 forward;
        public Vec3 right;
        public float camSpeed = 4;

        public override void Start()
        {
            position = new Vec3(0, 1, -5);
            target = new Vec3(position.X, position.Y, position.Z + 1);
            up = new Vec3(0, 1, 0);

            forward = VectorMath.Normalize(VectorMath.Subtract(target, position));

            Renderer.RegisterUniform("camPos", position);
            Renderer.RegisterUniform("camForward", forward);
            Renderer.RegisterUniform("camRight", right);
            Renderer.RegisterUniform("camUp", up);
        }

        public override void Update()
        {
            forward = VectorMath.Normalize(VectorMath.Subtract(VectorMath.Add(position, forward), position));
            right = VectorMath.Cross(up, forward);
            up = VectorMath.Cross(forward, right);

            target = VectorMath.Add(position, forward);

            if (InputManager.GetKey(Keyboard.Key.W))
                position = VectorMath.Add(position, VectorMath.Multiply(forward, Core.Program.deltaTime * camSpeed));

            if (InputManager.GetKey(Keyboard.Key.S))
                position = VectorMath.Add(position, VectorMath.Multiply(VectorMath.Invert(forward), Core.Program.deltaTime * camSpeed));

            if (InputManager.GetKey(Keyboard.Key.D))
                position = VectorMath.Add(position, VectorMath.Multiply(right, Core.Program.deltaTime * camSpeed));

            if (InputManager.GetKey(Keyboard.Key.A))
                position = VectorMath.Add(position, VectorMath.Multiply(VectorMath.Invert(right), Core.Program.deltaTime * camSpeed));

            if (InputManager.GetMouseButton(Mouse.Button.Right))
            {
                InputManager.SetCursorVisible(false);
                InputManager.CenterCursor();

                forward = VectorMath.Add(forward, VectorMath.Multiply(right, InputManager.mouseMovementX * 0.001f));
                forward = VectorMath.Add(forward, VectorMath.Multiply(up, -InputManager.mouseMovementY * 0.001f));
            }
            else
            {
                InputManager.SetCursorVisible(true);
            }

            if(InputManager.mouseScroll != 0)
                camSpeed = Math.Clamp(camSpeed + (InputManager.mouseScroll * (0.01f + MathF.Pow(MathF.PI, camSpeed) * 0.01f)), 0.001f, 10);

            Renderer.SetUniform("camPos", position);
            Renderer.SetUniform("camForward", forward);
            Renderer.SetUniform("camRight", right);
            Renderer.SetUniform("camUp", up);
        }
    }
}
