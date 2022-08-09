//------------------------------------------------------------------------------
// Sample program to handle the expored data on MSX, with Z88DK
#include <msx/gfx.h>
#include "pcg_def.h"
#include "map_def.h"
void msx1_vwrite_block(unsigned short src, unsigned short dst, unsigned short len);
void msx1_vram2vram(unsigned short dst, unsigned short src, unsigned short size);

// Main
void main()
{
	unsigned char val;
	unsigned char vrmbuf[768];
	unsigned short i, j, p, q, x, y;
	// Set screen mode to 2
	set_mode(mode_2);
	msx_color(15, 0, 0);
	// Set pattern generator table
	msx1_vwrite_block(ptngen, 0x0000, 0x0800);
	// Set pattern color table
	msx1_vwrite_block(ptnclr, 0x2000, 0x0800);
	// Copy definitions to 2nd and 3rd row
	msx1_vram2vram(0x0800, 0x0000, 0x0800);
	msx1_vram2vram(0x1000, 0x0000, 0x0800);
	msx1_vram2vram(0x2800, 0x2000, 0x0800);
	msx1_vram2vram(0x3000, 0x2000, 0x0800);

	// Show sandbox
	msx1_vwrite_block(nametable, 0x1800, 768);
	do {
		val = get_trigger(0);
	} while(!val);	// hit space bar to exit



	// Show map
	x = 0;
	y = 0;
	do
	{
		for(i = 0; i < 12; ++i)
		{
			for(j = 0; j < 16; ++j)
			{
				q = mapData[(y << 6) + x + (i << 6) + j];
				p = (i << 6) + (j << 1);
				vrmbuf[p] = mapptn[(q << 2) + 0];
				vrmbuf[p + 1] = mapptn[(q << 2) + 1];
				vrmbuf[p + 32] = mapptn[(q << 2) + 2];
				vrmbuf[p + 33] = mapptn[(q << 2) + 3];
			}
		}
		msx1_vwrite_block(vrmbuf, 0x1800, 768);
	}
	while(1);
}

// Library
unsigned short dst_addr;
unsigned short src_addr;
unsigned short cpy_size;
#asm
EXTERN	msxbios
DEFC	MSX_BIOSCALL_RDVRM = $004A
DEFC	MSX_BIOSCALL_WRTVRM = $004D
DEFC	MSX_BIOSCALL_SETWRT = $0053
#endasm
void msx1_vwrite_block(unsigned short src, unsigned short dst, unsigned short len)
{
	dst_addr = dst;
	src_addr = src;
	cpy_size = len;
#asm
	DI
	LD	BC, (_cpy_size)
	LD	DE, (_src_addr)
	LD	HL, (_dst_addr)
	LD	IX, MSX_BIOSCALL_SETWRT
	CALL	msxbios
MSX1_VWRITE_LOOP:
	DI
	LD	A,	(DE)
	OUT	($98), A
	INC	DE
	DEC	BC
	LD	A, C
	OR	B
	JR	NZ, MSX1_VWRITE_LOOP
	EI
#endasm
}
void msx1_vram2vram(unsigned short dst, unsigned short src, unsigned short size)
{
	dst_addr = dst;
	src_addr = src;
	cpy_size = size;
#asm
	DI
	LD		HL, (_cpy_size)
	LD		C, L
	LD		B, H
	LD		HL, (_dst_addr)
	EX		DE, HL
	LD		HL,	(_src_addr)
VRM_CPY_LOOP:
	LD		IX, MSX_BIOSCALL_RDVRM
	CALL 	msxbios
	EX		DE, HL
	LD		IX, MSX_BIOSCALL_WRTVRM
	CALL 	msxbios
	EX		DE, HL
	INC		DE
	INC		HL
	DEC		BC
	LD		A, B
	OR		C
	JR		NZ, VRM_CPY_LOOP
	EI
#endasm
}

