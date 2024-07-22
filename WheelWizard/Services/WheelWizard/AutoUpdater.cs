﻿using CT_MKWII_WPF.Helpers;
using CT_MKWII_WPF.Models;
using CT_MKWII_WPF.Models.Github;
using CT_MKWII_WPF.Views.Pages.Popups;
using Semver;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CT_MKWII_WPF.Services.WheelWizard;

public static class AutoUpdater
{
    public const string CurrentVersion = "1.1.2";
    private const string GithubApiUrl = "https://api.github.com/repos/patchzyy/WheelWizard/releases/latest";
    
    
    public static async Task CheckForUpdatesAsync()
    {
        var response = await HttpClientHelper.GetAsync<string>(GithubApiUrl);
        if (!response.Succeeded || response.Content is null)
        {
            HandleUpdateCheckError(response);
            return;
        }
        var latestRelease = JsonSerializer.Deserialize<GithubRelease>(response.Content);
        if (latestRelease?.TagName is null) return;
        
        var currentVersion = SemVersion.Parse(CurrentVersion, SemVersionStyles.Any);
        var latestVersion = SemVersion.Parse(latestRelease.TagName.TrimStart('v'), SemVersionStyles.Any);

        if (currentVersion.ComparePrecedenceTo(latestVersion) >= 0) return;
        if (IsAdministrator())
        {
            await UpdateAsync(latestRelease.Assets[0].BrowserDownloadUrl);
            return;
        }
        
        var initiatedUpdate = YesNoMessagebox.Show("Wheel Wizard Update!", "Update now!", "Maybe Later!", currentVersion.ToString() + "->" + latestVersion.ToString());
        if (!initiatedUpdate) return;
        var adminResult = YesNoMessagebox.Show("Update as Admin?", "Yes", "No");
        //     "Update Method", MessageBoxButtons.YesNo);
        if (adminResult)
        {
            RestartAsAdmin();
        }
        else
        {
            await UpdateAsync(latestRelease.Assets[0].BrowserDownloadUrl);
        }
    }
    
    private static void RestartAsAdmin()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = true;
        startInfo.WorkingDirectory = Environment.CurrentDirectory;
        startInfo.FileName = GetActualExecutablePath();
        startInfo.Verb = "runas";
        try
        {
            Process.Start(startInfo);
            Environment.Exit(0);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            MessageBox.Show("Failed to restart with administrator rights.");
        }
    }
    
    private static string GetActualExecutablePath()
    {
        using (var process = Process.GetCurrentProcess())
        {
            return process.MainModule.FileName;
        }
    }
    
    private static bool IsAdministrator()
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    private static async Task UpdateAsync(string downloadUrl)
    {
        var currentExecutablePath = Process.GetCurrentProcess().MainModule!.FileName;
        var currentFolder = Path.GetDirectoryName(currentExecutablePath);
        if (currentFolder is null)
        {
            MessageBox.Show("Unable to update WheelWizard. " +
                            "Please ensure the application is located in a folder that can be written to.");
            return;
        }
        var newFilePath = Path.Combine(currentFolder, "CT-MKWII-WPF_new.exe");

        await DownloadHelper.DownloadToLocation(downloadUrl, newFilePath);

        // we need to wait a bit before running the batch file to ensure the file is saved on disk
        await Task.Delay(200);
        CreateAndRunBatchFile(currentExecutablePath, newFilePath);

        Environment.Exit(0);
    }

    private static void CreateAndRunBatchFile(string currentFilePath, string newFilePath)
    {
        var batchFilePath = Path.Combine(Path.GetDirectoryName(currentFilePath), "update.bat");
        var originalFileName = Path.GetFileName(currentFilePath);
        var newFileName = Path.GetFileName(newFilePath);

        var batchContent = @"
@echo off
timeout /t 2 /nobreak
del """ + originalFileName + @"""
rename """ + newFileName + @""" """ + originalFileName + @"""
start """" """ + originalFileName + @"""
del ""%~f0""
";

        File.WriteAllText(batchFilePath, batchContent);

        Process.Start(new ProcessStartInfo(batchFilePath) { CreateNoWindow = true });
    }
    
    private static void HandleUpdateCheckError(HttpClientResult<string> response)
    {
        if (response.StatusCodeGroup == 4 || response.StatusCode is 503 or 504)
        {
            MessageBox.Show("Unable to check if WheelWizard is up to date. " +
                            "\nYou might be experiencing network issues.");
        }
        else
        {
            MessageBox.Show("An error occurred while checking for updates. Please try again later. " +
                            "\nError: " + response.StatusMessage);
        }
    }
    
    
}
