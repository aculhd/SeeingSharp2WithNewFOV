﻿/*
    Seeing# and all applications distributed together with it. 
	Exceptions are projects where it is noted otherwise.
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp2 (sourcecode)
     - http://www.rolandk.de (the authors homepage, german)
    Copyright (C) 2019 Roland König (RolandK)
    
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

using System;
using System.Threading.Tasks;
using SeeingSharp.Checking;
using SeeingSharp.Multimedia.Components;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp.Multimedia.Objects;
using SharpDX;

namespace SeeingSharp.SampleContainer.Basics3D._05_ParentChild
{
    [SampleDescription(
        "Parent/Child", 5, nameof(Basics3D),
        "PreviewImage.png",
        "https://github.com/RolandKoenig/SeeingSharp2/tree/master/Samples/SeeingSharp.SampleContainer/Basics3D/_05_ParentChild",
        typeof(SampleSettingsWith3D))]
    public class ParentChildSample : SampleBase
    {
        public override async Task OnStartupAsync(RenderLoop targetRenderLoop, SampleSettings settings)
        {
            targetRenderLoop.EnsureNotNull(nameof(targetRenderLoop));

            // Build dummy scene
            var scene = targetRenderLoop.Scene;
            var camera = targetRenderLoop.Camera;

            await targetRenderLoop.Scene.ManipulateSceneAsync(manipulator =>
            {
                // Create floor
                this.BuildStandardFloor(
                    manipulator, Scene.DEFAULT_LAYER_NAME);

                // Create cube geometry resource
                var resCubeGeometry = manipulator.AddGeometry(
                    new CubeGeometryFactory());
                var resMaterial = manipulator.AddSimpleColoredMaterial();

                //********************************
                // Create parent object
                var cubeMesh = new Mesh(resCubeGeometry, resMaterial);
                cubeMesh.Color = Color4Ex.GreenColor;
                cubeMesh.Position = new Vector3(0f, 0.5f, 0f);
                cubeMesh.EnableShaderGeneratedBorder();
                cubeMesh.BuildAnimationSequence()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_360DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .CallAction(() => cubeMesh.RotationEuler = Vector3.Zero)
                    .ApplyAndRewind();
                manipulator.Add(cubeMesh);

                //********************************
                // Create first level child
                var actChild = new Mesh(resCubeGeometry, resMaterial);
                actChild.Position = new Vector3(-2f, 0f, 0f);
                actChild.Scaling = new Vector3(0.5f, 0.5f, 0.5f);
                manipulator.Add(actChild);
                manipulator.AddChild(cubeMesh, actChild);

                actChild = new Mesh(resCubeGeometry, resMaterial);
                actChild.Position = new Vector3(0f, 0f, 2f);
                actChild.Scaling = new Vector3(0.5f, 0.5f, 0.5f);
                manipulator.Add(actChild);
                manipulator.AddChild(cubeMesh, actChild);

                //********************************
                // Create second level parent/child relationships
                var actSecondLevelParent = new Mesh(resCubeGeometry, resMaterial);
                actSecondLevelParent.Position = new Vector3(3f, 0f, 0f);
                actSecondLevelParent.Scaling = new Vector3(0.8f, 0.8f, 0.8f);
                actSecondLevelParent.Color = Color4Ex.BlueColor;
                actSecondLevelParent.BuildAnimationSequence()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_180DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .RotateEulerAnglesTo(new Vector3(0f, EngineMath.RAD_360DEG, 0f), TimeSpan.FromSeconds(2.0))
                    .WaitFinished()
                    .CallAction(() => actSecondLevelParent.RotationEuler = Vector3.Zero)
                    .ApplyAndRewind();
                manipulator.Add(actSecondLevelParent);
                manipulator.AddChild(cubeMesh, actSecondLevelParent);

                var actSecondLevelChild = new Mesh(resCubeGeometry, resMaterial);
                actSecondLevelChild.Position = new Vector3(1f, 0f, 0f);
                actSecondLevelChild.Scaling = new Vector3(0.4f, 0.4f, 0.4f);
                manipulator.Add(actSecondLevelChild);
                manipulator.AddChild(actSecondLevelParent, actSecondLevelChild);

                actSecondLevelChild = new Mesh(resCubeGeometry, resMaterial);
                actSecondLevelChild.Position = new Vector3(-1f, 0f, 0f);
                actSecondLevelChild.Scaling = new Vector3(0.4f, 0.4f, 0.4f);
                manipulator.Add(actSecondLevelChild);
                manipulator.AddChild(actSecondLevelParent, actSecondLevelChild);

                actSecondLevelChild = new Mesh(resCubeGeometry, resMaterial);
                actSecondLevelChild.Position = new Vector3(0f, 0f, 1f);
                actSecondLevelChild.Scaling = new Vector3(0.4f, 0.4f, 0.4f);
                manipulator.Add(actSecondLevelChild);
                manipulator.AddChild(actSecondLevelParent, actSecondLevelChild);
            });

            // Configure camera
            camera.Position = new Vector3(5f, 5f, 5f);
            camera.Target = new Vector3(0f, 0.5f, 0f);
            camera.UpdateCamera();

            // Append camera behavior
            targetRenderLoop.SceneComponents.Add(new FreeMovingCameraComponent());
        }
    }
}