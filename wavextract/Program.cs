using System;
using System.Text;
using System.IO;

namespace wavextract
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("usage: wavextract <filename>");
                return;
            }
            BinaryReader reader;
            try
            {
                reader = new BinaryReader(new FileStream(args[0], FileMode.Open), Encoding.ASCII);
            } catch (FileNotFoundException e)
            {
                Console.WriteLine("file not found.");
                return;
            }
            Directory.CreateDirectory("wav_output");

            int num_files = 0;

            while (true)
            {
                try
                {
                    long position = reader.BaseStream.Position;
                    if (reader.ReadByte() != 0x52 || new string(reader.ReadChars(3)) != "IFF") 
                        continue;
                    int chunk_size = reader.ReadInt32();
                    if (new string(reader.ReadChars(4)) == "WAVE")
                    {
                        reader.BaseStream.Position = position;
                        byte[] wav = reader.ReadBytes(chunk_size + 8);

                        Console.WriteLine("output/{0}.wav", position.ToString("X"));
                        BinaryWriter writer =
                            new BinaryWriter(new FileStream(@"wav_output\" + position.ToString("X") + ".wav", FileMode.Create),
                                             Encoding.ASCII);
                        num_files++;
                        writer.Write(wav);
                        writer.Flush();
                        writer.Close();
                    }
                }
                catch (EndOfStreamException)
                {
                    if (num_files == 0)
                    {
                        Directory.Delete("wav_output");
                    }
                    Console.WriteLine("Done! extracted {0} files", num_files);
                    return;
                }
            }
        }
    }
}
