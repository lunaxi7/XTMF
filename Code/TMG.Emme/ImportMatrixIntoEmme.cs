﻿/*
    Copyright 2014-2016 Travel Modelling Group, Department of Civil Engineering, University of Toronto

    This file is part of XTMF.

    XTMF is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    XTMF is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with XTMF.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMG.Input;
using XTMF;

namespace TMG.Emme
{
    public class ImportMatrixIntoEmme : IEmmeTool
    {
        [SubModelInformation(Description= "Demand Matrix File", Required= true)]
        public FileLocation MatrixFile;

        [RunParameter("Scenario", 0, "The number of the Emme scenario")]
        public int ScenarioNumber;

        private const string _ToolName = "tmg.XTMF_internal.import_matrix_batch_file";
        private const string _OldToolName = "TMG2.XTMF.ImportMatrix";

        private static Tuple<byte, byte, byte> _ProgressColour = new Tuple<byte, byte, byte>(100, 100, 150);

        public bool Execute(Controller controller)
        {
            var mc = controller as ModellerController;
            if (mc == null)
                throw new XTMFRuntimeException("Controller is not a ModellerController!");
            var pathToUse = Path.GetFullPath(this.MatrixFile.GetFilePath());
            var args = string.Join(" ", "\""+ pathToUse + "\"",
                                        this.ScenarioNumber);

            Console.WriteLine("Importing matrix into scenario " + this.ScenarioNumber.ToString() + " from file " + pathToUse);

            var result = "";
            if(mc.CheckToolExists(_ToolName))
            {
                return mc.Run(_ToolName, args, (p => this.Progress = p), ref result);
            }
            else
            {
                return mc.Run(_OldToolName, args, (p => this.Progress = p), ref result);
            }
        }

        public string Name
        {
            get;
            set;
        }

        public float Progress
        {
            get;
            set;
        }

        public Tuple<byte, byte, byte> ProgressColour
        {
            get { return _ProgressColour; }
        }

        public bool RuntimeValidation(ref string error)
        {
            return true;
        }

    }
}
