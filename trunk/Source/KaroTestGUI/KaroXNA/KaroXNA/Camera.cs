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
        float ZoomValue = 1f;

        float prevXAngle = 0f;
        float prevYAngle = 0f;
        float prevZoomValue = 1f;

        private Vector3 CenterBoard = new Vector3(40f, 5f, 30f);

		public Camera(float aspectRatio)
		{
            this.CameraPosition = new Vector3(0f, 20f, 20f);
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(80), 1.3f, 0.1f, 1000f);
            this.UpdateView();
		}

        /// <summary>
        /// Rotate over Y-ax
        /// </summary>
        /// <param name="angle">Value to change the angle with</param>
		public void DoYRotation(float angle)
		{
            this.SetPreviousValues();

            if (YAngle + angle >= 360)
                this.YAngle -= 360;

            this.YAngle += angle;
            this.UpdateView();
        }

        /// <summary>
        /// Rotate over X-ax
        /// </summary>
        /// <param name="angle">Value to change the angle with</param>
        public void DoXRotation(float angle)
        {
            this.SetPreviousValues();

            this.XAngle += angle;
            this.UpdateView();
        }

        /// <summary>
        /// Zoom in/out
        /// </summary>
        /// <param name="value">Value to change the zoom with</param>
        public void DoZoom(float value)
        {
            this.SetPreviousValues();

            ZoomValue += value;

            if (ZoomValue <= 0) { ZoomValue = 0.01f; }
            if (ZoomValue >= 10) { ZoomValue = 10f; }

            this.UpdateView();
        }

        /// <summary>
        /// Update the camera position
        /// </summary>
        public void UpdateView()
        {
            Matrix m = Matrix.Identity * Matrix.CreateTranslation(CameraPosition) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(this.YAngle), MathHelper.ToRadians(this.XAngle), 0f) * Matrix.CreateScale(this.ZoomValue);
            this.View = Matrix.CreateLookAt(m.Translation + CenterBoard, CenterBoard, Vector3.Up);
        }

        /// <summary>
        /// Set the camera to "Top View"
        /// </summary>
        public void SetFixedTop()
        {
            this.SetPreviousValues();

            this.YAngle = 0f;
            this.XAngle = -44.999f;
            this.ZoomValue = 1f;

            this.UpdateView();
        }

        /// <summary>
        /// Shows the board from the view of player one
        /// </summary>
        public void SetFixedViewPlayerOne()
        {
            this.SetPreviousValues();

            this.YAngle = 0f;
            this.XAngle = 0f;
            this.ZoomValue = 1f;

            this.UpdateView();
        }

        /// <summary>
        /// Shows the board from the view of player two
        /// </summary>
        public void SetFixedViewPlayerTwo()
        {
            this.SetPreviousValues();

            this.YAngle = 0f;
            this.XAngle = 180f;
            this.ZoomValue = 1f;

            this.UpdateView();
        }

        /// <summary>
        /// Restores the previous position
        /// </summary>
        public void RestorePreviousValues()
        {
            // 'Back-up'
            float x, y, zoom;
            x = this.prevXAngle;        // Previous state
            y = this.prevYAngle;        // Previous state
            zoom = this.prevZoomValue;  // Previous state

            // Save the current state
            this.SetPreviousValues();

            // Set new positions
            this.XAngle = x;
            this.YAngle = y;
            this.ZoomValue = zoom;

            // Update the view
            this.UpdateView();
        }

        /// <summary>
        /// Saves current values in previous values
        /// </summary>
        private void SetPreviousValues()
        {
            this.prevXAngle = this.XAngle;
            this.prevYAngle = this.YAngle;
            this.prevZoomValue = this.ZoomValue;
        }
    }
}
