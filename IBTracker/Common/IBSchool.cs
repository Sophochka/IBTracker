using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBTracker.Common
{
    public class IBSchool
    {
        public string Name { get; set; }

        public Uri Site { get; set; }

        // Primary Years Programme
        public bool PYP { get; set; }

        // Middle Years Programme
        public bool MYP { get; set; }

        // Diploma Programme
        public bool DP { get; set; }

        // Career-related Programme
        public bool CP { get; set; }

        public bool AllProgramm => PYP && MYP && DP && CP;

        public string Languages { get; set; }
    }
}
