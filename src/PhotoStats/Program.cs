using System;
using System.IO;
using NMagickWand;
using NMagickWand.Enums;


namespace PhotoStats
{
    class Program
    {
        readonly PhotoStatsOptions _opts;
        
        
        public Program(PhotoStatsOptions opts)
        {
            _opts = opts;
        }
        
        
        public static void Main(string[] args)
        {
            var opts = new PhotoStatsOptions();
            opts.Parse(args);
            
            var p = new Program(opts);
            p.Run();
        }
        
        
        void Run()
        {
            MagickWandEnvironment.Genesis();
            
            var qr = (double) MagickWandEnvironment.QuantumRange;
            
            WriteLine(new string[] { "Quantum Range", $"{qr}" });
            Console.WriteLine();
            WriteLine(new string[] { "Image Name", "Mean", "StdDev", "Mean (pct)", "StdDev (pct)", "Kurtosis", "Skew" });
            
            foreach(var file in Directory.GetFiles(_opts.PhotoRoot))
            {
                double mean, stddev, kurtosis, skewness;
                
                using(var wand = new MagickWand(file))
                {
                    wand.AutoLevelImage();
                    wand.GetImageChannelMean(ChannelType.AllChannels, out mean, out stddev);
                    wand.GetImageChannelKurtosis(ChannelType.AllChannels, out kurtosis, out skewness);
                    
                    WriteLine(new string[] { $"{Path.GetFileName(file)}", $"{mean}", $"{stddev}", $"{mean / qr}", $"{stddev / qr}", $"{kurtosis}", $"{skewness}" });
                }
            }
            
            MagickWandEnvironment.Terminus();
        }
        
        
        void WriteLine(params object[] vals)
        {
            Console.WriteLine(string.Join("\t", vals));
        }
    }
}
