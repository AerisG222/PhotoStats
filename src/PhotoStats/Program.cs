using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;
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
            var parser = new Parser(config => config.HelpWriter = null);
            var result = parser.ParseArguments<PhotoStatsOptions>(args);
            
            var exitCode = result.MapResult(
                opts => 
                {
                    var errs = opts.ValidateOptions().ToList();
                    
                    if(errs.Count > 0)
                    {
                        ShowUsage(errs);
                        return 1;
                    }
            
                    var p = new Program(opts);
                    p.Run();
                    return 0;
                },
                errors =>
                {
                    ShowUsage();
                    return 1;
                }
            );
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
        
        
        static void ShowUsage(IList<string> errors = null)
        {
            var help = new HelpText();
            
            help.Heading = "PhotoStats";
            help.AddPreOptionsLine("A tool to suggest optimization parameters for SizePhotos");
            
            // this is a little lame, but force a NotParsed<T> options result
            // so that we can get a nice help screen.  this might be required
            // if the passed args are valid to the parser, but not w/ custom 
            // validation logic that runs after parsing
            var parser = new Parser(config => config.HelpWriter = null);
            var result = parser.ParseArguments<PhotoStatsOptions>(new string[] { "--xxx" });
            help.AddOptions(result);
            
            if(errors != null)
            {
                help.AddPostOptionsLine("Errors:");
                help.AddPostOptionsLines(errors);
            }
            
            Console.WriteLine(help.ToString());            
        }
    }
}
