Tfs2Gource==========Small [windows] command line designed to export changesets from TFS for use in Gource.## Usagetfs2Gource.exe -tfsUrl http://sheme:port/tfs/collection -u username -p password -do domain -output filename.logOnce the output is written the log file will be available for us in Gource.## RequirementsRequires .net 3.5 client profile and TFS APIs (recommend installing Microsoft Team Explorer if VS2010 isn't installed). Also requires Gource, and you'll want to know how to script the command-line to take this output and feed it into Gource.