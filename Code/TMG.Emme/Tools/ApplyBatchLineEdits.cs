﻿/*
    Copyright 2015 Travel Modelling Group, Department of Civil Engineering, University of Toronto

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
using System.Linq;
using System.Text;
using TMG.Input;
using XTMF;
namespace TMG.Emme.Tools
{

    public class ApplyBatchLineEdits : IEmmeTool
    {
        private const string ToolNamespace = "tmg.XTMF_internal.apply_batch_line_edits";

        [RunParameter("Scenario Number", 0, "The EMME scenario number to target.")]
        public int ScenarioNumber;

        [SubModelInformation(Required = true, Description = "The batch line edit file to apply.")]
        public FileLocation InputFile;

        public string Name { get; set; }

        public float Progress { get; set; }

        public Tuple<byte, byte, byte> ProgressColour { get { return new Tuple<byte, byte, byte>(50, 150, 50); } }

        public bool Execute(Controller controller)
        {
            var modeller = controller as ModellerController;
            if(modeller == null)
            {
                throw new XTMFRuntimeException("In '" + Name + "' the controller was not for modeller!");
            }
            return modeller.Run(ToolNamespace, GetArguments());
        }

        private string GetArguments()
        {
            return string.Format("{0} \"{1}\"", ScenarioNumber, InputFile.GetFilePath());
        }

        public bool RuntimeValidation(ref string error)
        {
            if(ScenarioNumber <= 0)
            {
                error = "The scenario number '" + ScenarioNumber 
                    + "' is an invalid scenario number!";
                return false;
            }
            return true;
        }
    }

}