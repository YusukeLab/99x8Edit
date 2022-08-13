using System;
using System.Collections.Generic;
using System.Text;

namespace _99x8Edit
{
    public class Compression
    {
        public enum Type
        {
            BytePair,
            RunLength
        }
        public static CompressionBase Create(Type type)
        {
            if (type == Type.BytePair)
            {
                return new CompressBPE();
            }
            else if (type == Type.RunLength)
            {
                return new CompressRLE();
            }
            return null;
        }
    }
    public abstract class CompressionBase
    {
        // Compression
        public abstract byte[] Encode(byte[] source); 
    }
    public class CompressRLE : CompressionBase
    {
        // Compression based on run length encoding
        public override byte[] Encode(byte[] source)
        {
            List<byte> outputData = new List<byte>();
            int current_data = -1;
            int count = 0;
            for (int i = 0; i < source.Length; ++i)
            {
                if ((int)(source[i]) != current_data)
                {
                    if (count > 0)                      // New type of data started
                    {
                        outputData.Add((byte)count);   // Record size and value of previous data
                        outputData.Add((byte)current_data);
                    }
                    count = 1;
                    current_data = (int)source[i];
                }
                else
                {
                    if (count == 255)                  // Size data is only one byte
                    {
                        outputData.Add((byte)count);   // Record size and value of previous data
                        outputData.Add((byte)current_data);
                        count = 1;
                    }
                    else
                    {
                        count++;
                    }
                }
            }
            outputData.Add((byte)count);      // Record size and value of previous data
            outputData.Add((byte)current_data);
            return outputData.ToArray();
        }
    }
    public class CompressBPE : CompressionBase
    {
        // Compression based on byte pair encoding
        public override byte[] Encode(byte[] source)
        {
            const int Threshold = 3;                    // Quit when there are only 3 pairs
            // for output
            Dictionary<byte, int> pair_table = new Dictionary<byte, int>();
            byte[] work_buffer = source.Clone() as byte[];   // Default output buffer =  input data
            int[] chr_used = new int[256];
            int compressed_size = work_buffer.Length;
            do
            {
                // Count all pairs
                Dictionary<int, int> pairs_count = new Dictionary<int, int>();
                for (int i = 0; i < compressed_size - 1; ++i)
                {
                    byte left = work_buffer[i + 0];              // look one pair
                    byte right = work_buffer[i + 1];
                    int index = ((left << 8) | right);      // pair to 2 byte key
                    // Count up the value according to pair
                    if(!pairs_count.ContainsKey(index))     // pair will be the key
                        pairs_count.Add(index, 1);
                    else
                        pairs_count[index] = pairs_count[index] + 1;
                    // Record the value which appeared already
                    if(chr_used[left] < 255) chr_used[left]++;
                    if (chr_used[right] < 255) chr_used[right]++;
                }
                // Search unused value
                int unused = -1;
                for(int i = 0; i < 256; ++i)
                {
                    if(chr_used[i] == 0)
                    {
                        unused = i;
                        break;
                    }
                }
                // Search most frequent byte pairs
                int bestcount = Threshold - 1;
                int bestpair = 0;
                foreach (KeyValuePair<int, int> kvp in pairs_count)
                {
                    if (kvp.Value > bestcount)
                    {
                        bestcount = kvp.Value;
                        bestpair = kvp.Key;
                    }
                }
                // If there are enough pairs and unused value, compress
                if ((bestcount < Threshold) || (unused == -1))
                {
                    break;  // End compression
                }
                pair_table.Add((byte)unused, bestpair);
                // Replace the most frequent pairs to unused value
                int buffer_read_index = 0;
                int buffer_write_index = 0;
                int best_left = bestpair >> 8;
                int best_right = bestpair & 0xFF;
                while (buffer_read_index < compressed_size)
                {
                    if(buffer_read_index < compressed_size - 1)
                    {
                        if ((best_left == work_buffer[buffer_read_index]) && (best_right == work_buffer[buffer_read_index + 1]))
                        {
                            work_buffer[buffer_write_index] = (byte)unused;
                            ++buffer_read_index;
                        }
                        else
                        {
                            work_buffer[buffer_write_index] = work_buffer[buffer_read_index];
                        }
                    }
                    else
                    {
                        work_buffer[buffer_write_index] = work_buffer[buffer_read_index];
                    }
                    ++buffer_read_index;
                    ++buffer_write_index;
                }
                compressed_size = buffer_write_index;
                chr_used[unused] = 1;
            }
            while (true);
            // Create buffer for output
            List<byte> ret = new List<byte>();
            // Make dictionary
            int dictionary_size = pair_table.Count * 3;
            // Output size of dictionary
            ret.Add((byte)(dictionary_size >> 8));
            ret.Add((byte)(dictionary_size & 0xFF));
            foreach (KeyValuePair<byte, int> kvp in pair_table)
            {
                // Output dictionary
                ret.Add((byte)(kvp.Key));
                ret.Add((byte)(kvp.Value >> 8));
                ret.Add((byte)(kvp.Value & 0xFF));
            }
            // Output size of buffer
            ret.Add((byte)(compressed_size >> 8));
            ret.Add((byte)(compressed_size & 0xFF));
            // Output buffer
            for(int i = 0; i < compressed_size; ++ i)
            {
                ret.Add(work_buffer[i]);
            }
            System.Diagnostics.Debug.WriteLine("Compress: orginal buffer size:" + work_buffer.Length.ToString());
            System.Diagnostics.Debug.WriteLine("Compress: compressed buffer size:" + ret.Count.ToString());
            return ret.ToArray();
        }
    }
}
