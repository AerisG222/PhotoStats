using System;
using System.CommandLine;


namespace PhotoStats
{
    public class PhotoStatsOptions
    {
        bool _help;
        string _photoRoot;

        public string PhotoRoot { get { return _photoRoot; } }


        public void Parse(string[] args)
        {
            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.ApplicationName = "SizePhotos";

                syntax.HandleHelp = false;

                syntax.DefineOption("h|help", ref _help, "help");
                syntax.DefineOption("p|photo-dir", ref _photoRoot, "Directory containing the source photos");

                if(_help)
                {
                    Console.WriteLine(syntax.GetHelpText());
                    Environment.Exit(0);
                }
                else
                {
                    ValidateOptions(syntax);
                }
            });
        }

        
        void ValidateOptions(ArgumentSyntax syntax)
        {
            if(string.IsNullOrWhiteSpace(PhotoRoot))
            {
                syntax.ReportError("Please specify the local path containing the photos to process");
            }
        }
    }
}
