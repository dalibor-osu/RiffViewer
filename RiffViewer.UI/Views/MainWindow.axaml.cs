using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using PrettyLogSharp;
using RiffViewer.Lib.Reader;
using RiffViewer.Lib.Riff;
using RiffViewer.Lib.Riff.Chunk;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using RiffViewer.UI.ViewModels;
using static PrettyLogSharp.PrettyLogger;
using Path = System.IO.Path;

namespace RiffViewer.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Settings.TryLoad();
        InitializeComponent();
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        if (Settings.Instance.LastOpenedFilePath == string.Empty)
        {
            return;
        }
        
        IRiffFile parsedFile;
        try
        {
            parsedFile = new RiffReader(Settings.Instance.LastOpenedFilePath).ReadFile();
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

        return control;
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
            parsedFile = new RiffReader(file.Path.AbsolutePath).ReadFile();
        }
        catch (Exception exception)
        {
            Log(exception);
            return;
        }

        Settings.Instance.LastOpenedFilePath = file.Path.AbsolutePath;
        Settings.Save();
        
        SetContextFile(parsedFile);
    }

    private void SetContextFile(IRiffFile? riffFile)
    {
        Log($"Setting file to context: { riffFile?.Path ?? "null" }");
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
                TextTrimming = TextTrimming.CharacterEllipsis
            }
        );

        return stackPanel;
    }

    private string GetRiffChunkExpanderHeaderAdditionalInfo(IChunk chunk)
    {
        return $"Offset: { chunk.Offset.ToString("N0", new CultureInfo("cs-CZ")) }, Length { chunk.Length.ToString("N0", new CultureInfo("cs-CZ")) } bytes";
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

        headerButton.Click += (_, _) => { UpdateDetail(chunk.ToString() ?? "null"); };

        return headerButton;
    }

    private void UpdateDetail(string text)
    {
        Detail.Text = text;
    }
}