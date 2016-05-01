using System.Collections.Generic;
using CommandLine;


namespace PhotoStats
{
    public class PhotoStatsOptions
    {
        [Option('p', "photo-dir", HelpText = "Directory containing the source photos")]
        public string PhotoRoot { get; set; }
        
        
        public IEnumerable<string> ValidateOptions()
        {
            if(string.IsNullOrWhiteSpace(PhotoRoot))
            {
                yield return "Please specify the local path containing the photos to process";
            }
        }
    }
}
