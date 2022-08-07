//-----------------------------------------------------
// Decode compressed data
/*
	On real machine, decode the compressed data as below
	This should be optimized, but as a sample
*/
void decode(unsigned char* src, unsigned char* dst)
{
	unsigned char dict[256 * 3];
	unsigned char stack[16];
	unsigned char v, stack_cnt = 0;
	unsigned short dict_size, data_size, work;
	unsigned char* end_ptr;

	memset(dict, 0, 256 * 3);		// Clear dictionary buffer
    dict_size = *src++ << 8;		// Read dictionary size
    dict_size |= *src++;
    end_ptr = src + dict_size;
    while (src < end_ptr)   		// Dictionary to buffer
    {
        v = *src++;
        work = v + v + v;
        dict[work + 0] = 1;	// Exists
        dict[work + 1] = *src++;
        dict[work + 2] = *src++;
    }
    data_size = *src++ << 8;		// Read data size
    data_size |= *src++;
    end_ptr = src + data_size;
	// Decode
	while (1)
	{
	    // Get data from stack or read new data
	    if (stack_cnt != 0)
	    {
	        v = stack[--stack_cnt];
	    }
	    else
	    {
	        if (src == end_ptr)
	        {
	            break;				// End decoding
	        }
	        v = *src++;
	    }
	    // See if corresponding pair exists
        work = v + v + v;
	    if (dict[work] == 1)
	    {
	        // Push pair into the stack
	        stack[stack_cnt++] = dict[work + 2];
	        stack[stack_cnt++] = dict[work + 1];
	    }
	    else
	    {
	        // Write data into output buffer
	        *dst++ = v;
	    }
	}
}
