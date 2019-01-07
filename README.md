# WinCompWPF
Hacking on a sample using Windows Composition APIs in WPF

## Status as of 12/14

![status gif](https://media.giphy.com/media/YlVSPYKUTi5f68weAz/giphy.gif)

Close up on lights targeting bar outlines [here](https://media.giphy.com/media/iqAR8LDJKqwuOmlN2l/giphy.mp4).

## Summary

Rough summary of what you'll find in the repo - 

- WPF app hosting class that implements HwndHost
- Uses pinvoke and COM interop to access additional platform APIs
- Custom Graph class and associated helpers 
- Graph uses Composition APIs and animations to enhance look & feel - including Implicit animation, Lights, and Shapes
- Text rendered using SharpDX

## TODO / Wish list

Stylistic
- Add more graph bar style options - shared linear gradient, blurred semi-transparent color, etc
- Trend line on top of bars
- Shadows? 3D transforms?

P3 
- Make graph orientation options (horizontal vs vertical)
- Update how data can be passed to the graph (include bar labels with data)
- Automate graph resizing based on parent container size
- Option for data sets (groups of bars)
- Bar hover-over text
- Ability to add data later on (abstract from constructor)
- Text change property events to auto trigger text redraw
- Options for min bar width
- Legend
