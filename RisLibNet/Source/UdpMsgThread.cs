using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Ris
{
    //**************************************************************************
    //**************************************************************************
    //**************************************************************************

    public abstract class UdpMsgThread
    {
        //**********************************************************************
        // Inheriting classes override this method to process received messages

        public abstract void processRxMsg (ByteContent aRxMsg);

        //**********************************************************************
        // Members

        public Thread            mThread;
        public UdpRxMsgSocket    mRxSocket;
        public UdpTxMsgSocket    mTxSocket;
        public int               mRxCount;

        public void configure(
            BaseMsgMonkeyCreator aMonkeyCreator,
            String aRxAddress, 
            int aRxPort, 
            String aTxAddress, 
            int aTxPort)
        {
            // Rx socket
            mRxSocket = new UdpRxMsgSocket();
            mRxSocket.configure(aMonkeyCreator,aRxAddress,aRxPort);

            // Tx socket
            mTxSocket = new UdpTxMsgSocket();
            mTxSocket.configure(aMonkeyCreator,aTxAddress,aTxPort);
        }

        //**********************************************************************
        // Launch thread

        public void start()
        {
            // Create new thread object using thread run function
            mThread = new Thread(new ThreadStart(threadRun));
            // Start the thread
            mThread.Start();
        }

        //**********************************************************************
        // Shutdown thread

        public void stop()
        {
            if (mRxSocket != null)
            {
                mRxSocket.close();
            }
        }

        //**********************************************************************
        //**********************************************************************
        //**********************************************************************
        // Thread run function, receives messages

        public void threadRun()
        {
            while (true)
            {
                //--------------------------------------------------------------
                // Receive from the message socket

                ByteContent tMsg = mRxSocket.receiveMsg();

                //--------------------------------------------------------------
                // Call inheritor's override to process the message

                if (tMsg != null)
                {
                    processRxMsg(tMsg);
                }
                else
                {
                    return;
                }
            }
        }

        //**********************************************************************
        //**********************************************************************
        //**********************************************************************
        // Transmit message

        public void sendMsg(ByteContent aTxMsg)
        {
            mTxSocket.sendMsg(aTxMsg);
        }
    }
}
