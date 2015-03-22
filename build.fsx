#r @"packages/FAKE.3.5.4/tools/FakeLib.dll"
#load "build-helpers.fsx"
#load "hockey-app-helpers.fsx"

open Fake
open Fake
open System
open System.IO
open System.Linq
open BuildHelpers
open Fake.XamarinHelper
open HockeyAppHelper

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

Target "android-deploy" (fun () ->
 Environment.SetEnvironmentVariable("HockeyAppApiToken", "ba597cb15f28485db212aa8a4d10037d")
 
 let buildCounter = BuildHelpers.GetBuildCounter TeamCityHelper.TeamCityBuildNumber
 
 let hockeyAppApiToken = Environment.GetEnvironmentVariable("HockeyAppApiToken")
 
 let appPath = "AndroidApp/bin/Release/AndroidApp.AndroidApp.apk"
 
 HockeyAppHelper.Upload hockeyAppApiToken appPath buildCounter
)

"android-build"
  ==> "android-package"

RunTarget() 