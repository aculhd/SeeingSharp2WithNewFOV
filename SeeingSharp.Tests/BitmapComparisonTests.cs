﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeeingSharp.Tests.Util;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GDI = System.Drawing;

namespace SeeingSharp.Tests
{
    [TestClass]
    public class BitmapComparisonTests
    {
        public const string TEST_CATEGORY = "SeeingSharp Util BitmapComparison";

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Positive()
        {
            using (GDI.Bitmap leftBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "FlatShadedObject.png"))
            using (GDI.Bitmap rightBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "FlatShadedObject.png"))
            {
                Assert.IsTrue(
                    BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap) == 0f);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Negative_Without_Model()
        {
            using (GDI.Bitmap leftBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "ClearedScreen.png"))
            using (GDI.Bitmap rightBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "FlatShadedObject.png"))
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.IsTrue(comparisonResult > 0.25f);
                Assert.IsTrue(comparisonResult < 0.6f);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Negative_Inversed_Image()
        {
            using (GDI.Bitmap leftBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "FlatShadedObject.png"))
            using (GDI.Bitmap rightBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "FlatShadedObject_Negative.png"))
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.IsTrue(comparisonResult > 0.9f);
                Assert.IsTrue(comparisonResult <= 1.0f);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Negative_BlackWhite()
        {
            using (GDI.Bitmap leftBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "WhiteScreen.png"))
            using (GDI.Bitmap rightBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "BlackScreen.png"))
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.IsTrue(comparisonResult == 1.0f);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Negative_Enlighted()
        {
            using (GDI.Bitmap leftBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "FlatShadedObject.png"))
            using (GDI.Bitmap rightBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "FlatShadedObject_Enlighted.png"))
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.IsTrue(comparisonResult > 0.1);
                Assert.IsTrue(comparisonResult < 0.4);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void BitmapComparison_Negative_Smaller()
        {
            using (GDI.Bitmap leftBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "FlatShadedObject.png"))
            using (GDI.Bitmap rightBitmap = TestUtilities.LoadBitmapFromResource("BitmapComparison", "FlatShadedObject_Smaller.png"))
            {
                float comparisonResult = BitmapComparison.CalculatePercentageDifference(leftBitmap, rightBitmap);
                Assert.IsTrue(comparisonResult > 0.1);
                Assert.IsTrue(comparisonResult < 0.4);
            }
        }
    }
}
