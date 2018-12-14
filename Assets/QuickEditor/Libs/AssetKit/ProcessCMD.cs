using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ProcessCMD
{
    public static bool ProcessCommand(string command, string argument)
    {
        EditorUtility.DisplayProgressBar("",command+" "+argument, 1.0f);
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command);
        info.Arguments = argument;
        info.CreateNoWindow = true;
        info.ErrorDialog = true;
        info.UseShellExecute = false;

        if (info.UseShellExecute)
        {
            info.RedirectStandardOutput = false;
            info.RedirectStandardError = false;
            info.RedirectStandardInput = false;
        }
        else
        {
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }

        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);

        string log = process.StandardOutput.ReadToEnd();
        bool error = false;
        if (log.Contains("Error"))
        {
            error = true;
            Debug.LogError(log);
        }
        else
        {
            Debug.Log(log);
        }
        process.WaitForExit();
        process.Close();
        EditorUtility.ClearProgressBar();
        return !error;
    }

}
