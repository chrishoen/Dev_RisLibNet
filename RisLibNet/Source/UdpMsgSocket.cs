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
    // Receive socket class. This receives byte content messages from a socket.

    public class UdpRxMsgSocket
    {
        //**********************************************************************
        //**********************************************************************
        //**********************************************************************
        // Members.

        public BaseMsgMonkey     mMonkey;
        public UdpClient         mUdpClient;
        public IPEndPoint        mIPEndPoint;
        public int               mRxCount;
        public bool              mValidFlag;

        //**********************************************************************
        //**********************************************************************
        //**********************************************************************
        // Configure the socket.

        public void configure(BaseMsgMonkeyCreator aMonkeyCreator,String aAddress, int aPort)
        {
            mMonkey  = aMonkeyCreator.createNew();
            mUdpClient  = new UdpClient(aPort);
            mIPEndPoint = new IPEndPoint(IPAddress.Parse(aAddress), aPort);
            
            Prn.print(Prn.SocketInit2,"UdpRxMsgSocket     $ {0,16} : {1}",mIPEndPoint.Address.ToString(),mIPEndPoint.Port);
        }

        public void close()
        {
            if (mUdpClient != null)
            {
                mUdpClient.Close();
                mUdpClient = null;
            }
        }

        //**********************************************************************
        //**********************************************************************
        //**********************************************************************
        // Receive message from socket.

        public ByteContent receiveMsg()
        {
            //------------------------------------------------------------------
            // Guard

            if (mUdpClient == null) return null;

            //------------------------------------------------------------------
            // Receive bytes from socket

            byte[] tRxBytes = null;

            try
            {
                tRxBytes = mUdpClient.Receive(ref mIPEndPoint);
            }
            catch
            {
                Prn.print(Prn.SocketRun1, "UdpRxSocket Receive EXCEPTION");
                return null;
            }
            //------------------------------------------------------------------
            // Guard.

            if (tRxBytes != null)
            {
                Prn.print(Prn.SocketRun2, "UdpRxSocket rx message {0}",tRxBytes.Length);
            }
            else
            {
                Prn.print(Prn.SocketRun1, "UdpRxSocket ERROR");
                return null;
            }

            //------------------------------------------------------------------
            // Create byte buffer.

            ByteBuffer tBuffer = new ByteBuffer(tRxBytes);
            tBuffer.setCopyFrom();
            tBuffer.setLength(tRxBytes.Length);

            //------------------------------------------------------------------
            // Copy from the receive buffer into the message parser object
            // and validate the header

            mMonkey.extractMessageHeaderParms(tBuffer);

            // If the header is not valid then error
            if (!mMonkey.mHeaderValidFlag)
            {
                Prn.print(Prn.SocketRun1, "UdpRxSocket Receive FAIL INVALID HEADER");
                return null;
            }

            Prn.print(Prn.SocketRun3, "UdpRxSocket Receive Header {0}",mMonkey.mHeaderLength);

            //------------------------------------------------------------------
            // At this point the buffer contains the complete message.
            // Extract the message from the byte buffer into a new message
            // object and return it.

            tBuffer.rewind();
            ByteContent tRxMsg = mMonkey.getMsgFromBuffer(tBuffer);

            if (tRxMsg == null)
            {
                Prn.print(Prn.SocketRun1, "UdpRxSocket FAIL INVALID MESSAGE");
                return null;
            }

            // Returning true  means socket was not closed
            // Returning false means socket was closed
            mRxCount++;
            return tRxMsg;
        }
    }

    //**************************************************************************
    //**************************************************************************
    //**************************************************************************
    // Transmit socket class. This sends byte content messages to a socket.

    public class UdpTxMsgSocket
    {
        //**********************************************************************
        //**********************************************************************
        //**********************************************************************
        // Members

        public BaseMsgMonkey     mMonkey;
        public Socket            mSocket;
        public IPEndPoint        mIPEndPoint;
        public int               mTxMsgCount;
        public bool              mValidFlag;

        //**********************************************************************
        //**********************************************************************
        //**********************************************************************
        // Configure the socket.

        public void configure(BaseMsgMonkeyCreator aMonkeyCreator,String aAddress, int aPort)
        {
            mMonkey     = aMonkeyCreator.createNew();
            mSocket     = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
            mIPEndPoint = new IPEndPoint(IPAddress.Parse(aAddress), aPort);

            Prn.print(Prn.SocketInit2,"UdpTxMsgSocket     $ {0,16} : {1}",mIPEndPoint.Address.ToString(),mIPEndPoint.Port);
        }

        //**********************************************************************
        //**********************************************************************
        //**********************************************************************
        // Send message to the socket.

        public void sendMsg (ByteContent aMsg)
        {
            //------------------------------------------------------------------
            // Create byte buffer.
            ByteBuffer tBuffer = new ByteBuffer(mMonkey.getMaxBufferSize());

            // Copy message to buffer.
            mMonkey.putMsgToBuffer(tBuffer,aMsg);

            //------------------------------------------------------------------
            // Send buffer to socket. 
            byte[] tTxBytes  = tBuffer.getBaseAddress();
            int    tTxLength = tBuffer.getPosition();

            try
            {
                int tSent = mSocket.SendTo(tTxBytes, tTxLength, SocketFlags.None, mIPEndPoint);
                Prn.print(Prn.SocketRun2, "UdpTxSocket tx message {0}",tSent);
            }
            catch
            {
                Prn.print(Prn.SocketRun2, "UdpTxSocket Send ERROR");
            }
        }
    }
}
