using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HLab.Erp.Lims.Analysis.Data
{
    public class LanguageParser
    {
        private readonly Dictionary<string, string> _dict = new Dictionary<string, string>();
        public LanguageParser(string source)
        {
            var regex = new Regex("{(.*?)}");
            var matches = regex.Matches(source);
            foreach (Match match in matches)
            {
                var v = match.Groups[1].Value.Split('=');
                if(v.Length>1)
                    _dict.Add(v[0], v[1]);
                else if (v.Length == 1)
                    _dict.Add("FR",v[0]);
            }

            if(_dict.Count==0)
                _dict.Add("FR",source);
        }

        public string this[string lng]
        {
            get
            {
                try

                {
                    return _dict[lng];
                }
                catch (KeyNotFoundException)
                {
                    return _dict.Values.FirstOrDefault();
                }
            }
        }
    }
    public class FormParametersParser
    {
        private readonly Dictionary<string, string> _dict = new Dictionary<string, string>();
        public FormParametersParser(string source)
        {
            source.Split('■').Select(e => e.Split('=')).ToList().ForEach(e =>
            {
                if(e.Length==2)
                    _dict.Add(e[0],e[1]);
            });
        }

        public T Get<T>(string n)
        {
            try
            {
                var value = _dict[n];
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.String:
                        return (T)Convert.ChangeType(value, typeof(T));
                    case TypeCode.Double:
                        return (T)Convert.ChangeType(double.Parse(value), typeof(T));
                }
            }
            catch { }

            return default(T);
        }
        public void Set<T>(string n, T value)
        {
            try
            {
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.String:
                        _dict[n] = (string)Convert.ChangeType(value, typeof(string));
                        break;
                    case TypeCode.Double:
                        _dict[n] = ((double)Convert.ChangeType(value, typeof(double))).ToString(CultureInfo.CurrentCulture);
                        break;
                }
            }
            catch { }
        }

        public void ForEach(Action<string> action)
        {
            foreach (var v in _dict.Keys)
            {
                action(v);
            }
        }

        public string Values
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (var v in _dict)
                {
                    sb.Append(v.Key);
                    sb.Append("=");
                    sb.Append(v.Value);
                    sb.Append('■');
                }

                return sb.ToString();
            }
        }

    }
}
