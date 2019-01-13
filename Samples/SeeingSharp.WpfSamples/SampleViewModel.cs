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
using SeeingSharp.SampleContainer;
using SeeingSharp.SampleContainer.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SeeingSharp.WpfSamples
{
    public class SampleViewModel : PropertyChangedBase
    {
        private SampleMetadata m_sample;
        private BitmapSource m_bitmapSource;
        private Task m_bitmapSourceTask;

        public SampleViewModel(SampleMetadata sample)
        {
            m_sample = sample;
        }

        public SampleMetadata SampleMetadata => m_sample;

        public string Name => m_sample.Name;

        public string Group => m_sample.Group;

        public BitmapSource BitmapSource
        {
            get
            {
                if((m_bitmapSource == null) && (m_bitmapSourceTask == null))
                {
                    var sourceLink = m_sample.TryGetSampleImageLink();
                    if(sourceLink == null) { return null; }

                    m_bitmapSourceTask = Task.Run(() =>
                    {
                        BitmapImage source = new BitmapImage();
                        source.BeginInit();
                        source.StreamSource = sourceLink.OpenRead();
                        source.EndInit();
                        source.Freeze();

                        m_bitmapSource = source;
                    }).ContinueWith(
                        (task) => RaisePropertyChanged(nameof(BitmapSource)),
                        TaskScheduler.FromCurrentSynchronizationContext());
                }
                return m_bitmapSource;
            }
        }
    }
}