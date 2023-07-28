using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Ris;

//**************************************************************************
//**************************************************************************
//**************************************************************************

public class Prn
{
    //**********************************************************************
    //**********************************************************************
    //**********************************************************************
    // Members

    public static bool mSuppressFlag = true;

    public enum PrnMode { Console, WinForm }
    public static PrnMode mPrnMode = PrnMode.Console;

    public static UdpTxStringSocket mTxSocket;

    //**********************************************************************
    //**********************************************************************
    //**********************************************************************
    // Initialize

    public static void initializeForConsole()
    {
        mPrnMode = PrnMode.Console;
        PrintSettings.initialize();
        mSuppressFlag = false;
    }

    public static void initializeForWinForm()
    {
        mPrnMode = PrnMode.WinForm;
        PrintSettings.initialize();
        mTxSocket = new UdpTxStringSocket();
        mTxSocket.configure(PortDef.cPrintView);
        mSuppressFlag = false;
    }

    //**********************************************************************
    //**********************************************************************
    //**********************************************************************
    // Access

    public static void setFilter(int aFilter, bool aEnablePrint)
    {
        PrintSettings.setFilter(aFilter, aEnablePrint);
    }

    public static void Suppress()
    {
        mSuppressFlag = true;
    }

    public static void UnSuppress()
    {
        mSuppressFlag = false;
    }

    public static void ToggleSuppress()
    {
        mSuppressFlag = !mSuppressFlag;
    }

    //**********************************************************************
    //**********************************************************************
    //**********************************************************************
    // Print

    public static void print(int aFilter, string aFormat, params object[] aObjects)
    {
        // Test for print enabled
        if (mSuppressFlag == true && aFilter != 0) return;
        if (PrintSettings.mFilterTable[aFilter] == false) return;

        if (mPrnMode == PrnMode.Console)
        {
            // Print to console
            Console.WriteLine(aFormat, aObjects);
        }
        else
        {
            // Print to PrintView
            String tString = string.Format(aFormat, aObjects);
            mTxSocket.sendString(tString);
        }
    }

    ~Prn()
    {
    }

    //**********************************************************************
    //**********************************************************************
    //**********************************************************************
    // Filter constants

    public const int  FilterZero = 0;
    public const int  PrintInit1 = 1;
    public const int  PrintInit2 = 2;
    public const int  PrintInit3 = 3;
    public const int  PrintInit4 = 4;
    public const int  PrintRun1 = 5;
    public const int  PrintRun2 = 6;
    public const int  PrintRun3 = 7;
    public const int  PrintRun4 = 8;
    public const int  SocketInit1 = 9;
    public const int  SocketInit2 = 10;
    public const int  SocketInit3 = 11;
    public const int  SocketInit4 = 12;
    public const int  SocketRun1 = 13;
    public const int  SocketRun2 = 14;
    public const int  SocketRun3 = 15;
    public const int  SocketRun4 = 16;
    public const int  ThreadInit1 = 33;
    public const int  ThreadInit2 = 34;
    public const int  ThreadInit3 = 35;
    public const int  ThreadInit4 = 36;
    public const int  ThreadRun1 = 37;
    public const int  ThreadRun2 = 38;
    public const int  ThreadRun3 = 39;
    public const int  ThreadRun4 = 40;
    public const int  ProcInit1 = 41;
    public const int  ProcInit2 = 42;
    public const int  ProcInit3 = 43;
    public const int  ProcInit4 = 44;
    public const int  ProcRun1 = 45;
    public const int  ProcRun2 = 46;
    public const int  ProcRun3 = 47;
    public const int  ProcRun4 = 48;


    public const int View01 = 101;
    public const int View02 = 102;
    public const int View03 = 103;
    public const int View04 = 104;
    public const int View11 = 105;
    public const int View12 = 106;
    public const int View13 = 107;
    public const int View14 = 108;
    public const int View21 = 109;
    public const int View22 = 110;
    public const int View23 = 111;
    public const int View24 = 112;

    };
