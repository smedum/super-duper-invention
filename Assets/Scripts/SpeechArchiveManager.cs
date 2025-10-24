using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class SpeechArchiveManager : MonoBehaviour
{
    [Header("UI References")]
    public Text displayText;
    
    [Header("Archive Settings")]
    [Tooltip("Maximum number of archived files to keep")]
    public int maxArchivedFiles = 100;
    
    [Tooltip("Maximum file size in MB")]
    public float maxFileSizeMB = 10f;
    
    [Tooltip("Enable automatic file rotation")]
    public bool enableFileRotation = true;
    
    // Private fields
    private string archiveFolderPath;
    private readonly object _lock = new object();
    private Queue<string> uiMessageQueue = new Queue<string>();
    private List<byte[]> backupMemoryBuffer = new List<byte[]>();
    private CancellationTokenSource cancellationTokenSource;
    
    // Constants
    private const int MAX_UI_QUEUE_SIZE = 50;
    private const int MAX_FILENAME_LENGTH = 200;
    private const int MAX_RETRY_ATTEMPTS = 3;
    private const int RETRY_DELAY_MS = 500;
    private const int MAX_BACKUP_BUFFER_SIZE = 50;
    private const long MAX_BACKUP_TOTAL_SIZE = 100 * 1024 * 1024; // 100MB
    
    void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
        InitializeArchive();
        StartCoroutine(ProcessUIQueue());
    }
    
    void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }
    
    private void InitializeArchive()
    {
        try
        {
            string basePath = Application.persistentDataPath;
            if (string.IsNullOrEmpty(basePath)) return;
            
            lock (_lock)
            {
                archiveFolderPath = Path.Combine(basePath, "SpeechArchive");
            }
            
            if (!Directory.Exists(archiveFolderPath))
            {
                Directory.CreateDirectory(archiveFolderPath);
            }
            
            UpdateUI($"📁 Archive Ready!\nPath: {archiveFolderPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Archive init failed: {e.Message}");
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
    
    public void SaveRecording(byte[] audioData, string filename)
    {
        _ = SaveRecordingAsync(audioData, filename, cancellationTokenSource.Token);
    }
    
    public async Task<bool> SaveRecordingAsync(byte[] audioData, string filename, CancellationToken cancellationToken = default)
    {
        if (audioData == null || audioData.Length == 0) return false;
        
        float fileSizeMB = audioData.Length / (1024f * 1024f);
        if (fileSizeMB > maxFileSizeMB) return false;
        
        filename = ValidateAndSanitizeFilename(filename);
        if (string.IsNullOrEmpty(filename)) return false;
        
        string filePath;
        lock (_lock)
        {
            filePath = Path.Combine(archiveFolderPath, filename);
        }
        
        for (int attempt = 1; attempt <= MAX_RETRY_ATTEMPTS; attempt++)
        {
            try
            {
                await Task.Run(() => File.WriteAllBytes(filePath, audioData), cancellationToken);
                UpdateUI($"✅ Saved: {filename}\nSize: {fileSizeMB:F2}MB");
                
                if (enableFileRotation)
                    await RotateFilesAsync(cancellationToken);
                
                return true;
            }
            catch (Exception e)
            {
                if (attempt < MAX_RETRY_ATTEMPTS)
                    await Task.Delay(RETRY_DELAY_MS * attempt, cancellationToken);
                else
                    AddToBackupBuffer(audioData);
            }
        }
        return false;
    }
    
    private string ValidateAndSanitizeFilename(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            filename = $"speech_{DateTime.Now:yyyyMMdd_HHmmss}.dat";
        
        foreach (char c in Path.GetInvalidFileNameChars())
            filename = filename.Replace(c, '_');
        
        if (!filename.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
            filename += ".dat";
        
        if (filename.Length > MAX_FILENAME_LENGTH)
        {
            string extension = Path.GetExtension(filename);
            filename = filename.Substring(0, MAX_FILENAME_LENGTH - extension.Length) + extension;
        }
        
        return filename;
    }
    
    private async Task RotateFilesAsync(CancellationToken cancellationToken)
    {
        try
        {
            string[] files;
            lock (_lock)
            {
                files = Directory.GetFiles(archiveFolderPath, "*.dat");
            }
            
            if (files.Length <= maxArchivedFiles) return;
            
            var sortedFiles = files.Select(f => new FileInfo(f))
                                 .OrderBy(fi => fi.CreationTime)
                                 .Select(fi => fi.FullName)
                                 .ToList();
            
            int filesToDelete = sortedFiles.Count - maxArchivedFiles;
            for (int i = 0; i < filesToDelete; i++)
            {
                await Task.Run(() => File.Delete(sortedFiles[i]), cancellationToken);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"File rotation error: {e.Message}");
        }
    }
    
    private void AddToBackupBuffer(byte[] data)
    {
        lock (_lock)
        {
            if (backupMemoryBuffer.Count >= MAX_BACKUP_BUFFER_SIZE)
                backupMemoryBuffer.RemoveAt(0);
            
            long currentTotalSize = backupMemoryBuffer.Sum(arr => arr.Length);
            while (currentTotalSize + data.Length > MAX_BACKUP_TOTAL_SIZE && backupMemoryBuffer.Count > 0)
            {
                var removed = backupMemoryBuffer[0];
                backupMemoryBuffer.RemoveAt(0);
                currentTotalSize -= removed.Length;
            }
            
            backupMemoryBuffer.Add(data);
        }
    }
    
    private void UpdateUI(string message)
    {
        lock (_lock)
        {
            if (uiMessageQueue.Count >= MAX_UI_QUEUE_SIZE)
                uiMessageQueue.Dequeue();
            uiMessageQueue.Enqueue($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
    }
    
    private IEnumerator ProcessUIQueue()
    {
        while (displayText != null && displayText.isActiveAndEnabled)
        {
            string message = null;
            lock (_lock)
            {
                if (uiMessageQueue.Count > 0)
                    message = uiMessageQueue.Dequeue();
            }
            
            if (message != null)
                displayText.text = message;
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void TestSaveFunction()
    {
        byte[] testData = new byte[1024];
        new System.Random().NextBytes(testData);
        SaveRecording(testData, $"test_{DateTime.Now:yyyyMMdd_HHmmss}.dat");
    }
    
    public string GetArchiveStats()
    {
        try
        {
            lock (_lock)
            {
                var files = Directory.GetFiles(archiveFolderPath, "*.dat");
                long totalSize = files.Sum(f => new FileInfo(f).Length);
                float totalSizeMB = totalSize / (1024f * 1024f);
                
                return $"Files: {files.Length}/{maxArchivedFiles}\nSize: {totalSizeMB:F2}MB";
            }
        }
        catch (Exception e)
        {
            return $"Error: {e.Message}";
        }
    }
}
