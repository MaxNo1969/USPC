using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PROTOCOL;
using System.Threading;

namespace USPC.Workers
{
    class AscansReader
    {
        List<ChannelAscansReader> chReaders = new List<ChannelAscansReader>();
        public AscansReader(int _timeout)
        {
            for (int board = 0; board < Program.numBoards; board++)
            {
                for (int channel = 0; channel < Program.channelsOnBoard[board]; channel++)
                {
                    chReaders.Add(new ChannelAscansReader(board, channel, _timeout));
                }
            }
        }
        public void start()
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: Будем запускать {2} потоков", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, chReaders.Count);
            foreach (ChannelAscansReader reader in chReaders)
            {
                if(!reader.IsBusy)reader.RunWorkerAsync();
                while (!reader.IsBusy) Thread.Sleep(10);
            }
        }
        public void stop()
        {
            foreach (ChannelAscansReader reader in chReaders)
            {
                if(reader.IsBusy)reader.CancelAsync();
            }
        }

    }
}
