using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaCache_v3
{
    class ValueVrapper
    {
        //"workers","list",0,100,"string",0,255,0)

        String _Caption;
        String _datatyp;
        double _min, _max;
        string _Def;

        public ValueVrapper(String Caption, String datatyp, double min, double max, string Def)
        {
            _Caption = Caption;
            _datatyp = datatyp;
            _min = min;
            _max = max;
            _Def = Def; 
        }
        public String toValueString()
        {
            return "\"" + _datatyp + "\"," + _min + "," + _max + "," + _Def;
        } 
        public String toValuevsCaptionString()
        {
            return "\"" + _Caption + "\"," + "\"" + _datatyp + "\"," + _min + "," + _max + "," + _Def;
        }
    }

}
