using System;
using Microsoft.Xna.Framework;

namespace KaroXNA
{
    public class Camera
    {
        public  Matrix View, Projection;
		public  Vector3 CameraPosition;

        float XAngle = 0f;
        float YAngle = 0f;

        private  Vector3 CenterBoard = new Vector3(30,10,20);

		public Camera()
		{
            this.CameraPosition = new Vector3(6f, 0, 6f);
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(80), 1.3f, 0.1f, 1000f);
            this.View = Matrix.CreateLookAt(CameraPosition + CenterBoard, CenterBoard, Vector3.Up);
		}

		public void DoYRotation(float angle)
		{
            if ((this.YAngle + angle) > 360)
            {
                this.YAngle = 0;
            }
            else if ((this.YAngle + angle) < 0)
            {
                this.YAngle = 360;
            }
            else
            {
                this.YAngle = /*this.YAngle +*/ angle;
            }
            CameraPosition = Vector3.Transform(CameraPosition, Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(YAngle), MathHelper.ToRadians(XAngle), 0f));
            this.View = Matrix.CreateLookAt(CameraPosition + CenterBoard, CenterBoard, Vector3.Up);
            
        }

        public void DoXRotation(float angle)
        {
            this.XAngle = this.XAngle + angle;
            CameraPosition = Vector3.Transform(CameraPosition, Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(YAngle), MathHelper.ToRadians(XAngle), 0f));
            this.View = Matrix.CreateLookAt(CameraPosition + CenterBoard, CenterBoard, Vector3.Up);
        }

        public void DoZoom(float value)
        {
            CameraPosition = Vector3.Transform(CameraPosition, Matrix.CreateScale(value + 1f));
            this.View = Matrix.CreateLookAt(CameraPosition + CenterBoard, CenterBoard, Vector3.Up);
        }
    }
}
