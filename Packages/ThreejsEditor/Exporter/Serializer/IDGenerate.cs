using System;

namespace Piruzhaolu.ThreejsEditor
{
    public static class IDGenerate
    {
        private static readonly DateTime StartTime = new DateTime(2020, 6, 18);
        private static long _lastTime = 0;
        private static int _index = 0;

        public static long Generate()
        {
            var span = DateTime.Now.Subtract(StartTime);
            var totalSeconds = span.TotalSeconds;
            var s = (long) totalSeconds;
            if (_lastTime != s)
            {
                _lastTime = s;
                _index = 0;
            }

            _index++;
            return s * 10000 + _index;
        }


        public static string GenGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
        
    }
}