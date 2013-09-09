param($installPath, $toolsPath, $package, $project)
$projectPath = [System.IO.Path]::GetDirectoryName($project.FullName)
$projectNamespace = ($project.Properties | Where-Object { $_.Name -eq "RootNamespace" } | Select-Object -First 1).Value

Function ReplaceOldFrameworkNamespaces {
    param($file)

	Write-Host "Processing $file to remove old namespaces"
    
    $content = Get-Content $file.FullName
    $replaceCount = 0
    
    $replacements = @{
        "Curtin.Framework.Auth" = @("Curtin.Framework.Common.Auth", "Curtin.Framework.Integration.Auth", "Curtin.Framework.Web.Auth", "Curtin.Framework.Web.Controllers.Filters");
        "Curtin.Framework.Azure" = @("Curtin.Framework.Azure.Diagnostics", "Curtin.Framework.Azure.Storage");
        "Curtin.Framework.Container" = @("Curtin.Framework.Azure.Container", "Curtin.Framework.Common.Container", "Curtin.Framework.Integration.Container", "Curtin.Framework.Web.Container", "Curtin.Framework.Web.Controllers.FilterProviders");
        "Curtin.Framework.Controllers" = @("Curtin.Framework.Web.Controllers");
        "Curtin.Framework.Data" = @("Curtin.Framework.Database.Migrations", "dummy");
        "Curtin.Framework.FlowForms" = @("Curtin.Framework.Web.FlowForms");
        "Curtin.Framework.Helpers" = @("Curtin.Framework.Web.FlowForms.Helpers");
        "Curtin.Framework.Meta" = @("Curtin.Framework.Web.Pages");
        "Curtin.Framework.NHibernate" = @("Curtin.Framework.Database.NHibernate");
        "Curtin.Framework.UserService" = @("Curtin.Framework.Integration.UserService", "Curtin.Framework.Common.Classes", "Curtin.Framework.Common.UserService");
        "Curtin.Framework.Utils" = @("Curtin.Framework.Common.Attributes", "Curtin.Framework.Common.Classes", "Curtin.Framework.Common.Extensions", "Curtin.Framework.Common.Utils", "Curtin.Framework.Common.WebRequest", "Curtin.Framework.Web.Extensions", "Curtin.Framework.Web.Menu", "Curtin.Framework.Web.Controllers.Filters");
        "Curtin.Framework.Views" = @("Curtin.Framework.Web.Views");
        "Curtin.Framework.Web" = @("Curtin.Framework.Web", "Curtin.Framework.Web.Extensions", "Curtin.Framework.Web.Controllers.ActionResults", "Curtin.Framework.Web.Controllers.Constraints", "Curtin.Framework.Web.Pages", "Curtin.Framework.Web.Controllers.Filters", "Curtin.Framework.Common.Classes");
        "Curtin.Framework.TestHelpers.Utils" = @("Curtin.Framework.Test.AutoSubstitute", "Curtin.Framework.Test.Utils", "Curtin.Framework.Test.Web", "Curtin.Framework.Test.WebRequest");
        "AutoMockContainer" = @("AutoSubstituteContainer");
        "MvcContrib.TestHelper" = @("Curtin.Framework.Test.Web");
        "RedirectToHttps" = @("RedirectToHttpsAttribute");
        "FluentlyUpdateDatabase" = @("Migrations.UpdateDatabase");
        "UpdateDatabase" = @("Migrations.UpdateDatabase");
        "FluentlyMigrations.UpdateDatabase" = @("Migrations.UpdateDatabase");
        "SqlMigration" = @("Curtin.Framework.Data.SqlMigration");
        "\[StringValue" = @("[System.ComponentModel.Description");
    }
    
    foreach($oldNamespace in $replacements.Keys) {        

        $searchString = $oldNamespace;
        $replacementString = $replacements[$oldNamespace][0];
        if ($replacements[$oldNamespace].Length -gt 1) {
            $replacementString = "";
            $searchString = "using $oldNamespace;";
            foreach($newNamespace in $replacements[$oldNamespace]) {
                $replacementString += "using $newNamespace;`r`n";
            }
        }
        $content = $content | ForEach-Object { [regex]::Replace($_, $searchString, [System.Text.RegularExpressions.MatchEvaluator]{$replaceCount ++; return $replacementString}) }
    }

	$content = $content -creplace "AutoMock", "AutoSubstitute";
	$content = $content -creplace "autoMock", "autoSubstitute";
	$content = $content -creplace "automock", "autoSubstitute";
	$content = $content -creplace "_automock", "_autoSubstitute";
	$content = $content -creplace "_autoMock", "_autoSubstitute";
    
    if ($replaceCount -gt 0) {
        [IO.File]::WriteAllLines($file.FullName, ($content -join "`r`n"))
    }
}

Function HasOldFramework {
    param($projectPath)
    
    $packagesPath = [System.IO.Path]::Combine($projectPath, "packages.config")
    # If there is a packages.config file
    if (Test-Path $packagesPath) {
        $curtinFramework = ([xml](Get-Content $packagesPath)).SelectNodes("/packages/package[@id='Curtin.Framework']")
        if ($curtinFramework.Count -gt 0) {
            return $true
        }
    }
    return $false
}

Function ReplaceDbTestBase {
    param($path, $toolsPath, $namespace)
    
    $content = Get-Content ([System.IO.Path]::Combine($toolsPath, "DbTestBase.cstemp"))
    $content = $content -replace "\%namespace\%", $namespace
    [IO.File]::WriteAllLines($path, ($content -join "`r`n"))

    Write-Host
    Write-Warning "Replaced DbTestBase.cs completely - if there was any custom code then this needs to be replaced - check the Git diff! Also; you will need to do an alt+enter on the NHibernateConfiguration class in that file."
    Write-Host
}

Function ReplaceMigrationTests {
    param($path, $toolsPath, $namespace, $dbtestbaseNamespace)
    
    $content = Get-Content ([System.IO.Path]::Combine($toolsPath, "MigrationTests.cstemp"))
    $content = $content -replace "\%namespace\%", $namespace
    $content = $content -replace "\%dbtestbasenamespace\%", $dbtestbaseNamespace
    [IO.File]::WriteAllLines($path, ($content -join "`r`n"))

    Write-Host
    Write-Warning "Replaced MigrationTests.cs completely - if there was any custom code then this needs to be replaced - check the Git diff!"
    Write-Host
}

Function RemoveOldFramework() {
    param($projectName, $projectPath, $projectNamespace, $toolsPath)

    # Is Curtin.Framework installed?
    Write-Host "Checking for old Curtin.Framework"
    $hasOldFramework = HasOldFramework($projectPath)
    if (!$hasOldFramework) {
        return
    }
    
    # Uninstall Curtin.Framework
    Write-Host "Project" $projectName "has Curtin.Framework installed; uninstalling and updating namespaces."
    if (Get-Command "Uninstall-Package" -errorAction SilentlyContinue) {
        if ($projectName -match "Test") {
            Uninstall-Package Curtin.Framework.TestHelpers
            Uninstall-Package Castle.Core
            Uninstall-Package MvcContrib.Mvc3.TestHelper-ci
            Uninstall-Package RhinoMocks
            Install-Package Curtin.Framework.Test
        }
        Uninstall-Package Curtin.Framework
        Uninstall-Package NHibernateProfiler -errorAction SilentlyContinue
        Uninstall-Package HibernatingRhinos.Profiler.Appender.v4.0 -errorAction SilentlyContinue
        Uninstall-Package WebActivator -errorAction SilentlyContinue
    } else {
        Write-Host
        if ($projectName -match "Test") {
            Write-Warning "[!!!] Unable to find NuGet commands; please manually uninstall Curtin.Framework, Curtin.Framework.TestHelpers, Castle.Core, RhinoMocks and MvcContrib.Mvc3.TestHelper-ci and install Curtin.Framework.Test!"
        } else {
            Write-Warning "[!!!] Unable to find NuGet commands; please manually uninstall Curtin.Framework!"
        }
        Write-Host
    }
    
    # Find all .cs files and replace old namespaces
    $files = Get-ChildItem -recurse $projectPath *.cs
    if ($files.Count -gt 0) {
        foreach($file in $files) {
            ReplaceOldFrameworkNamespaces($file)
        }
        Write-Host
        Write-Warning "[!!!] Run ReSharper code optimisation to optimise usings before building."
        Write-Host
    }
    
    # Replace namespace in FlowForms.tt file
    $file = Get-ChildItem -recurse $projectPath FlowForm.tt
    if (!($file -eq $null)) {
        (Get-Content $file.FullName) |
            Foreach-Object {$_ -replace "Curtin.Framework.FlowForms", "Curtin.Framework.Web.FlowForms"} |
            Set-Content $file.FullName
    }

    # Add database namespace to Global.asax.cs so that a call to Migrations.UpdateDatabase() can compile
    $file = Get-ChildItem $projectPath Global.asax.cs
    if (!($file -eq $null)) {
        $content = Get-Content $file.FullName
        Set-Content $file.FullName "using Curtin.Framework.Database.Migrations;`r`n", $content
    }

    # Perform actions if it's a test project
    if ($projectName -match "Test") {
        $dbTestBaseNamespace = ""
        # Replace DbTestBase.cs with new version
        if (Test-Path ([System.IO.Path]::Combine($projectPath, "Helpers\\DbTestBase.cs"))) {
            $dbTestBaseNamespace = "$projectNamespace.Helpers"
            ReplaceDbTestBase ([System.IO.Path]::Combine($projectPath, "Helpers\\DbTestBase.cs")) $toolsPath $dbTestBaseNamespace
        }
        if (Test-Path ([System.IO.Path]::Combine($projectPath, "Data\\DbTestBase.cs"))) {
            $dbTestBase = "$projectNamespace.Data"
            ReplaceDbTestBase ([System.IO.Path]::Combine($projectPath, "Data\\DbTestBase.cs")) $toolsPath $dbTestBaseNamespace
        }
        # Replace MigrationTests.cs with new version
        if (Test-Path ([System.IO.Path]::Combine($projectPath, "Database\\MigrationTests.cs"))) {
            ReplaceMigrationTests ([System.IO.Path]::Combine($projectPath, "Database\\MigrationTests.cs")) $toolsPath "$projectNamespace.Database" $dbTestBaseNamespace
        }
        if (Test-Path ([System.IO.Path]::Combine($projectPath, "Data\\MigrationTests.cs"))) {
            ReplaceMigrationTests ([System.IO.Path]::Combine($projectPath, "Data\\MigrationTests.cs")) $toolsPath "$projectNamespace.Data" $dbTestBaseNamespace
        }
    }
}

Write-Host "Installing Curtin.Framework.Common"
RemoveOldFramework $project.Name $projectPath $projectNamespace $toolsPath
