# About 99x8Edit

99x8Edit is a graphic editor for systems based on TI TMS9918 and Yamaha V9938,
such as MSX, Spectravideo, Sega Master System, and so on.  
The aim of 99x8Edit is to be a simple and practical editor for TMS9918/V9938
applications, so any feedbacks and requests are welcome.  
99x8Edit is free to use, and released under MIT license.  

# Features
Editing - Tiles, sprites, and map. Editing is as easy as operating Excel.  
Export - Various Formats including assembly, C, raw data, and MSX BASIC(BSAVE format).  
Import - Imports raw data, MSX BASIC format, PNG image.  
Undo/Redo  
Compression - Exported data can be compressed. Includes sample program to decode on real machine.  
Peek - ROM data can be imaged for easy reference.  
Font - TrueType fonts can be converted automatically for reference.  
CRT Filter - Includes CRT filter to view how it looks on real machines.

# Contact

Twitter:@chocolatechnica

# How to use

## Main Window

"A" icon - PCG Editor.  
map icon - Map Editor.  
human icon - Sprite editor.  
Export button - Export the data to be used in your program.  

	C header - table definition for C source  
	C compressed - table definition for C source, compressed  
	ASM data - table definition for assembly source  
	ASM compressed - table definition for assemble source, compressed  
	MSX BASIC - format to be loaded by MSX BASIC, the BLOAD command  
	raw data - raw binary with no header  
	raw compressed - raw binary with no header, compressed  

	Data will be compressed by byte pair encoding,  
	but exceptionally, map data will be compressed by run length encoding,  
	since we have to decode it realtime.  
	To decode the compressed data on actual machines, see attached samples.  

Open icon - Load all settings.  
Save icon - Save all settings. For save as, right click the icon.  
Eye icon - Peek ROM data and other specified data.  
Undo - Undo action.  
Redo - Redo action.  
About - Version info.  

## PCG window

PCG Editor - Here you can edit the selected character.   
	Click the cell to edit each dots.  
	Right click for further actions.  
	Keyboard shortcuts are as below.  
PCG - Select the character to edit. Right click for copy/paste actions.  
Current color - Foreground and backbround color of the selected row.  
	Click to change color.  
Sandbox - In the sandbox, you can check how the character looks like.  
	Characters can be drag&dropped from PCG, of pressing enter.  
Palette - Available colors. Double click to edit (only for V9938).  
TMS9918 - Fix the palette to the colors of TMS9918.  
CRT Filter - Emulates how it looks like on CRT displays.  

### Keyboard shortcuts in the editor
	Cursor	Move current selection.  
	Space	Set/Reset one dot.  
	CTRL+C	Copy one 8x1 line.  
	CTRL+V	Paste one 8x1 line.  
	Delete	Clear one 8x1 line.  
	1-8	Set/Reset the pixel corresponding to 1-8.  
	+/-	Change the foreground color.  
	[/]	Change the background color.  
	,/.	Switch current color(background/foreground).  

## Map window
On map window, you can create a map to be loaded in your game.  
One map pattern is four PCG characters.  

PCG - The PCG characters defined on PCG window.  
Tile patterns - Defines four characters of one map pattern.  
	Characters can be drag&dropped from PCG.   
Map editor - The map. Pattern can be drag&dropped.  
	Right click for copy/paste and further actions.  

## Sprite window

Sprite Editor - Edit the selected sprite.  
	Click the cell to edit each dots.  
	Keyboard shortcuts are available also.  
  
	Sprite editing on V9938(not TMS9918)  
	When sprites are overlayed on V9938, color "OR" appears,  
	which is the synthesis of two colors.  
	OR color will be defined automatically from the colors of  
	two overlayed sprites.  
	Also, on V9938, you can set colors for each lines.  

Overlay - You can view how it looks like when two sprites are overlayed.  
Sprites - Select the sprite to edit.  
  
Enjoy!  
