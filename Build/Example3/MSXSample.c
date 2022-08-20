//------------------------------------------------------------------------------
// Sample program of multicolored sprite of MSX2, with Z88DK
//
// Expected to be built as below:
// zcc +msx -create-app -lmsxbios -subtype=rom MSXSample.c -o sample.rom
#include <msx/gfx.h>
// Include the exported definitions
#include "spr_def.h"

// Prototypes
void buf2vram(unsigned short src, unsigned short dst, unsigned short len);

// Struct
typedef struct
{
	unsigned char y;
	unsigned char x;
	unsigned char id;
	unsigned char color;
} SpriteAttribute;

// Main routine
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
	buf2vram(sprptn, 0x3800, 0x0800);

	// Set sprite color of first handle
	buf2vram(&sprclr[0], 0x1C00, 16);
	// Set sprite color of second handle
	buf2vram(&sprclr[16], 0x1C10, 16);
	do
	{
		// Set sprite attribute of first handle
		spr_attr.y = y;
		spr_attr.x = x;
		spr_attr.id = 0;
		buf2vram(&spr_attr, 0x1E00, 4);
		// Set sprite attribute of second handle
		spr_attr.y = y;
		spr_attr.x = x;
		spr_attr.id = 4;
		buf2vram(&spr_attr, 0x1E04, 4);
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
unsigned short dst_addr;
unsigned short src_addr;
unsigned short cpy_size;
#asm
EXTERN	msxbios
DEFC	MSX_BIOSCALL_RDVRM = $004A
DEFC	MSX_BIOSCALL_WRTVRM = $004D
DEFC	MSX_BIOSCALL_SETWRT = $0053
#endasm
void buf2vram(unsigned short src, unsigned short dst, unsigned short len)
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
