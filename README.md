# WinCompWPF
Hacking on a sample using Windows Composition APIs in WPF

## Status as of 12/14

![status gif](https://media.giphy.com/media/2fJLyxV561XaqglsQM/giphy.gif)

Close up on lights targeting bar outlines [here](https://media.giphy.com/media/iqAR8LDJKqwuOmlN2l/giphy.mp4).

## Summary

Rough summary of what you'll find in the repo - 

- WPF app hosting class that implements HwndHost
- Uses pinvoke and COM interop to access additional platform APIs
- Custom Graph class and associated helpers 
- Graph uses Composition APIs and animations to enhance look & feel - including Implicit animation, Lights, and Shapes
- Text rendered using SharpDX
