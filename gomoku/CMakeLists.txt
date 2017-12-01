enable_language(CSharp)
project(gomoku)

cmake_minimum_required(VERSION 3.10)

file(GLOB_RECURSE SRC src/*.cs)

add_executable(gomoku ${SRC})

set_property(TARGET gomoku PROPERTY VS_DOTNET_REFERENCES
    "Microsoft.CSharp"
    "PresentationCore"
    "PresentationFramework"
    "System"
	"System.Drawing"
    "System.Core"
    "System.Data"
    "System.Data.DataSetExtensions"
    "System.Xaml"
    "System.Xml"
    "System.Xml.Linq"
    "WindowsBase")