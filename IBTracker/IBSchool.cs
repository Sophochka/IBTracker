using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBOTracker
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

        public ICollection<string> Languages { get; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public void SetLanguages(string text)
        {
            var list = text.Split("\r\n").Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)).ToList();
            Languages.Clear();
            list.ForEach(l => Languages.Add(l));
        }
    }
}
