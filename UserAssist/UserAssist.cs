// UserAssist 2.6.0
// Didier Stevens 22/07/2006 - 13/06/2012 https://DidierStevens.com
// Small demo tool to decrypt and manipulate the userassist regkey 
// Use at your own risk
//
// History:
//  Version 1.0 22/07/2006
//   The program displays the results with a multi-line TextBox
//  Version 1.1 23/07/2006
//   Replaced the multi-line TextBox with a TreeView
//   Added data display and decoding
//  Version 1.1 24/07/2006
//   Corrected the filetype when saving
//  Version 2.0 1/08/2006
//   Replaced TreeView with ListView
//   Added menu and new commands
//  Version 2.1 3/08/2006
//   Added about dialog box
//  Version 2.1.1 6/08/2006
//   Code cleaning & commenting
//  Version 2.1.2 5/09/2006
//   replaced http with https
//   fixed bug in GetLogging when UserAssist key doesn't exist
//  Version 2.2.0 30/10/2006
//   Added headers to the CSV file created with the Save command
//  Version 2.3.0 15/07/2007 - 16/07/2007
//   Added highlight function
//   Added save as HTML
//   Added new IE7 UserAssist GUID key {0D6D4F41-2994-4BA0-8FEF-620E43CD2812}
//   Added menu command to load registry hive files (DAT files like NTUSER.DAT) directly
//  Version 2.4.0 27/08/2007
//   Added explain option
//  Version 2.4.1 29/08/2007
//   Added Shell GUIDs to explain option
//  Version 2.4.1 13/09/2007 - 14/09/2007
//   Added logic to handle unencrypted entries
//   Added explanation for UEME_UITOOLBAR
//  18/09/2007
//   Added toolbar button IDs for Windows 2003
//  Version 2.4.2 05/11/2007
//   Added toggle to enable/disable loadind of local keys at startup
//  Version 2.4.3 09/08/2009
//   Added lastutc
//  Version 2.5.0 03/10/2009
//   Windows 7
//  Version 2.6.0 13/06/2012
//   Merging pre-Windows 7 and Windows 7 code


using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UserAssist
{

    static class UserAssist
    {
        public const string Name = "UserAssist";
        public const string URL = "https://DidierStevens.com";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmUserAssist());
        }
    }
}