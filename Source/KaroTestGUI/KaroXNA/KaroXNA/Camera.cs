﻿using System;
using Microsoft.Xna.Framework;

namespace KaroXNA
{
    public class Camera
    {
        #region Properties
        public  Matrix View, Projection;
		public  Vector3 CameraPosition;

        float XAngle = 0f;
        float YAngle = 0f;
        float ZoomValue = 1f;

        float prevXAngle = 0f;
        float prevYAngle = 0f;
        float prevZoomValue = 1f;

        bool restore = false;

        private Vector3 CenterBoard = new Vector3(40f, 5f, 30f);
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aspectRatio">Aspectratio of screen</param>
        public Camera(float aspectRatio)
		{
            this.CameraPosition = new Vector3(0f, 20f, 20f);
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1.3f, 0.1f, 2000f);

            this.SetFixedViewPlayerTwo();
            
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

            // Limit angle
            if (this.XAngle > 45)
                this.XAngle = 45f;
            if (this.XAngle < -135)
                this.XAngle = -135;

            // Zorgen dat hij niet gaat flippen op 45 graden
            if (this.XAngle == -45f && angle > 0)
                this.XAngle = -44.999f;
            else if (this.XAngle == -45f && angle < 0)
                this.XAngle = -45.001f;

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

            if (ZoomValue <= 0.02f) { ZoomValue = 0.02f; }
            if (ZoomValue >= 10f) { ZoomValue = 10f; }

            this.UpdateView();
        }

        /// <summary>
        /// Update the camera position
        /// </summary>
        public void UpdateView()
        {
            Matrix m = Matrix.Identity * Matrix.CreateTranslation(CameraPosition) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(this.YAngle), MathHelper.ToRadians(this.XAngle), 0f) * Matrix.CreateScale(this.ZoomValue);
            this.View = Matrix.CreateLookAt(m.Translation + CenterBoard, CenterBoard, Vector3.Up);
            this.restore = false;
        }

        /// <summary>
        /// Set the camera to "Top View"
        /// </summary>
        public void SetFixedTop()
        {
            if (!restore)
            {
                this.SetPreviousValues();

                this.YAngle = 0f;
                this.XAngle = -44.999f;

                this.UpdateView();

                // Set na update view!!
                this.restore = true;
            }
            else
            {
                this.RestorePreviousValues();
            }
        }

        /// <summary>
        /// Shows the board from the view of player one
        /// </summary>
        public void SetFixedViewPlayerOne()
        {
            this.SetPreviousValues();

            this.YAngle = 0f;
            this.XAngle = 0f;
            this.ZoomValue = 2.5f;

            this.UpdateView();
        }

        /// <summary>
        /// Shows the board from the view of player two
        /// </summary>
        public void SetFixedViewPlayerTwo()
        {
            this.SetPreviousValues();

            this.YAngle = 180f;
            this.XAngle = 0f;
            this.ZoomValue = 2.5f;

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