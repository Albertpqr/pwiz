﻿/*
 * Original author: Henry Sanford <henrytsanford .at. u.washington.edu>,
 *                  MacCoss Lab, Department of Genome Sciences, UW
 *
 * Copyright 2020 University of Washington - Seattle, WA
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pwiz.Common.SystemUtil;
using pwiz.Skyline.Controls.Graphs;
using pwiz.Skyline.Model;
using pwiz.SkylineTestUtil;

namespace pwiz.SkylineTestFunctional
{
    [TestClass]
    public class PeakAreaRelativeAbundanceGraphTest : AbstractFunctionalTest
    {
        [TestMethod]
        public void TestPeakAreaRelativeAbundanceGraph()
        {
            TestFilesZip = @"TestFunctional\PeakAreaRelativeAbundanceGraphTest.zip";
            RunFunctionalTest();
        }

        protected override void DoTest()
        {
            RunUI(() => SkylineWindow.OpenFile(TestFilesDir.GetTestPath("Rat_plasma.sky")));
            WaitForDocumentLoaded();
            RunUI(() =>
            {
                SkylineWindow.SelectedPath = SkylineWindow.Document.GetPathTo((int)SrmDocument.Level.Molecules, 0);
                SkylineWindow.ShowPeakAreaRelativeAbundanceGraph();
                var peakAreaGraph = FormUtil.OpenForms.OfType<GraphSummary>().FirstOrDefault(graph =>
                    graph.Type == GraphTypeSummary.abundance && graph.Controller is AreaGraphController);
                Assert.IsNotNull(peakAreaGraph);

                // Verify that setting the targets to Proteins or Peptides produces the correct number of points
                SkylineWindow.SetAreaProteinTargets(true);
                var curveList = peakAreaGraph.GraphControl.GraphPane.CurveList[1];
                Assert.AreEqual(curveList.Points.Count, 48);
                SkylineWindow.SetAreaProteinTargets(false);
                curveList = peakAreaGraph.GraphControl.GraphPane.CurveList[1];
                Assert.AreEqual(curveList.Points.Count, 125);

                // Verify that excluding peptide lists reduces the number of points
                SkylineWindow.SetExcludePeptideListsFromAbundanceGraph(true);
                curveList = peakAreaGraph.GraphControl.GraphPane.CurveList[1];
                Assert.AreEqual(curveList.Points.Count, 45);

                //CONSIDER add quantitative checks for relative abundance results
            });

        }
    }
}
