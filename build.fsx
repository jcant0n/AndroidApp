#r @"packages/FAKE.3.5.4/tools/FakeLib.dll"
#load "build-helpers.fsx"
open Fake
open Fake
open System
open System.IO
open System.Linq
open BuildHelpers
open Fake.XamarinHelper

Target "android-build" (fun () ->
 MSBuild "AndroidApp/bin/Release" "Build" [ ("Configuration", "Release"); ("Platform", "Any CPU") ] [ "AndroidApp.sln" ] |> ignore
)

Target "android-package" (fun () ->
    AndroidPackage (fun defaults ->
        {defaults with
            ProjectPath = "AndroidApp/AndroidApp.csproj"
            Configuration = "Release"
            OutputPath = "AndroidApp/bin/Release"
        })
    |> fun file -> TeamCityHelper.PublishArtifact file.FullName
)

"android-build"
  ==> "android-package"
  
RunTarget() 