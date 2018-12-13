# WinCompWPF
Hacking on a sample using Windows Composition APIs in WPF

## Status as of 12/13

![status gif](https://i.makeagif.com/media/12-13-2018/4i7bmZ.gif)

## Summary

Rough summary of what you'll find in the repo - 

- WPF app hosting class that implements HwndHost
- Uses pinvoke and COM interop to access additional platform APIs
- Custom Graph class and associated helpers 
- Graph uses Composition APIs and animations to enhance look & feel
- Text rendered using SharpDX

Sped up linear gradient animation on bars: 

![linear gradient animating gif](https://i.makeagif.com/media/12-13-2018/e0_Xg0.gif)
