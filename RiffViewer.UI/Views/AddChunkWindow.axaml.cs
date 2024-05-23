using System;
using System.IO;
using System.Net;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using PrettyLogSharp;
using RiffViewer.Lib.Reader;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using RiffViewer.Lib.Writer;
using static PrettyLogSharp.PrettyLogger;
    
namespace RiffViewer.UI.Views;

public partial class AddChunkWindow : Window
{
    private readonly IRiffFile _riffFile;
    private IBrush _defaultBrush = Brushes.Red;
    
    public AddChunkWindow(IRiffFile file)
    {
        _riffFile = file;
        
        Width = 400;
        Height = 250;
        
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        Position.Text = _riffFile.MainChunk.ChildChunks.Count.ToString();
        _defaultBrush = FileNameBlock.BorderBrush ?? _defaultBrush;
    }

    private async void SelectButton_OnClick(object? sender, RoutedEventArgs e)
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
            Title = "Select RIFF Chunk File",
            AllowMultiple = false
        });
        
        if (files.Count < 1)
        {
            Log("File was not not selected");
            return;
        }

        FileNameBlock.Text = files[0].Path.LocalPath;
    }

    private void ConfirmButton_OnClick(object? sender, RoutedEventArgs eventArgs)
    {
        string chunkPath = FileNameBlock.Text ?? string.Empty;
        if (!File.Exists(chunkPath))
        {
            FileNameBlock.BorderBrush = Brushes.Red;
            return;
        }
        
        IChunk chunk;
        
        // Read the chunk file
        {
            var chunkReader = new RiffReader(chunkPath, false);
            try
            {
                chunk = chunkReader.ReadChunk();
            }
            catch (Exception e)
            {
                Log("There was an error while reading the file with chunk:", LogType.Exception);
                Log(e.Message);
                return;
            }
        }

        if (!int.TryParse(Position.Text, out int position))
        {
            position = _riffFile.MainChunk.ChildChunks.Count;
        }
        
        _riffFile.MainChunk.InsertChunk(chunk, position);
        Close();
    }

    private void FileNameBlock_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (FileNameBlock != null)
        {
            // Move the caret to the end of the text
            FileNameBlock.CaretIndex = FileNameBlock.Text?.Length ?? 0;
            FileNameBlock.BorderBrush = _defaultBrush;
        }
    }

    private void Position_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (int.TryParse(Position.Text, out _))
        {
            Position.BorderBrush = _defaultBrush;
        }
        else
        {
            Position.BorderBrush = Brushes.Red;
        }
    }
}