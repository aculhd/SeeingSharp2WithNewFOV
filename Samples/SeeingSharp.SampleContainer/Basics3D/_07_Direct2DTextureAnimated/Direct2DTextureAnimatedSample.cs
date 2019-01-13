﻿#region License information
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp2 (sourcecode)
     - http://www.rolandk.de (the autors homepage, german)
    Copyright (C) 2018 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Components;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing2D;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;
using SharpDX;

namespace SeeingSharp.SampleContainer.Basics3D._07_Direct2DTextureAnimated
{
    [SampleDescription(
        "Direct2D Texture (animated)", 7, nameof(SeeingSharp.SampleContainer.Basics3D),
        sampleImageFileName:"PreviewImage.png",
        sourceCodeUrl: "https://github.com/RolandKoenig/SeeingSharp2/tree/master/_Samples/SeeingSharp.SampleContainer/Basics3D/_07_Direct2DTextureAnimated")]
    public class Direct2DTextureAnimatedSample : SampleBase
    {
        private SolidBrushResource m_solidBrush;
        private TextFormatResource m_textFormat;
        private SolidBrushResource m_animatedRectBrush;

        /// <summary>
        /// Called when the sample has to startup.
        /// </summary>
        public override async Task OnStartupAsync(RenderLoop targetRenderLoop, SampleSettings settings)
        {
            targetRenderLoop.EnsureNotNull(nameof(targetRenderLoop));

            // Build dummy scene
            Scene scene = targetRenderLoop.Scene;
            Camera3DBase camera = targetRenderLoop.Camera as Camera3DBase;

            // Whole animation takes x milliseconds
            float animationMillis = 3000f;

            // 2D rendering is made here
            m_solidBrush = new SolidBrushResource(Color4Ex.Gray);
            m_animatedRectBrush = new SolidBrushResource(Color4Ex.RedColor);
            Custom2DDrawingLayer d2dDrawingLayer = new Custom2DDrawingLayer((graphics) =>
            {
                // Draw the background
                RectangleF d2dRectangle = new RectangleF(10, 10, 236, 236);
                graphics.Clear(Color4Ex.LightBlue);
                graphics.FillRoundedRectangle(
                    d2dRectangle, 30, 30,
                    m_solidBrush);

                // Recalculate current location of the red rectangle on each frame
                float currentLocation = ((float)(DateTime.UtcNow - DateTime.UtcNow.Date).TotalMilliseconds % animationMillis) / animationMillis;
                var rectPos = GetAnimationLocation(currentLocation, 165f, 165f);
                graphics.FillRectangle(
                    new RectangleF(
                        20f + rectPos.x,
                        20f + rectPos.y,
                        50f, 50f),
                    m_animatedRectBrush);
            });

            // Build 3D scene
            await targetRenderLoop.Scene.ManipulateSceneAsync((manipulator) =>
            {
                // Create floor
                base.BuildStandardFloor(
                    manipulator, Scene.DEFAULT_LAYER_NAME);

                // Define Direct2D texture resource
                var resD2DTexture = manipulator.AddResource<Direct2DTextureResource>(
                    () => new Direct2DTextureResource(d2dDrawingLayer, 256, 256));
                var resD2DMaterial = manipulator.AddSimpleColoredMaterial(resD2DTexture);

                // Create cube geometry resource
                var pType = new CubeType();
                pType.Material = resD2DMaterial;
                var resPalletGeometry = manipulator.AddResource<GeometryResource>(
                    () => new GeometryResource(pType));

                // Create cube object
                GenericObject cubeObject = manipulator.AddGeneric(resPalletGeometry);
                cubeObject.Color = Color4Ex.GreenColor;
                cubeObject.YPos = 0.5f;
                cubeObject.EnableShaderGeneratedBorder();
                cubeObject.BuildAnimationSequence()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_360DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .CallAction(() => cubeObject.RotationEuler = Vector3.Zero)
                    .ApplyAndRewind();
            });

            // Configure camera
            camera.Position = new Vector3(3f, 3f, 3f);
            camera.Target = new Vector3(0f, 0.5f, 0f);
            camera.UpdateCamera();

            // Append camera behavior
            targetRenderLoop.SceneComponents.Add(new FreeMovingCameraComponent());
        }

        public override void NotifyClosed()
        {
            base.NotifyClosed();

            SeeingSharpUtil.SafeDispose(ref m_solidBrush);
            SeeingSharpUtil.SafeDispose(ref m_animatedRectBrush);
            SeeingSharpUtil.SafeDispose(ref m_textFormat);
        }

        public (float x, float y) GetAnimationLocation(float procentualLoc, float maxWidth, float maxHeight)
        {
            float xPos = 0f;
            float yPos = 0f;
            float currentLineLoc = (procentualLoc % 0.25f) / 0.25f;
            if(procentualLoc < 0.25f)
            {
                xPos = maxWidth * currentLineLoc;
                yPos = 0f;
            }
            else if(procentualLoc < 0.5f)
            {
                xPos = maxWidth;
                yPos = maxHeight * currentLineLoc;
            }
            else if(procentualLoc < 0.75f)
            {
                xPos = maxWidth - (maxWidth * currentLineLoc);
                yPos = maxHeight;
            }
            else
            {
                xPos = 0f;
                yPos = maxHeight - (maxHeight * currentLineLoc);
            }

            return (xPos, yPos);
        }
    }
}