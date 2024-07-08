﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PROTOCOL;

namespace USPC.Workers
{
    class AscansReader
    {
        List<ChannelReaderThread> chReaders = new List<ChannelReaderThread>();
        public AscansReader(int _timeout)
        {
            for (int board = 0; board < Program.numBoards; board++)
            {
                for (int channel = 0; channel < Program.channelsOnBoard[board]; channel++)
                {
                    chReaders.Add(new ChannelReaderThread(board, channel));
                }
            }
        }
        public void start()
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: Будем запускать {2} потоков", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, chReaders.Count);
            foreach (ChannelReaderThread reader in chReaders)
            {
                reader.start();
            }
        }
        public void stop()
        {
            foreach (ChannelReaderThread reader in chReaders)
            {
                reader.stop();
            }
        }
    }
}
