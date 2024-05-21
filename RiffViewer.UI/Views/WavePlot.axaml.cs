using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using RiffViewer.Lib.Riff.Chunk.Interfaces;
using RiffViewer.Lib.Riff.Formats.Wav;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.AxisRules;
using ScottPlot.DataSources;
using ScottPlot.TickGenerators;

namespace RiffViewer.UI.Views;

public partial class WavePlot : Window
{
    private readonly WavFile _file;

    public WavePlot(WavFile file)
    {
        _file = file;

        InitializeComponent();
        
        Title = $"Riff Viewer - {Path.GetFileName(_file.Path)} waveform";
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        var signal = WaveformPlot.Plot.Add.Signal(new FastSignalSourceDouble(GetAmplitudeData(), 1.0 / _file.FmtChunk.SamplingRate));
        
        WaveformPlot.Plot.Axes.Rules.Add(new LockedVertical(WaveformPlot.Plot.Axes.Left, -1.2, 1.2));
        WaveformPlot.Plot.Axes.Rules.Add(new LockedCenterY(WaveformPlot.Plot.Axes.Left, 0));
        WaveformPlot.Plot.Axes.Rules.Add(new MaximumBoundary(WaveformPlot.Plot.Axes.Bottom, WaveformPlot.Plot.Axes.Left,
            new AxisLimits(new CoordinateRange(0, _file.GetLengthInSeconds()), new CoordinateRange(-1.5, 1.5))));

        WaveformPlot.Plot.Axes.Bottom.Label.Text = "Time [s]";
        WaveformPlot.Plot.Axes.Left.Label.Text = "Amplitude";
        
        WaveformPlot.Refresh();
    }

    private double[] GetAmplitudeData()
    {
        if (_file.MainChunk.FindSubChunk("data") is not IDataChunk dataChunk)
        {
            return [];
        }

        int bytesPerSample = _file.FmtChunk.BitsPerSample / 8;
        int samplesCount = dataChunk.Data.Length / _file.FmtChunk.DataBlockSize;
        double maxValue = Math.Pow(2, _file.FmtChunk.BitsPerSample);

        byte[] currentBytes = new byte[bytesPerSample];
        double[] samples = new double[samplesCount];

        for (int i = 0; i < samplesCount; i++)
        {
            Array.Copy(dataChunk.Data, i * _file.FmtChunk.DataBlockSize, currentBytes, 0, bytesPerSample);
            double value = BitConverter.ToInt16(currentBytes) / maxValue * 2;
            samples[i] = value;
        }

        return samples;
    }
}