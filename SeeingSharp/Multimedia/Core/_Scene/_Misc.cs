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

namespace SeeingSharp.Multimedia.Core
{
    public delegate void Rendering3DHandler(object sender, Rendering3DArgs e);
    public delegate void Updating3DHandler(object sender, Updating3DArgs e);

    /// <summary>
    /// The transformation type used within a scene object node.
    /// </summary>
    public enum SpacialTransformationType
    {
        /// <summary>
        /// Scaling, translation and Rotation using euler angles (pitch, yaw and roll).
        /// </summary>
        ScalingTranslationEulerAngles,

        /// <summary>
        /// Scaling, translation and Rotation using quaternion.
        /// </summary>
        ScalingTranslationQuaternion,

        /// <summary>
        /// Scaling, translation and rotation based on direction vectors (forward, up)
        /// </summary>
        ScalingTranslationDirection,

        /// <summary>
        /// Scaling and translation components.
        /// </summary>
        ScalingTranslation,

        /// <summary>
        /// Translation and Rotation using euler angles (pitch, yaw and roll).
        /// </summary>
        TranslationEulerAngles,

        /// <summary>
        /// Translation and Rotation using quaternion.
        /// </summary>
        TranslationQuaternion,

        /// <summary>
        /// Translation and rotation using direction vectors (forward, up)
        /// </summary>
        TranslationDirection,

        /// <summary>
        /// Just translation transformation
        /// </summary>
        Translation,

        /// <summary>
        /// A custom transformation matrix.
        /// </summary>
        CustomTransform,

        /// <summary>
        /// The object should take its transform from another object.
        /// </summary>
        TakeFromOtherObject,

        /// <summary>
        /// No transformation at all
        /// </summary>
        None
    }

    /// <summary>
    /// Defines the host-mode remote object hosting.
    /// </summary>
    public enum ObjectHostMode
    {
        /// <summary>
        /// Default mode - overtake all possible values.
        /// </summary>
        Default,

        /// <summary>
        /// Take all values, but don't take rotation value.
        /// </summary>
        IgnoreRotation,

        /// <summary>
        /// Take all values, but don't take size value.
        /// </summary>
        IgnoreScaling,

        /// <summary>
        /// Take all values, but don't take size and rotation value.
        /// </summary>
        IgnoreRotationScaling
    }

    /// <summary>
    /// Controls how a SceneObject's visibility is calculated.
    /// </summary>
    public enum VisibilityTestMethod
    {
        /// <summary>
        /// Default. The object is visible when it passes all filters on a view.
        /// </summary>
        ByObjectFilters,

        /// <summary>
        /// The object will be rendered always.
        /// </summary>
        ForceVisible,

        /// <summary>
        /// The object is hidden always.
        /// </summary>
        ForceHidden
    }

    /// <summary>
    /// EventArgs class for Rendering3DHandler.
    /// </summary>
    public class Rendering3DArgs : EventArgs
    {
        /// <summary>
        /// Gets the render state.
        /// </summary>
        /// <value>Gets the render state.</value>
        public RenderState RenderState
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rendering3DArgs"/> class.
        /// </summary>
        /// <param name="renderState">Current render state.</param>
        public Rendering3DArgs(RenderState renderState)
        {
            this.RenderState = renderState;
        }
    }

    /// <summary>
    /// EventArgs class for Updating3DHandler.
    /// </summary>
    public class Updating3DArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the update state.
        /// </summary>
        public UpdateState UpdateState
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Updating3DArgs"/> class.
        /// </summary>
        public Updating3DArgs(UpdateState updateState)
        {
            this.UpdateState = updateState;
        }
    }
}