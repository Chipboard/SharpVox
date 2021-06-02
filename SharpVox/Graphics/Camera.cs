using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SharpVox.Utilities;
using SharpVox.Environment;

namespace SharpVox.Graphics
{
    public class Camera : SceneObject
    {
        public Vec3 position;
        public Vec3 target;

        public Vec3 up;
        public Vec3 forward;
        public Vec3 backward;
        public Vec3 right;
        public Vec3 left;

        public Mat4 viewMatrix;

        public override void Update()
        {
            backward = VectorMath.Subtract(position, target);
            forward = VectorMath.Invert(backward);
            up = new Vec3(0, 1, 0);
            right = VectorMath.Normalize(VectorMath.Cross(up, backward));
            left = VectorMath.Invert(right);
            up = VectorMath.Cross(backward, right);

            viewMatrix = new Mat4();

            Renderer.screenStates.Shader.SetUniform("camPos", new Vec3(0, 0, 0));
        }
    }
}
