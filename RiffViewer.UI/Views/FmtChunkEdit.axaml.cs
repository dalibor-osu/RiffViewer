using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RiffViewer.Lib.Riff.Formats.Wav;

namespace RiffViewer.UI.Views;

public partial class FmtChunkEdit : Window
{
    private readonly FmtChunk _fmtChunk;
    private readonly WavFile _wavFile;
    
    public FmtChunkEdit(FmtChunk fmtChunk, WavFile wavFile)
    {
        Width = 350;
        Height = 500;
        
        _fmtChunk = new FmtChunk(fmtChunk);
        _wavFile = wavFile;
        
        InitializeComponent();
        
        Title = "Edit FMT Chunk";
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        AudioFormatBox.Text = _fmtChunk.AudioFormat.ToString();
        ChannelCount.Text = _fmtChunk.ChannelCount.ToString();
        SamplingRate.Text = _fmtChunk.SamplingRate.ToString();
        BitsPerSample.Text = _fmtChunk.BitsPerSample.ToString();
    }

    private void AudioFormatBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!short.TryParse(AudioFormatBox.Text, out short audioFormat))
        {
            return;
        }
        
        _fmtChunk.SetAudioFormat(audioFormat);
    }

    private void ChannelCount_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!short.TryParse(ChannelCount.Text, out short channelCount))
        {
            return;
        }
        
        _fmtChunk.SetChannelCount(channelCount);
        UpdateCalculatedValues();
    }

    private void SamplingRate_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!int.TryParse(SamplingRate.Text, out int samplingRate))
        {
            return;
        }
        
        _fmtChunk.SetSamplingRate(samplingRate);
        UpdateCalculatedValues();
    }

    private void BitsPerSample_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!short.TryParse(BitsPerSample.Text, out short bitsPerSample))
        {
            return;
        }
        
        _fmtChunk.SetBitsPerSample(bitsPerSample);
        UpdateCalculatedValues();
    }

    private void UpdateCalculatedValues()
    {
        DataRate.Text = _fmtChunk.DataRate.ToString();
        DataBlockSize.Text = _fmtChunk.DataBlockSize.ToString();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        _wavFile.SetFmtChunk(_fmtChunk);
        Close();
    }
}