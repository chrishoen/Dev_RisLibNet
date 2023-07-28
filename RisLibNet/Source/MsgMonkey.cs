using System;
using System.Text;
using System.IO;

namespace Ris
{
    //******************************************************************************
    //******************************************************************************
    //******************************************************************************
    // This is an abstract base class for a message creator. Inheriting classes 
    // are used to create messages for a specific message set.

    public abstract class BaseMsgCreator
    {
       //***************************************************************************
       // Create a new message, based on a message type.

       public abstract ByteContent createMsg (int aMessageType);

    };

    //******************************************************************************
    // This is an abstract base class for a message monkey. It can be used
    // by code that receives messages into byte buffers such that the message
    // classes don't have to be visible to the receiving code. Inheriting classes
    // provide all of the details that are needed by receiving code to receive and
    // extract messages, as opposed to having the message classes being visible
    // to the receiving code. Likewise for the transmitting code.

    public abstract class BaseMsgMonkey
    {
        //***************************************************************************
        //***************************************************************************
        //***************************************************************************
        // Constructors and initialization:

        public BaseMsgMonkey(BaseMsgCreator aCreator)
        {
           mMsgCreator = aCreator;

           mHeaderLength=0;
           mMessageLength=0;
           mMessageType=0;
           mPayloadLength=0;
           mHeaderValidFlag=false;
           mNetworkOrder=false;
        }
   
        //***************************************************************************
        //***************************************************************************
        //***************************************************************************
        // Header processisng:

        // Extract message header parameters from a buffer and validate them
        // Returns true if the header is valid
        public abstract bool extractMessageHeaderParms(ByteBuffer aBuffer);

        // Message header parameters, these are common to all message headers.
        // They are extracted from an actual received message header. In some
        // form, all message headers contain these parameters.

        public int  mHeaderLength;
        public int  mMessageLength;
        public int  mMessageType;
        public int  mPayloadLength;
        public bool mHeaderValidFlag;

        //***************************************************************************
        //***************************************************************************
        //***************************************************************************
        // Message processing:

        // Create a new message based on a message type
        public ByteContent createMessage(int aMessageType)
        {
            return mMsgCreator.createMsg(aMessageType);
        }

        // Preprocess a message before it is sent.
        public abstract void processBeforeSend(ByteContent aMsg);

        //***************************************************************************
        //***************************************************************************
        //***************************************************************************
        // Copy a message to a byte buffer.

        public void putMsgToBuffer (ByteBuffer aBuffer,ByteContent aMsg)
        {
            // Call inheritor's override to preprocess the message before it is sent.
            processBeforeSend(aMsg);

            // Set buffer direction for put.
            aBuffer.setCopyTo();

            // Call inheritor's copier to copy from the message to the buffer.
            aMsg.copyToFrom(aBuffer);
        }

        //***************************************************************************
        //***************************************************************************
        //***************************************************************************
        // Copy a message from a byte buffer
        // The header parms must be extracted prior to calling this.
        //
        // 1) Create a new message object of the type specifed by the identifiers 
        //    that were extracted from the header
        // 2) Copy the data from the byte buffer into the new message object
        // and returns a pointer to the base class.

        public ByteContent getMsgFromBuffer (ByteBuffer aBuffer)
        {
            // Guard.
            if (!mHeaderValidFlag) return null;

            // Call inheritor's creator to create a new message based on the
            // message type that was extracted from the header.
            ByteContent aMsg = mMsgCreator.createMsg(mMessageType);

            // Guard.
            if (aMsg==null) return null;

            // Set buffer direction for get.
            aBuffer.setCopyFrom();

            // Call inheritor's copier to copy from the buffer to the message.
            aMsg.copyToFrom(aBuffer);

            // Done.
            return aMsg;
        }

        //******************************************************************************
        //******************************************************************************
        //******************************************************************************
        // Copy a message from a byte buffer
        //
        // 1) Extract the header parameters.
        // 2) Create a new message object of the type specifed by the identifiers 
        //    that were extracted from the header
        // 3) Copy the data from the byte buffer into the new message object
        // and returns a pointer to the base class.

        public ByteContent makeMsgFromBuffer (ByteBuffer aBuffer)
        {
            // Set buffer direction for get.
            aBuffer.setCopyFrom();

            // Extract the header parameters.
            aBuffer.rewind();
            extractMessageHeaderParms(aBuffer);
            aBuffer.rewind();

            // Guard.
            if (!mHeaderValidFlag) return null;

            // Call inheritor's creator to create a new message based on the
            // message type that was extracted from the header.
            ByteContent aMsg = mMsgCreator.createMsg(mMessageType);

            // Guard
            if (aMsg==null) return null;

            // Call inheritor's copier to copy from the buffer to the message.
            aMsg.copyToFrom(aBuffer);

            // Done.
            return aMsg;
        }

        //***************************************************************************
        //***************************************************************************
        //***************************************************************************
        // Message creator, this must be set by the inheritor.

        public BaseMsgCreator mMsgCreator;

        //***************************************************************************
        //***************************************************************************
        //***************************************************************************
        // Buffer management:


        // Return a constant header length
        public abstract int getHeaderLength();

        // Return a constant max buffer size
        public abstract int getMaxBufferSize();

        // Endianess for buffers associated with the parser.
        // If true then the messages will be sent in network order,
        // big endian. If false, then little endian.
        void setNetworkOrder (bool aNetworkOrder)
        {
            mNetworkOrder = aNetworkOrder;
        }
        public bool mNetworkOrder;

        // Configures a byte buffer endianess
        public void configureByteBuffer (ByteBuffer aBuffer)
        {
           //aBuffer.setNetworkOrder(mNetworkOrder);
        }
    };

    //******************************************************************************
    // This is an abstract base class for a message monkey creator. It defines
    // a method that inheriting classes overload to create new message monkeys.
    // It is used by transmitters and receivers to create new instances of message
    // monkeys.

    public abstract class BaseMsgMonkeyCreator
    {
       public abstract BaseMsgMonkey createNew();
    };
}
