del *.rom
del *.def
del *.lst
del *.bin
zcc +msx -create-app -lmsxbios -subtype=rom MSXSample.c -o test.rom
