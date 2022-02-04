<#
This script will run on debug.
It will load in a PowerShell command shell and import the module developed in the project. To end debug, exit this shell.
#>
# Load the module.
$env:PSModulePath = (Resolve-Path .).Path + ";" + $env:PSModulePath
Import-Module 'VstTools' #-Verbose

$message = "| Exit this shell to end the debug session! |"
$line = "-" * $message.Length
$color = "Cyan"
Write-Host -ForegroundColor $color $line
Write-Host -ForegroundColor $color $message
Write-Host -ForegroundColor $color $line


Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Composite Morphing\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\Composite Morphing\"
Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Distortion\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\Distortion\"
Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Human Voices\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\Human Voices\"
Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Oneshots\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\Oneshots\"
Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Phrases\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\Phrases\"
Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Psychoacoustic\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\Psychoacoustic\"
Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Retro\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\Retro\"
Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\SFX and Noise\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\SFX and Noise\"
Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Synthesizers\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\Synthesizers\"
Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Textures\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\Textures\"
Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Traditional\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\Traditional\"
Convert-OmnisphereToBitwig -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Waveforms\" -SaveTo "C:\Users\Mark\Desktop\Multisamples\Waveforms\"




#Import-OmnisphereFileSystem -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Synthesizers\Synthesizers 01.db" -SaveTo "C:\Users\Mark\Desktop\Test\Synthesizers\"
#Import-OmnisphereFileSystem -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Composite Morphing\Composite Morphing 01.db" -SaveTo "C:\Users\Mark\Desktop\Test\Composite Morphing\"
#Import-OmnisphereFileSystem -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Oneshots\Oneshots 01.db" -SaveTo "C:\Users\Mark\Desktop\Test\Oneshots\"
#Import-OmnisphereFileSystem -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Human Voices\Human Voices 01.db" -SaveTo "C:\Users\Mark\Desktop\Test\Human Voices\"
#Import-OmnisphereFileSystem -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Phrases\Phrases 01.db" -SaveTo "C:\Users\Mark\Desktop\Test\Phrases\"
#Import-OmnisphereFileSystem -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Psychoacoustic\Psychoacoustic01.db" -SaveTo "C:\Users\Mark\Desktop\Test\Psychoacoustic\"
#Import-OmnisphereFileSystem -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Retro\Retro 01.db" -SaveTo "C:\Users\Mark\Desktop\Test\Retro\"
#Import-OmnisphereFileSystem -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\SFX and Noise\SFX and Noise 01.db" -SaveTo "C:\Users\Mark\Desktop\Test\SFX and Noise\"
#Import-OmnisphereFileSystem -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Textures\Textures 01.db" -SaveTo "C:\Users\Mark\Desktop\Test\Textures\"
#Import-OmnisphereFileSystem -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Traditional\Traditional 01.db" -SaveTo "C:\Users\Mark\Desktop\Test\Traditional\"
#Import-OmnisphereFileSystem -Source "C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Waveforms\Waveforms 01.db" -SaveTo "C:\Users\Mark\Desktop\Test\Waveforms\"


