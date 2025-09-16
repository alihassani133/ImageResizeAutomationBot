using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizeAutomationBot
{
    internal class ProgressTracking
    {
        private readonly HashSet<string> _modified;
        private readonly string _path;
        public ProgressTracking(string path)
        {
             _path = path;
            _modified = LoadFile(path);
        }
        public bool IsModified(string partNumber) => _modified.Contains(partNumber);

        public void MarkDone(string partNumber)
        {
            if (string.IsNullOrEmpty(partNumber)) return;

            if (_modified.Add(partNumber))
            {
                File.AppendAllText(_path, partNumber + Environment.NewLine);
            }
        }
        private static HashSet<string> LoadFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "PartNumber\n");
            }

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in File.ReadAllLines(filePath).Skip(1))
            {
                var v = line.Trim();
                if (!string.IsNullOrEmpty(v)) set.Add(v);
            }
            return set;
        }
    }
}
