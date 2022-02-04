param(
	[Parameter()] $ProjectName,
	[Parameter()] $ConfigurationName,
	[Parameter()] $TargetDir
)

Copy '*.dl*' '.\VstTools' -Force -Verbose
