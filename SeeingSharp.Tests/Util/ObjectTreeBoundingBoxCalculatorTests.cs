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

using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeeingSharp.Util;

namespace SeeingSharp.Tests.Util
{
    [TestClass]
    public class ObjectTreeBoundingBoxCalculatorTests
    {
        public const string TEST_CATEGORY = "SeeingSharp Util ObjectTreeBoundingBoxCalculator";

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void Test_DefaultUsage()
        {
            var calculator = new ObjectTreeBoundingBoxCalculator(); 
            calculator.AddCoordinate(new Vector3(-1f, 0f, -1f));
            calculator.AddCoordinate(new Vector3(1f, 1f, 1f));

            Assert.IsTrue(calculator.CanCreateBoundingBox);

            var boundingBox = calculator.CreateBoundingBox();

            Assert.IsTrue(boundingBox.Minimum.Equals(new Vector3(-1f, 0f, -1f)));
            Assert.IsTrue(boundingBox.Maximum.Equals(new Vector3(1f, 1f, 1f)));
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void Test_Empty()
        {
            var calculator = new ObjectTreeBoundingBoxCalculator(); 

            Assert.IsFalse(calculator.CanCreateBoundingBox);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void Test_SingleCoordinate()
        {
            var calculator = new ObjectTreeBoundingBoxCalculator(); 
            calculator.AddCoordinate(new Vector3(-1f, 0f, -1f));

            Assert.IsFalse(calculator.CanCreateBoundingBox);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void Test_MoreEqualCoordinates()
        {
            var calculator = new ObjectTreeBoundingBoxCalculator(); 
            calculator.AddCoordinate(new Vector3(-1f, 0f, -1f));
            calculator.AddCoordinate(new Vector3(-1f, 0f, -1f));
            calculator.AddCoordinate(new Vector3(-1f, 0f, -1f));
            calculator.AddCoordinate(new Vector3(-1f, 0f, -1f));

            Assert.IsFalse(calculator.CanCreateBoundingBox);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [ExpectedException(typeof(SeeingSharpException))]
        public void Test_MoreEqualCoordinates_CreateBoxException()
        {
            var calculator = new ObjectTreeBoundingBoxCalculator(); 
            calculator.AddCoordinate(new Vector3(-1f, 0f, -1f));
            calculator.AddCoordinate(new Vector3(-1f, 0f, -1f));
            calculator.AddCoordinate(new Vector3(-1f, 0f, -1f));
            calculator.AddCoordinate(new Vector3(-1f, 0f, -1f));

            Assert.IsFalse(calculator.CanCreateBoundingBox);

            calculator.CreateBoundingBox();
        }
    }
}
