using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using NAudio.Wave;
using PrettyLogSharp;
using RiffViewer.Lib.Reader;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using RiffViewer.Lib.Riff.Formats.Wav;
using RiffViewer.Lib.Writer;
using RiffViewer.UI.ViewModels;
using static PrettyLogSharp.PrettyLogger;
using Path = System.IO.Path;
using RiffChunk = RiffViewer.Lib.Riff.Chunk.RiffChunk;

namespace RiffViewer.UI.Views;

public partial class MainWindow : Window
{
    private IChunk? _currentDetailChunk = null;
    private bool _binaryDetail = false;
    private WavePlot? _plotWindow;
    
    private WaveOutEvent? _outputDevice;
    private AudioFileReader? _audioFile;

    private const string TempDir = "./temp_rv";
    
    public MainWindow()
    {
        Settings.TryLoad();
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        if (string.IsNullOrWhiteSpace(Settings.Instance.LastOpenedFilePath))
        {
            return;
        }
        
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Playback.IsVisible = false;
        }

        if (!Directory.Exists(TempDir))
        {
            var dir = Directory.CreateDirectory(TempDir);
            dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden; 
        }
        else
        {
            RemoveTempFiles();
        }

        IRiffFile parsedFile;
        try
        {
            parsedFile = new RiffReader(Settings.Instance.LastOpenedFilePath, false).ReadFile();
        }
        catch (Exception exception)
        {
            Settings.Instance.LastOpenedFilePath = string.Empty;
            Log(exception);
            return;
        }

        SetContextFile(parsedFile);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (DataContext is not MainViewModel context)
        {
            Log("Data context was null", LogType.Warning);
            return;
        }

        UpdateRiffFileTree(context);

        base.OnDataContextChanged(e);
    }

    void UpdateRiffFileTree(MainViewModel context)
    {
        Log("Updating RIFF file tree");
        if (context.RiffFile == null)
        {
            Log("Riff file was null, hiding expander");
            Root.IsVisible = false;
            return;
        }

        ShowWaveform.IsEnabled = Playback.IsEnabled = context.RiffFile is WavFile;
        
        Title = $"Riff Viewer - {Path.GetFileName(context.RiffFile.Path)}";
        
        CreateTree(Root, context.RiffFile);
    }

    private void CreateTree(Expander expander, IRiffFile file)
    {
        OpenFileButton.IsVisible = false;

        expander.Header = CreateRiffChunkHeader(file.MainChunk);
        expander.Padding = new Thickness(10);
        expander.IsVisible = true;
        expander.IsExpanded = true;

        var stackPanel = new StackPanel
        {
            Spacing = 10,
        };

        foreach (var child in file.MainChunk.ChildChunks)
        {
            stackPanel.Children.Add(CreateNode(child));
        }

        expander.Content = stackPanel;
        UpdateDetail(@$"
        Current file: {Path.GetFileName(file.Path)}
        Format: {file.Format}
        Total Size: {file.MainChunk.Length.ToString("N0", new CultureInfo("cs-CZ"))}
        ");
    }

    private Control CreateNode(IChunk chunk)
    {
        Control control;

        if (chunk is ListChunk listChunk)
        {
            var expander = new Expander
            {
                Padding = new Thickness(10),
                Header = CreateRiffChunkHeader(chunk),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                IsEnabled = true,
                Margin = new Thickness(50, 0, 0, 0)
            };

            var rootStackPanel = new StackPanel
            {
                Spacing = 10,
            };

            foreach (var child in listChunk.ChildChunks)
            {
                rootStackPanel.Children.Add(CreateNode(child));
            }

            expander.Content = rootStackPanel;
            control = expander;
        }
        else
        {
            var border = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5),
                Child = CreateRiffChunkHeader(chunk),
                Margin = new Thickness(50, 0, 0, 0)
            };

            control = border;
        }

        control.ContextMenu = CreateContextMenuForNode(chunk);

        return control;
    }

    private ContextMenu CreateContextMenuForNode(IChunk chunk)
    {
        var menu = new ContextMenu();
        
        var removeMenuItem = new MenuItem
        {
            Header = "Remove"
        };
        removeMenuItem.Click += (_, _) => RemoveChunk(chunk.GetChunkPath());
        menu.Items.Add(removeMenuItem);
        
        var extractMenuItem = new MenuItem
        {
            Header = "Extract"
        };
        extractMenuItem.Click += async (_, _) => await ExtractChunk(chunk.GetChunkPath());
        menu.Items.Add(extractMenuItem);

        if (chunk is FmtChunk fmtChunk)
        {
            var editMenuItem = new MenuItem()
            {
                Header = "Edit"
            };

            if (GetDataContext()?.RiffFile is not WavFile wavFile)
            {
                throw new ArgumentException("RiffFile is not a valid WavFile");
            }

            editMenuItem.Click += (_, _) =>
            {
                var fmtChunkEditWindow = new FmtChunkEdit(fmtChunk, wavFile);
                fmtChunkEditWindow.Closed += (_, _) => UpdateRiffFileTree(GetDataContext()!);
                fmtChunkEditWindow.Show(this);
            };
            menu.Items.Add(editMenuItem);
        }

        return menu;
    }

    private void RemoveChunk(string chunkPath)
    {
        var context = GetDataContext();
        if (context?.RiffFile == null)
        {
            return;
        }

        context.RiffFile.RemoveChunk(chunkPath);
        UpdateRiffFileTree(context);
    }

    private async Task ExtractChunk(string chunkPath)
    {
        var context = GetDataContext();
        if (context?.RiffFile == null)
        {
            return;
        }
        
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = GetTopLevel(this);

        if (topLevel == null)
        {
            return;
        }
        
        // Start async operation to open the dialog.
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export RIFF Chunk"
        });
        
        if (file == null)
        {
            Log("File was not not selected");
            return;
        }
        
        var chunk = context.RiffFile.FindChunk(chunkPath);

        if (chunk == null)
        {
            Log($"Chunk {chunkPath} not found in file.");
            return;
        }

        var writer = new RiffWriter();
        writer.WriteChunk(file.Path.LocalPath, chunk);
    }

    private async void OpenFile_OnClick(object? sender, RoutedEventArgs e)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = GetTopLevel(this);

        if (topLevel == null)
        {
            return;
        }

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open RIFF File",
            AllowMultiple = false
        });

        if (files.Count < 1)
        {
            Log("File count was less than 1");
            return;
        }

        var file = files[0];

        IRiffFile parsedFile;
        try
        {
            parsedFile = new RiffReader(file.Path.LocalPath, false).ReadFile();
        }
        catch (Exception exception)
        {
            Log(exception);
            return;
        }

        Settings.Instance.LastOpenedFilePath = file.Path.LocalPath;
        Settings.Save();
        
        foreach (var ownedWindow in OwnedWindows)
        {
            ownedWindow.Close();
        }

        SetContextFile(parsedFile);
    }

    private void SetContextFile(IRiffFile? riffFile)
    {
        Log($"Setting file to context: {riffFile?.Path ?? "null"}");
        if (DataContext is not MainViewModel context)
        {
            Log($"context was null", LogType.Warning);
            context = new MainViewModel();
        }
        
        context.RiffFile = riffFile;
        UpdateRiffFileTree(context);
    }

    private StackPanel CreateRiffChunkHeader(IChunk chunk)
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10
        };

        stackPanel.Children.Add(CreateRiffChunkButton(chunk));
        stackPanel.Children.Add(
            new TextBlock
            {
                Text = GetRiffChunkExpanderHeaderAdditionalInfo(chunk),
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                HorizontalAlignment = HorizontalAlignment.Stretch
            }
        );

        return stackPanel;
    }

    private string GetRiffChunkExpanderHeaderAdditionalInfo(IChunk chunk)
    {
        return
            $"Offset: {chunk.Offset.ToString("N0", new CultureInfo("cs-CZ"))}, Length {chunk.Length.ToString("N0", new CultureInfo("cs-CZ"))} bytes";
    }

    private Button CreateRiffChunkButton(IChunk chunk)
    {
        var headerButton = new Button
        {
            Content = chunk switch
            {
                ListChunk listChunk => listChunk.Type,
                RiffChunk riffChunk => riffChunk.Format,
                _ => chunk.Identifier
            },
            IsEnabled = true
        };

        headerButton.Click += (_, _) =>
        {
            UpdateDetail(chunk);
            _currentDetailChunk = chunk;
        };

        return headerButton;
    }

    private void UpdateDetail(IChunk? chunk)
    {
        if (!_binaryDetail || chunk == null)
        {
            UpdateDetail(chunk?.ToString() ?? "null");
            return;
        }

        const int bytesPerLine = 16;
        const int byteLimit = 10000;

        var builder = new StringBuilder();
        byte[] chunkBytes = chunk.GetBytes();
        byte[] bytesToConvert = chunkBytes.Length > byteLimit ? chunkBytes.Take(byteLimit).ToArray() : chunkBytes;
        int bytesLeft = bytesToConvert.Length;

        Span<byte> currentBytes = stackalloc byte[bytesPerLine];
        Span<char> asciiConverted = stackalloc char[bytesPerLine];

        for (int i = 0; i < bytesToConvert.Length; i += bytesPerLine)
        {
            for (int j = 0; j < bytesPerLine; j++)
            {
                if (j >= bytesLeft)
                {
                    currentBytes[j] = 0;
                    asciiConverted[j] = '_';
                    continue;
                }

                currentBytes[j] = bytesToConvert[i + j];
                char currentChar = (char)currentBytes[j];
                asciiConverted[j] = char.IsAsciiLetterOrDigit(currentChar) || currentChar == ' ' ? currentChar : '_';
            }

            builder.AppendLine($"{BitConverter.ToString(currentBytes.ToArray())}\t{new string(asciiConverted)}");
            bytesLeft -= bytesPerLine;
        }

        if (bytesToConvert.Length == byteLimit)
        {
            builder.AppendLine($"...\nData longer than {byteLimit} bytes");
        }

        UpdateDetail(builder.ToString());
    }

    private void UpdateDetail(string text)
    {
        Detail.Text = text;
    }

    private void ChangeView_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_currentDetailChunk == null)
        {
            return;
        }

        _binaryDetail = !_binaryDetail;
        UpdateDetail(_currentDetailChunk);
    }

    private MainViewModel? GetDataContext()
    {
        return DataContext is not MainViewModel mainViewModel ? null : mainViewModel;
    }

    private async void SaveAs_OnClick(object? sender, RoutedEventArgs e)
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = GetTopLevel(this);
        var context = GetDataContext();
        
        if (topLevel == null || context?.RiffFile == null)
        {
            return;
        }
        
        // Start async operation to open the dialog.
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save RIFF File"
        });
        
        if (file == null)
        {
            Log("File was not not selected");
            return;
        }

        var writer = new RiffWriter();
        writer.Write(file.Path.AbsolutePath, context.RiffFile);
        
        Title = $"Riff Viewer - {Path.GetFileName(file.Path.AbsolutePath)}";
    }

    private void ShowWaveform_OnClick(object? sender, RoutedEventArgs e)
    {
        var context = GetDataContext();
        if (context?.RiffFile is not WavFile wavFile)
        {
            return;
        }

        _plotWindow?.Close();
        _plotWindow = new WavePlot(wavFile);
        _plotWindow.Show(this);
    }

    private void Play_OnClick(object? sender, RoutedEventArgs e)
    {
        var context = GetDataContext();
        if (context?.RiffFile == null || _outputDevice?.PlaybackState == PlaybackState.Playing)
        {
            return;
        }
        
        if (!File.Exists(Path.Join(TempDir, "temp.wav")))
        {
            if (!Directory.Exists(TempDir))
            {
                try
                {
                    Directory.CreateDirectory(TempDir);
                }
                catch (Exception exception)
                {
                    Log(exception);
                    return;
                }
                
            }
            
            var writer = new RiffWriter();
            writer.Write(Path.Join(TempDir, "temp.wav"), context.RiffFile);
        }

        if (_outputDevice?.PlaybackState == PlaybackState.Paused)
        {
            _outputDevice.Play();
            return;
        }
        
        if (_outputDevice == null)
        {
            _outputDevice = new WaveOutEvent();
            _outputDevice.PlaybackStopped += (_, _) => Play.IsEnabled = true;
        }

        try
        {
            _audioFile ??= new AudioFileReader(Path.Join(TempDir, "temp.wav"));
            
            _outputDevice.Init(_audioFile);
            _outputDevice.Play();
        }
        catch (Exception exception)
        {
            Log(exception);
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _outputDevice?.Dispose();
        _audioFile?.Dispose();
        
        RemoveTempFiles();
        
        base.OnClosed(e);
    }

    private void RemoveTempFiles()
    {
        var di = new DirectoryInfo(TempDir);

        foreach (var file in di.GetFiles())
        {
            file.Delete(); 
        }
        
        foreach (var dir in di.GetDirectories())
        {
            dir.Delete(true); 
        }
    }

    private void Pause_OnClick(object? sender, RoutedEventArgs e)
    {
        _outputDevice?.Pause();
    }

    private void Stop_OnClick(object? sender, RoutedEventArgs e)
    {
        _outputDevice?.Stop();
        _outputDevice = null;
        _audioFile?.Dispose();
        _audioFile = null;
        
        File.Delete(Path.Join(TempDir, "temp.wav"));
    }

    private void AddChunk_OnClick(object? sender, RoutedEventArgs e)
    {
        var context = GetDataContext();
        if (context?.RiffFile == null)
        {
            return;
        }

        var addChunkWindow = new AddChunkWindow(context.RiffFile);
        addChunkWindow.Closed += (_, _) => UpdateRiffFileTree(GetDataContext()!);
        addChunkWindow.Show(this);
    }
}