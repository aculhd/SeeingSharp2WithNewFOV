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
using SeeingSharp.Util;
using System;
using SeeingSharp.Checking;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace SeeingSharp.Multimedia.Core
{
    public class TextureUploader : IDisposable, ICheckDisposed
    {
        // Given parameters
        private EngineDevice _device;
        private int _width;
        private int _height;
        private DXGI.Format _format;
        private bool _isMultisampled;
        private bool _isDisposed;

        //Direct3D resources for render target capturing
        // A staging texture for reading contents by Cpu
        // A standard texture for copying data from multisample texture to standard one
        // see http://www.rolandk.de/wp/2013/06/inhalt-der-rendertarget-textur-in-ein-bitmap-kopieren/
        private D3D11.Texture2D _copyHelperTextureStaging;
        private D3D11.Texture2D _copyHelperTextureStandard;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureUploader"/> class.
        /// </summary>
        /// <param name="device">The device on which the texture was created.</param>
        /// <param name="pixelWidth">With of textures to be uploaded.</param>
        /// <param name="pixelHeight">Height of textures to be uploaded.</param>
        /// <param name="format">Format of textures to be uploaded.</param>
        /// <param name="isMultisampled">True if this uploader expects multisampled textures.</param>
        internal TextureUploader(EngineDevice device, int pixelWidth, int pixelHeight, DXGI.Format format, bool isMultisampled)
        {
            _device = device;
            _width = pixelWidth;
            _height = pixelHeight;
            _format = format;
            _isMultisampled = isMultisampled; 
        }

        internal static TextureUploader ConstructUsingPropertiesFromTexture(EngineDevice device, D3D11.Texture2D texture)
        {
            var textureDesc = texture.Description;
            return new TextureUploader(
                device, 
                textureDesc.Width, textureDesc.Height, textureDesc.Format,
                GraphicsHelper.IsMultisampled(textureDesc));
        }

        /// <summary>
        /// Upload a texture from the graphics hardware.
        /// </summary>
        /// <param name="textureToUpload">The texture to be uploaded.</param>
        public MemoryMappedTexture<T> UploadToMemoryMappedTexture<T>(D3D11.Texture2D textureToUpload)
            where T : unmanaged
        {
            if (_isDisposed) { throw new ObjectDisposedException(nameof(TextureUploader)); }

            var result = new MemoryMappedTexture<T>(
                new Size2(_width, _height));
            this.UploadToMemoryMappedTexture(textureToUpload, result);
            return result;
        }

        /// <summary>
        /// Upload a texture from the graphics hardware.
        /// </summary>
        /// <param name="textureToUpload">The texture to be uploaded.</param>
        /// <param name="targetFloatBuffer">The target buffer to which to copy all data.</param>
        public unsafe void UploadToMemoryMappedTexture<T>(D3D11.Texture2D textureToUpload, MemoryMappedTexture<T> targetFloatBuffer) 
            where T : unmanaged
        {
            if (_isDisposed) { throw new ObjectDisposedException(nameof(TextureUploader)); }
            this.EnsureNotNullOrDisposed(nameof(targetFloatBuffer));

            // Check input texture
            var textureDesc = textureToUpload.Description;
            if (textureDesc.Width != _width)
            {
                throw new SeeingSharpGraphicsException($"Invalid texture: Width does not match (given: {textureDesc.Width}, expected: {_width})!");
            }
            if (textureDesc.Height != _height)
            {
                throw new SeeingSharpGraphicsException($"Invalid texture: Height does not match (given: {textureDesc.Height}, expected: {_height})!");
            }
            if (textureDesc.Format != _format)
            {
                throw new SeeingSharpGraphicsException($"Invalid texture: Format does not match (given: {textureDesc.Format}, expected: {_format})!");
            }
            if (GraphicsHelper.IsMultisampled(textureDesc) != _isMultisampled)
            {
                throw new SeeingSharpGraphicsException($"Invalid texture: Multisampling does not match (given: {GraphicsHelper.IsMultisampled(textureDesc)}, expected: {_isMultisampled})!");
            }

            // Check source and destination size
            var targetPixelSize = targetFloatBuffer.PixelSize;
            if (targetPixelSize.Width != _width)
            {
                throw new SeeingSharpGraphicsException("The width of the textures during texture upload does not match!");
            }
            if (targetPixelSize.Height != _height)
            {
                throw new SeeingSharpGraphicsException("The height of the textures during texture upload does not match!");
            }

            // Check format compatibility
            var textureFormatByteSize = DXGI.FormatHelper.SizeOfInBytes(_format);
            if (textureFormatByteSize != sizeof(T))
            {
                throw new SeeingSharpGraphicsException(
                    "Format of the texture to upload and the destination buffer does not match " +
                    $"(source: {_format} / {textureFormatByteSize} bytes, target: {typeof(T).Name} / {sizeof(T)} bytes)!");
            }

            // Upload the texture
            this.CopyTextureToStagingResource(textureToUpload);

            // Read the data into the .Net data block
            var dataBox = _device.DeviceImmediateContextD3D11.MapSubresource(
                _copyHelperTextureStaging, 0, D3D11.MapMode.Read, D3D11.MapFlags.None);
            try
            {
                var rowPitchSource = dataBox.RowPitch;
                var rowPitchDestination = targetFloatBuffer.Width * sizeof(T);
                if ((rowPitchSource > 0) && (rowPitchDestination > 0))
                {
                    for (var loopY = 0; loopY < _height; loopY++)
                    {
                        SeeingSharpUtil.CopyMemory(
                            dataBox.DataPointer + loopY * rowPitchSource,
                            targetFloatBuffer.Pointer + loopY * rowPitchDestination,
                            (uint)rowPitchDestination);
                    }
                }
                else
                {
                    throw new SeeingSharpGraphicsException($"Invalid row pitch (source: {rowPitchSource}, destination: {rowPitchDestination})!");
                }
            }
            finally
            {
                _device.DeviceImmediateContextD3D11.UnmapSubresource(_copyHelperTextureStaging, 0);
            }
        }

        public void Dispose()
        {
            SeeingSharpUtil.SafeDispose(ref _copyHelperTextureStaging);
            SeeingSharpUtil.SafeDispose(ref _copyHelperTextureStandard);
            _isDisposed = true;
        }

        /// <summary>
        /// Loads the target texture int a staging texture.
        /// </summary>
        private void CopyTextureToStagingResource(D3D11.Texture2D textureToUpload)
        {
            // Prepare needed textures
            if (_copyHelperTextureStaging == null)
            {
                _copyHelperTextureStaging = GraphicsHelper.Internals.CreateStagingTexture(_device, _width, _height, _format);
                if (_isMultisampled)
                {
                    _copyHelperTextureStandard = GraphicsHelper.Internals.CreateTexture(_device, _width, _height, _format);
                }
            }

            // Copy contents of the texture
            //  .. execute a ResolveSubresource before if the source texture is multisampled
            if (_isMultisampled)
            {
                _device.DeviceImmediateContextD3D11.ResolveSubresource(textureToUpload, 0, _copyHelperTextureStandard, 0, _format);
                _device.DeviceImmediateContextD3D11.CopyResource(_copyHelperTextureStandard, _copyHelperTextureStaging);
            }
            else
            {
                _device.DeviceImmediateContextD3D11.CopyResource(textureToUpload, _copyHelperTextureStaging);
            }
        }

        /// <inheritdoc />
        public bool IsDisposed => _isDisposed;
    }
}
