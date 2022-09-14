//------------------------------------------------------------------------------
// Sample decoding the compressed map data
//
// Expected to be built as below:
// zcc +msx -create-app -lmsxbios -subtype=rom MSXSample.c -o sample.rom
#include <msx/gfx.h>
// Include the exported definitions
#include "pcg_def.h"
#include "map_def.h"
#include "spr_def.h"
// Prototype declaration
void ram2vram(unsigned short, unsigned short, unsigned short);
// Working buffer
static unsigned char vrmbuf[768];
// Main function
void main()
{
	unsigned char val;
	unsigned short i, j, p, q, x, y;
	unsigned char* ptr;
	unsigned char byte_left;
	// Set screen mode to 2
	set_mode(mode_2);
	msx_color(15, 0, 0);
	// Sprite size is 16x16
	set_sprite_mode(sprite_large);
	// Read pattern generator table
	ram2vram(ptngen, 0x0000, 0x0800);
	// 2nd and 3rd row
	ram2vram(ptngen, 0x0800, 0x0800);
	ram2vram(ptngen, 0x1000, 0x0800);
	// Read pattern color table
	ram2vram(ptnclr, 0x2000, 0x0800);
	// 2nd and 3rd row
	ram2vram(ptnclr, 0x2800, 0x0800);
	ram2vram(ptnclr, 0x3000, 0x0800);
	// Read sprite pattern generator table
	ram2vram(sprptn, 0x3800, 0x0800);
	// Show sprite
	put_sprite_16(0, 128, 79, 1, 15);
	// Show map
	x = 0;
	y = 0;
	while(1)
	{
		for(i = 0; i < 12; ++i)
		{
			// Offset to each row is in the head of compressed data
			q = *(unsigned short*)(&mapData_compressed[(y + i) * 2]);
			ptr = mapData_compressed + q;
			q = 0;
			byte_left = 0;
			for(j = 0; j < 16; ++j)
			{
				// Get map data of col
				while(q <= x + j)
				{
					if(byte_left == 0)
					{
						byte_left = *ptr++;	// number of consecutive data
						val = *ptr++;		// one map datum
					}
					q++;			// current x in data
					byte_left--;
				}
				// Draw one pattern corresponding to one map datum
				p = (i << 6) + (j << 1);
				vrmbuf[p] = mapptn[(val << 2) + 0];
				vrmbuf[p + 1] = mapptn[(val << 2) + 1];
				vrmbuf[p + 32] = mapptn[(val << 2) + 2];
				vrmbuf[p + 33] = mapptn[(val << 2) + 3];
			}
		}
		ram2vram(vrmbuf, 0x1800, 768);
		do
		{
			val = msx_get_stick(0);
			if((val == 3) && (x < 48))
				++x;
			if((val == 7) && (x > 0))
				--x;
			if((val == 1) && (y > 0))
				--y;
			if((val == 5) && (y < 52))
				++y;
		}
		while(val == 0);
	}
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
