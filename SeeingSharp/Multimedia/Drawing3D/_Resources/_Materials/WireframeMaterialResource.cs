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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.Drawing3D
{
    public class WireframeMaterialResource : MaterialResource
    {
        // Resource keys
        private static readonly NamedOrGenericKey RES_KEY_VERTEX_SHADER = GraphicsCore.GetNextGenericResourceKey();
        private static readonly NamedOrGenericKey RES_KEY_PIXEL_SHADER = GraphicsCore.GetNextGenericResourceKey();
        private static readonly NamedOrGenericKey RES_KEY_GEO_SHADER = GraphicsCore.GetNextGenericResourceKey();
        private readonly NamedOrGenericKey KEY_CONSTANT_BUFFER = GraphicsCore.GetNextGenericResourceKey();

        // Some configuration
        private Color4 m_materialDiffuseColor;
        private float m_clipFactor;
        private float m_maxClipDistance;
        private float m_addToAlpha;
        private bool m_adjustTextureCoordinates;
        private bool m_cbPerMaterialDataChanged;
        private bool m_useVertexColors;

        // Resource members
        private VertexShaderResource m_vertexShader;
        private GeometryShaderResource m_geoShader;
        private PixelShaderResource m_pixelShader;
        private TypeSafeConstantBufferResource<CBPerMaterial> m_cbPerMaterial;
        private DefaultResources m_defaultResources;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMaterialResource"/> class.
        /// </summary>
        public WireframeMaterialResource()
        {
            m_maxClipDistance = 1000f;
            m_adjustTextureCoordinates = false;
            m_addToAlpha = 0f;
            m_materialDiffuseColor = Color4.White;
            m_useVertexColors = true;
        }

        /// <inheritdoc />
        protected override void LoadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            // Load all required shaders and constant buffers
            m_vertexShader = resources.GetResourceAndEnsureLoaded(
                RES_KEY_VERTEX_SHADER,
                () => GraphicsHelper.Internals.GetVertexShaderResource(device, "Common", "WireframeVertexShader"));
            m_geoShader = resources.GetResourceAndEnsureLoaded(
                RES_KEY_GEO_SHADER,
                () => GraphicsHelper.Internals.GetGeometryShaderResource(device, "Common", "WireframeGeometryShader"));
            m_pixelShader = resources.GetResourceAndEnsureLoaded(
                RES_KEY_PIXEL_SHADER,
                () => GraphicsHelper.Internals.GetPixelShaderResource(device, "Common", "WireframePixelShader"));
            m_cbPerMaterial = resources.GetResourceAndEnsureLoaded(
                KEY_CONSTANT_BUFFER,
                () => new TypeSafeConstantBufferResource<CBPerMaterial>());
            m_cbPerMaterialDataChanged = true;

            // Get a reference to default resource object
            m_defaultResources = resources.GetResourceAndEnsureLoaded<DefaultResources>(DefaultResources.RESOURCE_KEY);
        }

        /// <inheritdoc />
        protected override void UnloadResourceInternal(EngineDevice device, ResourceDictionary resources)
        {
            m_vertexShader = null;
            m_geoShader = null;
            m_pixelShader = null;
            m_cbPerMaterial = null;
        }

        /// <inheritdoc />
        internal override D3D11.InputLayout GetInputLayout(EngineDevice device, D3D11.InputElement[] inputElements)
        {
            return m_vertexShader.GetInputLayout(device, inputElements);
        }

        /// <inheritdoc />
        internal override void Apply(RenderState renderState, MaterialResource previousMaterial)
        {
            var deviceContext = renderState.Device.DeviceImmediateContextD3D11;
            var isResourceSameType =
                previousMaterial != null &&
                previousMaterial.ResourceType == this.ResourceType;

            // Apply local shader configuration
            if (m_cbPerMaterialDataChanged)
            {
                m_cbPerMaterial.SetData(
                    deviceContext,
                    new CBPerMaterial
                    {
                        ClipFactor = m_clipFactor,
                        MaxClipDistance = m_maxClipDistance,
                        Texture0Factor = 0f,
                        AdjustTextureCoordinates = m_adjustTextureCoordinates ? 1f : 0f,
                        AddToAlpha = m_addToAlpha,
                        MaterialDiffuseColor = m_materialDiffuseColor,
                        DiffuseColorFactor = m_useVertexColors ? 0f : 1f,
                    });
                m_cbPerMaterialDataChanged = false;
            }

            // Apply sampler and constants
            deviceContext.PixelShader.SetConstantBuffer(3, m_cbPerMaterial.ConstantBuffer);
            deviceContext.VertexShader.SetConstantBuffer(3, m_cbPerMaterial.ConstantBuffer);

            // Set texture resource (if set)
            deviceContext.PixelShader.SetShaderResource(0, null);

            // Set shader resources
            if (!isResourceSameType)
            {
                deviceContext.VertexShader.Set(m_vertexShader.VertexShader);
                deviceContext.GeometryShader.Set(m_geoShader.GeometryShader);
                deviceContext.PixelShader.Set(m_pixelShader.PixelShader);
            }
        }

        /// <inheritdoc />
        internal override void Discard(RenderState renderState)
        {
            var deviceContext = renderState.Device.DeviceImmediateContextD3D11;
            deviceContext.GeometryShader.Set(null);
        }

        /// <inheritdoc />
        public override bool IsLoaded => m_vertexShader != null;

        /// <summary>
        /// Gets or sets the ClipFactor.
        /// Pixel are clipped up to an alpha value defined by this ClipFactor within the pixel shader.
        /// </summary>
        public float ClipFactor
        {
            get => m_clipFactor;
            set
            {
                if (!EngineMath.EqualsWithTolerance(m_clipFactor, value))
                {
                    m_clipFactor = value;
                    m_cbPerMaterialDataChanged = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum distance on which to apply pixel clipping (defined by ClipFactor property).
        /// </summary>
        public float MaxClipDistance
        {
            get => m_maxClipDistance;
            set
            {
                if (!EngineMath.EqualsWithTolerance(m_maxClipDistance, value))
                {
                    m_maxClipDistance = value;
                    m_cbPerMaterialDataChanged = true;
                }
            }
        }

        /// <summary>
        /// Interpolate texture coordinate based on xy-scaling.
        /// </summary>
        public bool AdjustTextureCoordinates
        {
            get => m_adjustTextureCoordinates;
            set
            {
                if (m_adjustTextureCoordinates != value)
                {
                    m_adjustTextureCoordinates = value;
                    m_cbPerMaterialDataChanged = true;
                }
            }
        }

        /// <summary>
        /// Needed for video rendering (Frames from the MF SourceReader have alpha always to zero).
        /// </summary>
        public float AddToAlpha
        {
            get => m_addToAlpha;
            set
            {
                if (!EngineMath.EqualsWithTolerance(m_addToAlpha, value))
                {
                    m_addToAlpha = value;
                    m_cbPerMaterialDataChanged = true;
                }
            }
        }

        public Color4 MaterialDiffuseColor
        {
            get => m_materialDiffuseColor;
            set
            {
                if (m_materialDiffuseColor != value)
                {
                    m_materialDiffuseColor = value;
                    m_cbPerMaterialDataChanged = true;
                }
            }
        }

        public bool UseVertexColors
        {
            get => m_useVertexColors;
            set
            {
                if (m_useVertexColors != value)
                {
                    m_useVertexColors = value;
                    m_cbPerMaterialDataChanged = true;
                }
            }
        }
    }
}
