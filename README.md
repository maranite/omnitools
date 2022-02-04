# Introduction
This project creates a PowerShell module that provides Cmdlets for working with Bitwig, Omnisphere and VST presets.
The specific intent of the project was to allow Bitwig presets to be created from Omnisphere patches and multis, but along the way 
a variety of capabilities ended up being implemented:
* Locate and list preset libraries Spectrasnoics plugins   (Omnisphere, Keylab, Trilian).
* Locate multis and presets within each library.
* Read and Write factory preset files  (*.db)
* Read and Write patches  (*.prt_omn | *.prt_key )
* Read and Write multis   (*.mlt_omn | *.mlt_key )
* Modify metadata tags for patches and multis.
* Create multis from patches.
* Read and Write Bitwig preset files.
* Manipulate metadata for Bitwig presets.
* Create Bitwig presets from patches or multis.

In time, capabilities for working with Synthmaster and Serum presets will be included.
Eventually, it is my intention to create a service which automatically synchronizes Bitwig presets to Omnisphere patches.

# Getting Started
Install the module:         
PS> Install-Module -Name VstTools

See the list of available commands:             
PS> Get-Command -Module VstTools

Change directory to the Spectrasnoics steam folder:             
PS> Get-SteamFolder | cd

Load a single patch and change the rating and save it:              
PS> $patch = Open-OmnispherePatch -Source "C:\ProgramData\STEAM\Spectrasonics\Omnisphere\Settings Library\Patches\User\patch2.prt_omn"
PS> $patch.Metadata["Rating"] = "5"
PS> $patch.Metadata["Genres"] = @("Funky","Jazz","Pop")
PS> $patch.SaveAs("C:\ProgramData\STEAM\Spectrasonics\Omnisphere\Settings Library\Patches\User\patch2.prt_omn")

Convert all Omnisphere patches to Bitwig:                   
PS> $patches = Get-PatchesFolder
PS> Convert-OmnisphereToBitwig -Source $patches -Target "C:\Users\[username]\Documents\Bitwig\Library" 

# Build and Test
This code is builtand release automatically by VisualStudio TFS. 
If you would like to build it for yourself, use Visual Studio 2017.

# Contribute
If you would like to contribute to this project, please contact me directly.
