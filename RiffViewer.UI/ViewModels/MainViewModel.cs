﻿using ReactiveUI;
using RiffViewer.Lib.Riff;

namespace RiffViewer.UI.ViewModels;

public class MainViewModel : ReactiveObject
{
#pragma warning disable CA1822 // Mark members as static
    public IRiffFile? RiffFile { get; set; }
#pragma warning restore CA1822 // Mark members as static
}