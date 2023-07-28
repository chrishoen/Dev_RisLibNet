using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Ris;

//**************************************************************************
//**************************************************************************
//**************************************************************************

public class MyFunctions
{
    //**********************************************************************
    //**********************************************************************
    //**********************************************************************
    // Initialize

    public static bool IsValidIPAddress(string aIPAddress)
    {
        string[] tSplit = aIPAddress.Split('.');
        if (tSplit.Length != 4) return false;
        for (int i = 0; i < 4; i++)
        {
            int tInt;
            if (!Int32.TryParse(tSplit[i], out tInt)) return false;
            if (tInt < 0 || tInt > 255) return false;
        }

        return true;
    }

    //**************************************************************************
    //**************************************************************************
    //**************************************************************************
};
