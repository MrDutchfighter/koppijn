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

        private  Vector3 CenterBoard = new Vector3(0,0,0);

		public Camera()
		{
            this.CameraPosition = new Vector3(40f, 40f, 40f);
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 800f / 600f, 0.1f, 100f);
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
                this.YAngle = this.YAngle + angle;
            }
            CameraPosition = Vector3.Transform(new Vector3(40f, 40f, 40f), Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(YAngle), MathHelper.ToRadians(XAngle), 0f));
                this.View = Matrix.CreateLookAt(CameraPosition + CenterBoard, CenterBoard, Vector3.Up);
            
        }

        public void DoXRotation(float angle)
        {
               
                    this.XAngle = this.XAngle + angle;
                    CameraPosition = Vector3.Transform(new Vector3(40f, 40f, 40f), Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(YAngle), MathHelper.ToRadians(XAngle), 0f));
                    this.View = Matrix.CreateLookAt(CameraPosition + CenterBoard, CenterBoard, Vector3.Up);
        }

    }
}
