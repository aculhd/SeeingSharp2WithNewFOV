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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using SeeingSharp.Multimedia.Core;

namespace SeeingSharp.ModelViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowVM? m_viewModel;

        public MainWindow()
        {
            this.InitializeComponent();

            // Initialize Viewmodel
            if (GraphicsCore.IsLoaded)
            {
                m_viewModel = new MainWindowVM(this.CtrlRenderer.RenderLoop);
                m_viewModel.OpenFileDialogRequest += this.OnViewModelOpenFileDialogRequest;
                this.DataContext = m_viewModel;

                this.Loaded += this.OnLoaded;
            }
        }

        private void OnViewModelOpenFileDialogRequest(object? sender, OpenFileDialogEventArgs e)
        {
            var dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = e.FilterString;
            if (true == dlgOpenFile.ShowDialog(this))
            {
                e.SelectedFile = dlgOpenFile.FileName;
            }
            else
            {
                e.SelectedFile = string.Empty;
            }
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (m_viewModel == null) { return; }

            await m_viewModel.LoadInitialScene();
        }
    }
}
