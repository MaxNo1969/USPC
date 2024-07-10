using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PROTOCOL;

namespace USPC
{
    class BoardParam
    {
        public string name;
        public double val;
        public void set(string _name,double _val)
        {
            name = _name;
            val = _val;
        }
    }
    class Params
    {
        public Dictionary<string, double> paramDict;
        public Params()
        {
            paramDict = new Dictionary<string, double>();
        }

    }
    public class AscanParams
    {
        List<List<Params>> BoardParams;
        public AscanParams()
        {
            BoardParams = new List<List<Params>>();
            for (int board = 0; board < Program.numBoards; board++)
            {
                BoardParams.Add(new List<Params>());
                for(int ch=0;ch<Program.channelsOnBoard[board];ch++)
                    BoardParams[board].Add(new Params());
            }
        }
        string[] paramsNames = 
        {
         "scope_video",
         "scope_zero",
         "gateIF_phase",
         "gate1_phase",
         "gate2_phase",
         "gate1_trigger",
         "gate1_position",
         "gate1_level",
         "gate1_nb_alarm_level",
         "gate2_trigger",
         "gate2_position",
         "gate2_level",
         "gate2_nb_alarm_level",
         "scope_trigger",
         "scope_offset",
         "scope_range",
        };
        public void LoadParamsFromBoard()
        {
            for(int board=0;board<Program.numBoards;board++)
                for(int ch=0;ch<Program.channelsOnBoard[board];ch++)
                    foreach (string s in paramsNames)
                    {
                        double val = Program.pcxus.getParamValueDouble(s,board,ch);
                        if (Program.pcxus.Err == 0)this.set(board, ch, s, val);
                    }
        }
        public void set(int _board, int _channel, string _name, double _val)
        {
            Dictionary<string, double> dict = BoardParams[_board][_channel].paramDict;
            try
            {
                dict.Add(_name, _val);
            }
            catch (ArgumentNullException ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            catch(ArgumentException ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                dict[_name] = _val;
            }
        }
        public double get(int _board, int _channel, string _name)
        {
            Dictionary<string, double> dict = BoardParams[_board][_channel].paramDict;
            double ret = 0;
            if (dict.TryGetValue(_name, out ret))return ret;
            else return ret;
        }            
    }
}
