//------------------------------------------------------------------------------
// Sample program of multicolored sprite of MSX2, with Z88DK
//
// Expected to be built as below:
// zcc +msx -create-app -lmsxbios -subtype=rom MSXSample.c -o sample.rom
#include <msx/gfx.h>
// Include the exported definitions
#include "spr_def.h"
// Prototype declaration
void ram2vram(unsigned short, unsigned short, unsigned short);
// Struct
typedef struct
{
	unsigned char y;
	unsigned char x;
	unsigned char id;
	unsigned char color;
} SpriteAttribute;
// Main function
void main()
{
	unsigned char val;
	SpriteAttribute spr_attr;
	unsigned char x = 128, y = 79;
	// Initialize VDP
	set_mode(mode_2);
	// Sprite size is 16x16
	set_sprite_mode(sprite_large);
	// Graphic3 mode
	val = (get_vdp_reg(0) & 0xF1) | 0x04;
	set_vdp_reg(0, val);
	val = (get_vdp_reg(1) & 0xE7) | 0x00;
	set_vdp_reg(1, val);
	// Set sprite attribute table to 0x1E00
	set_vdp_reg(5, 0x3C);
	msx_vfill(0x1E00, 192 , 128);	// Erase all
	// Read sprite pattern generator table
	ram2vram(sprptn, 0x3800, 0x0800);
	// Set sprite color of first handle
	ram2vram(&sprclr[0], 0x1C00, 16);
	// Set sprite color of second handle
	ram2vram(&sprclr[16], 0x1C10, 16);
	do
	{
		// Set sprite attribute of first handle
		spr_attr.y = y;
		spr_attr.x = x;
		spr_attr.id = 0;
		ram2vram(&spr_attr, 0x1E00, 4);
		// Set sprite attribute of second handle
		spr_attr.y = y;
		spr_attr.x = x;
		spr_attr.id = 4;
		ram2vram(&spr_attr, 0x1E04, 4);
		// Move sprites
		val = msx_get_stick(0);
		if(val == 0) continue;
		if((val < 3) || (val > 7)) --y;
		if((val > 1) && (val < 5)) ++x;
		if((val > 5) && (val < 9)) --x;
		if((val > 3) && (val < 7)) ++y;
	} while(1);
}
// Library
#asm
extern 	msxbios
public	_ram2vram
defc	MSX_BIOSCALL_SETWRT = $0053
_ram2vram:
	ld		ix, 2
	add		ix, sp
	ld		d, (ix+5)
	ld		e, (ix+4)
	ld		h, (ix+3)
	ld		l, (ix+2)
	ld		b, (ix+1)
	ld		c, (ix+0)
	ld		ix, MSX_BIOSCALL_SETWRT
	call	msxbios
	di
_ram2vram_loop:
	ld		a,	(de)
	out		($98), a
	inc		de
	dec		bc
	ld		a, c
	or		b
	jr		nz, _ram2vram_loop
	ei
	ret
#endasm
